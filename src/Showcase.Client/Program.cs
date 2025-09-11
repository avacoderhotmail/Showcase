using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Showcase.Client;
using Showcase.Client.Services;
using AuthorizationMessageHandler = Showcase.Client.Services.AuthorizationMessageHandler;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:8000";

// --- Anonymous HttpClient (used only by AuthApiService) ---
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

// --- Auth Service & State Provider ---
builder.Services.AddScoped<IAuthApiService, AuthApiService>();
builder.Services.AddScoped<ApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<ApiAuthenticationStateProvider>());

// --- Authorization system ---
builder.Services.AddAuthorizationCore();

// --- Register handler that injects JWT ---
builder.Services.AddTransient<AuthorizationMessageHandler>(sp =>
{
    var handler = new AuthorizationMessageHandler(sp.GetRequiredService<IAccessTokenProvider>(), sp.GetRequiredService<IAuthApiService>());
    //handler.ConfigureHandler(
    //    authorizedUrls: new[] { apiBaseUrl },
    //    scopes: new[] { "your-api-scope" } // Optional, if you're using scopes
    //);
    return handler;
});



// --- Typed HttpClients (use AuthorizationMessageHandler for JWT) ---
builder.Services.AddHttpClient<IUserApiService, UserApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<AuthorizationMessageHandler>();

builder.Services.AddHttpClient<IProductApiService, ProductApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<AuthorizationMessageHandler>();

await builder.Build().RunAsync();
