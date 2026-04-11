using FluentMigrator.Runner;
using Hellbot.Core.Events;
using Hellbot.Core.Sessions;
using Hellbot.Service.Clients.ElevenLabs;
using Hellbot.Service.Clients.OBS;
using Hellbot.Service.Clients.Twitch;
using Hellbot.Service.Config;
using Hellbot.Service.Data;
using Hellbot.Service.Data.Migrations;
using Hellbot.Service.Data.Tables;
using Hellbot.Service.EventBus;
using Hellbot.Service.EventBus.Handlers;
using Hellbot.Service.EventBus.Middleware;
using Hellbot.Service.EventBus.Producers;
using Hellbot.Service.Tts;
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

// Database
builder.Services.Configure<DbOptions>(builder.Configuration.GetSection("Database"));
builder.Services.AddSingleton<IDbConnectionFactory, SqliteConnectionFactory>();
builder.Services.AddScoped<EventTable>();
builder.Services.AddScoped<VoiceProfilesTable>();


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
builder.Services.AddSingleton<IEventBus, HellbotEventBus>();
builder.Services.AddSingleton<ITtsQueue, TtsQueue>();
builder.Services.AddSingleton<IAudioPlayer, NAudioPlayer>();
builder.Services.AddSingleton<IStreamSessionManager, StreamSessionManager>();

// Event Producers
builder.Services.AddSingleton<ElevenLabsClient>();
builder.Services.AddTwitchLibEventSubWebsockets();
builder.Services.AddSingleton<TwitchClient>();
builder.Services.AddSingleton<OBSWebsocket>();
builder.Services.AddSingleton<ObsClient>();

// Handlers
builder.Services.Scan(scan => scan
    .FromAssembliesOf(typeof(IEventHandler))
    .AddClasses(classes => classes.AssignableTo<IEventHandler>())
    .UsingRegistrationStrategy(RegistrationStrategy.Append)
    .AsImplementedInterfaces()
    .WithScopedLifetime());

// Middleware (order matters, but not really)
builder.Services.AddSingleton<IEventMiddleware, EventLogger>();
builder.Services.AddSingleton<IEventMiddleware, StreamSessionContextEnricher>();

// Producers
builder.Services.AddHostedService<HeartbeatProducer>();
builder.Services.AddHostedService<TtsWorker>();
builder.Services.AddHostedService<TwitchEventSubProducer>();
builder.Services.AddHostedService<ObsEventProducer>();

builder.Services.AddSignalR();
builder.Services.AddControllers();

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

using (var scope = app.Services.CreateScope())
{
    var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

    var handlers = scope.ServiceProvider.GetServices<IEventHandler>().ToList();
    foreach (var handler in handlers)
    {
        handler.Register(eventBus);
    }
}

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
