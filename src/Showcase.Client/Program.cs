using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Showcase.Client;
using Showcase.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:8000";

builder.Services.AddHttpClient<IAuthApiService, AuthApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Annonymous HttpClient (no auth)
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl)
});

// --- Auth Service & State Provider ---
builder.Services.AddScoped<IAuthApiService, AuthApiService>();
builder.Services.AddScoped<ApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<ApiAuthenticationStateProvider>());

// --- Authorization system ---
builder.Services.AddAuthorizationCore();

// --- Register AuthorizationMessageHandler ---
builder.Services.AddTransient<AuthorizationMessageHandler>();

// --- Typed HttpClients (use AuthorizationMessageHandler for JWT) ---
builder.Services.AddHttpClient<IUserApiService, UserApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler(sp => new AuthorizationMessageHandler(sp.GetRequiredService<IAuthApiService>()));

builder.Services.AddHttpClient<IProductApiService, ProductApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler(sp => new AuthorizationMessageHandler(sp.GetRequiredService<IAuthApiService>()));

await builder.Build().RunAsync();
