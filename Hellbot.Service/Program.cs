using Hellbot.Core.Events;
using Hellbot.Service.Clients.Twitch;
using Hellbot.Service.Config;
using Hellbot.Service.Data;
using Hellbot.Service.EventBus;
using Hellbot.Service.EventBus.Handlers;
using Hellbot.Service.EventBus.Producers;
using Scrutor;
using Serilog;
using Serilog.Enrichers.ShortTypeName;
using Serilog.Events;
using TwitchLib.EventSub.Websockets.Extensions;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Warning()
    .MinimumLevel.Override("Hellbot", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithShortTypeName()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3} {ShortTypeName}] {Message:lj}{NewLine}{Exception}"
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
builder.Services.AddOptions<TwitchOptions>()
    .Bind(builder.Configuration.GetSection("Twitch"))
    .Validate(o => !string.IsNullOrEmpty(o.API.ClientSecret), "ClientSecret required!")
    .ValidateOnStart();

// Database
builder.Services.Configure<DbOptions>(builder.Configuration.GetSection("Database"));
builder.Services.AddSingleton<IDbConnectionFactory, SqliteConnectionFactory>();
builder.Services.AddScoped<EventTable>();
builder.Services.AddSingleton<DbInitializer>();

// Event Bus
builder.Services.AddSingleton<IEventBus, HellbotEventBus>();
builder.Services.AddTwitchLibEventSubWebsockets();
builder.Services.AddSingleton<TwitchClient>();

// Handlers
builder.Services.Scan(scan => scan
    .FromAssembliesOf(typeof(IEventHandler))
    .AddClasses(classes => classes.AssignableTo<IEventHandler>())
    .UsingRegistrationStrategy(RegistrationStrategy.Append)
    .AsImplementedInterfaces()
    .WithScopedLifetime());

// Producers
builder.Services.AddHostedService<HeartbeatProducer>();
builder.Services.AddHostedService<TwitchEventSubProducer>();

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
    var dbInit = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    await dbInit.InitializeAsync();

    var handlers = scope.ServiceProvider.GetServices<IEventHandler>().ToList();
    foreach (var handler in handlers)
    {
        handler.Register(scope.ServiceProvider.GetRequiredService<IEventBus>());
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
