using Hellbot.UI.Components;
using Hellbot.UI.Configuration;
using Hellbot.UI.Services;
using Microsoft.Extensions.Options;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

builder.Services.Configure<HellbotApiOptions>(
    builder.Configuration.GetSection(HellbotApiOptions.SectionName));

builder.Services.AddHttpClient("api", (sp, client) =>
{
    var baseUrl = sp.GetRequiredService<IOptions<HellbotApiOptions>>().Value.BaseUrl.TrimEnd('/') + "/";
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddScoped<EventFeed>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
