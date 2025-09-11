using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Showcase.Client;
using Showcase.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Base API URL from configuration
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:8000";

// Register HttpClient
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

// Register API services
builder.Services.AddScoped<IAuthApiService, AuthApiService>();
builder.Services.AddScoped<IUserApiService, UserApiService>();
builder.Services.AddScoped<IProductApiService, ProductApiService>();

await builder.Build().RunAsync();
