using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using Showcase.Client;
using Showcase.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:8000";

// --- Anonymous HttpClient (used only by AuthApiService for login/refresh) ---
builder.Services.AddHttpClient("AnonymousApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// --- Auth Service & State Provider ---
builder.Services.AddScoped<IAuthApiService, AuthApiService>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var anonymousClient = factory.CreateClient("AnonymousApi");
    var js = sp.GetRequiredService<IJSRuntime>();
    return new AuthApiService(anonymousClient, js);
});

builder.Services.AddScoped<ApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<ApiAuthenticationStateProvider>());

// --- Authorization system ---
builder.Services.AddAuthorizationCore();

// --- Register AuthorizationMessageHandler ---
builder.Services.AddTransient<AuthorizationMessageHandler>();

// --- Typed HttpClients (these use JWT automatically) ---
builder.Services.AddHttpClient<IRoleApiService, RoleApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler(sp =>
    new AuthorizationMessageHandler(sp.GetRequiredService<IAuthApiService>()));

builder.Services.AddHttpClient<IUserApiService, UserApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler(sp =>
    new AuthorizationMessageHandler(sp.GetRequiredService<IAuthApiService>()));

builder.Services.AddHttpClient<IProductApiService, ProductApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler(sp =>
    new AuthorizationMessageHandler(sp.GetRequiredService<IAuthApiService>()));

await builder.Build().RunAsync();
