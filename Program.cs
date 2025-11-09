using BlazingStory.Components;
using BlazingStory.McpServer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ServerApp1.Stories.Components;
using ServerApp1.Stories.Components.Pages;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var serverUrl = builder.Configuration[WebHostDefaults.ServerUrlsKey]?.Split(';').FirstOrDefault() ?? "http://localhost:5166";
builder.Services.TryAddScoped(sp => new HttpClient { BaseAddress = new Uri(serverUrl) });
builder.Services.AddBlazingStoryMcpServer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// Map BlazingStory MCP endpoints on the root URL, as "/mcp/blazingstory".
app.MapBlazingStoryMcp();

// Map the BlazingStory server component to the "/stories" path.
app.Map("/stories", appBuilder =>
{
    appBuilder.UseStaticFiles();
    appBuilder.UseRouting();
    appBuilder.UseAntiforgery();
    appBuilder.UseEndpoints(endpoints =>
    {
        endpoints.MapRazorComponents<BlazingStoryServerComponent<IndexPage, IFramePage>>()
            .AddInteractiveServerRenderMode();
    });
});

// Map the default app on the root URL.
app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();
#pragma warning disable ASP0014 // Suppress ASP0014 because this code intentionally mounts multiple Blazor application entry points.
app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorComponents<DefaultApp>()
        .AddInteractiveServerRenderMode();
});
#pragma warning restore ASP0014

app.Run();
