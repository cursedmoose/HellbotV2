using Hellbot.Core.Events;
using Hellbot.Service.EventBus;
using Hellbot.Service.EventBus.Handlers;
using Hellbot.Service.EventBus.Producers;
using Scrutor;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Warning()
    .MinimumLevel.Override("Hellbot", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
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
// Add services to the container.
builder.Services.AddSingleton<IEventBus, HellbotEventBus>();

// Handlers
builder.Services.Scan(scan => scan
    .FromAssembliesOf(typeof(IEventHandler))
    .AddClasses(classes => classes.AssignableTo<IEventHandler>())
    .UsingRegistrationStrategy(RegistrationStrategy.Append)
    .AsImplementedInterfaces()
    .WithSingletonLifetime());

// Producers
builder.Services.AddHostedService<HeartbeatProducer>();

builder.Services.AddSignalR();
builder.Services.AddSingleton<SignalREventBroadcaster>();
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
    var handlers = scope.ServiceProvider.GetServices<IEventHandler>().ToList();
    foreach (var handler in handlers)
    {
        handler.Register(scope.ServiceProvider.GetRequiredService<IEventBus>());
    }
}

app.Services.GetRequiredService<SignalREventBroadcaster>();

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
