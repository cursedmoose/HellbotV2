using Hellbot.Core.Events;
using Hellbot.Service.EventBus;
using Hellbot.Service.EventBus.Handlers.Global;
using Hellbot.Service.EventBus.Handlers.Test;
using Hellbot.Service.EventBus.Producers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IEventBus, HellbotEventBus>();

// Handlers
builder.Services.AddSingleton<EventLogger>();
builder.Services.AddSingleton<TestMessageHandler>();

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

app.Services.GetRequiredService<EventLogger>();
app.Services.GetRequiredService<SignalREventBroadcaster>();

app.Run();
