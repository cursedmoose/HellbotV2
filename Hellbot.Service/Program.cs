using Dapper;
using FluentMigrator.Runner;
using Hellbot.Core.Events;
using Hellbot.Service.Sessions;
using Hellbot.Service.Audio;
using Hellbot.Service.Clients.ElevenLabs;
using Hellbot.Service.Clients.OBS;
using Hellbot.Service.Clients.Playnite;
using Hellbot.Service.Clients.Twitch;
using Hellbot.Service.Clients.Whisper;
using Hellbot.Service.Commands;
using Hellbot.Service.Config;
using Hellbot.Service.Data;
using Hellbot.Service.Data.Migrations;
using Hellbot.Service.Data.Tables;
using Hellbot.Service.Entitlements;
using Hellbot.Service.EventBus;
using Hellbot.Service.EventBus.Handlers;
using Hellbot.Service.Status;
using Hellbot.Service.EventBus.Middleware;
using Hellbot.Service.EventBus.Producers;
using Hellbot.Service.Stats;
using Hellbot.Service.Tts;
using Hellbot.Service.Users;
using OBSWebsocketDotNet;
using Scrutor;
using Serilog;
using Serilog.Enrichers.ShortTypeName;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using TwitchLib.EventSub.Websockets.Extensions;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Warning()
    .MinimumLevel.Override("Hellbot", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithShortTypeName()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3} {ShortTypeName}] {Message:lj}{NewLine}{Exception}",
        theme: AnsiConsoleTheme.Code
    )
    .WriteTo.File(
        path: "bin/logs/log-.json",
        formatter: new Serilog.Formatting.Json.JsonFormatter(),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        shared: true,
        flushToDiskInterval: TimeSpan.FromSeconds(1)
    )
    .CreateLogger();

Console.OutputEncoding = System.Text.Encoding.UTF8;
Log.Information($"Application Starting: {DateTime.Now}");
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// Config
builder.Configuration.AddUserSecrets<Program>();
builder.Services.AddOptions<TwitchOptions>()
    .Bind(builder.Configuration.GetSection("Twitch"))
    .Validate(o => !string.IsNullOrEmpty(o.API.ClientSecret), "Twitch:API:ClientSecret required!")
    .ValidateOnStart();
builder.Services.AddOptions<ObsOptions>().Bind(builder.Configuration.GetSection("OBS"));
builder.Services.AddOptions<ElevenLabsOptions>()
    .Bind(builder.Configuration.GetSection("ElevenLabs"))
    .Validate(o => !string.IsNullOrEmpty(o.ApiKey), "ElevenLabs:ApiKey required!")
    .ValidateOnStart();
builder.Services.Configure<DbOptions>(builder.Configuration.GetSection("Database"));
builder.Services.Configure<WhisperOptions>(builder.Configuration.GetSection("Whisper"));
builder.Services.Configure<PlayniteOptions>(builder.Configuration.GetSection("Playnite"));
builder.Services.Configure<StreamSessionOptions>(builder.Configuration.GetSection("StreamSession"));
builder.Services.Configure<UserStatsOptions>(builder.Configuration.GetSection("UserStats"));


SqlMapper.AddTypeHandler(new SqliteGuidTypeHandler());
builder.Services.AddSingleton<IDbConnectionFactory, SqliteConnectionFactory>();
builder.Services.AddScoped<IDbContext, SqliteDbContext>();
builder.Services.AddSingleton<UserCache>();

builder.Services.Scan(scan => scan
    .FromAssemblyOf<EventTable>()
    .AddClasses(classes => classes.Where(t => t.Name.EndsWith("Table")))
    .AsSelf()
    .WithScopedLifetime());


builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(rb => {
        var connectionString = builder.Configuration.GetSection("Database").GetRequiredSection("ConnectionString").Value;
        rb.AddSQLite()
        .WithGlobalConnectionString(connectionString)
        .ScanIn(typeof(M001_CreateEventsTable).Assembly)
        .For.Migrations();
    });

builder.Services.AddHostedService<Hellbot.Service.Data.MigrationRunner>();

// Event Bus
builder.Services.AddSingleton<ServiceStatusProvider>();
builder.Services.AddSingleton<IEventBus, HellbotEventBus>();
builder.Services.AddSingleton<ITtsPlaybackGate, TtsPlaybackGate>();
builder.Services.AddSingleton<ITtsQueue, TtsQueue>();
builder.Services.AddSingleton<IAudioPlayer, NAudioPlayer>();
builder.Services.AddSingleton<IStreamingChannelUpdater, TwitchStreamingChannelUpdater>();
builder.Services.AddSingleton<IStreamSessionManager, StreamSessionManager>();

// Event Producers
builder.Services.AddSingleton<ElevenLabsClient>();
builder.Services.AddTwitchLibEventSubWebsockets();
builder.Services.AddSingleton<TwitchClient>();
builder.Services.AddSingleton<OBSWebsocket>();
builder.Services.AddSingleton<ObsClient>();
builder.Services.AddSingleton<WhisperClient>();
builder.Services.AddSingleton<PlayniteClient>();

// Handlers
builder.Services.Scan(scan => scan
    .FromAssembliesOf(typeof(IEventHandler))
    .AddClasses(classes => classes.AssignableTo<IEventHandler>())
    .UsingRegistrationStrategy(RegistrationStrategy.Append)
    .AsImplementedInterfaces()
    .WithScopedLifetime());

builder.Services.Scan(scan => scan
    .FromAssembliesOf(typeof(ICommandHandler))
    .AddClasses(classes => classes.AssignableTo<ICommandHandler>())
    .UsingRegistrationStrategy(RegistrationStrategy.Append)
    .AsImplementedInterfaces()
    .WithScopedLifetime());

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEntitlementService, EntitlementService>();

builder.Services.AddSingleton<UserStatsRecorder>();
builder.Services.AddSingleton<IUserStatsRecorder>(static sp => sp.GetRequiredService<UserStatsRecorder>());
builder.Services.AddScoped<IUserStatsReader, UserStatsReader>();
builder.Services.AddHostedService<UserStatsFlushWorker>();

// Middleware runs in registration order; enrich context before EventLogger so logs include Context.Stream.
builder.Services.AddScoped<IEventMiddleware, StreamSessionContextEnricher>();
builder.Services.AddScoped<IEventMiddleware, UserContextEnricher>();
builder.Services.AddScoped<IEventMiddleware, UserPreferenceEnricher>();
builder.Services.AddScoped<IEventMiddleware, EventLogger>();

// Producers
builder.Services.AddHostedService<HeartbeatProducer>();
builder.Services.AddHostedService<TtsWorker>();
builder.Services.AddHostedService<TwitchEventSubProducer>();
builder.Services.AddHostedService<ObsEventProducer>();
builder.Services.AddHostedService<MicCaptureService>();
builder.Services.AddHostedService<PlayniteEventProducer>();

builder.Services.AddSignalR();
builder.Services.AddControllers()
    .AddJsonOptions(o =>
        o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapHub<EventHub>("/eventsHub");
app.MapControllers();

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application crashed");
}
finally
{
    Log.CloseAndFlush();
}
