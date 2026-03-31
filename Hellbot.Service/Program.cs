using Hellbot.Core.Events;
using Hellbot.Service.EventBus;
using Hellbot.Service.EventBus.Handlers;
using Hellbot.Service.EventBus.Handlers.Global;
using Hellbot.Service.EventBus.Handlers.Test;
using Hellbot.Service.EventBus.Producers;
using Scrutor;

var builder = WebApplication.CreateBuilder(args);

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

app.Run();
