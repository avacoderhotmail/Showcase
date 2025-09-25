using Microsoft.AspNetCore.Components;
using Showcase.Contracts.Contracts.Product;
using System.Net.Http.Json;

namespace Showcase.Client.Services
{
    public class BreadcrumbService : IBreadcrumbService
    {
        private readonly HttpClient _http;
        private readonly NavigationManager _nav;

        public BreadcrumbService(HttpClient http, NavigationManager nav)
        {
            _http = http;
            _nav = nav;

            // set BaseAddress if not already set
            if (_http.BaseAddress == null)
                _http.BaseAddress = new Uri("https://localhost:8000/");
        }
        public async Task<string?> ResolveSegmentAsync(string segment, string parentPath)
        {
            if (int.TryParse(segment, out var productId) && parentPath.StartsWith("products"))
            {
                try
                {
                    var product = await _http.GetFromJsonAsync<ProductReadDto>($"api/products/{productId}");
                    return product?.Name ?? $"Product {productId}";
                }
                catch(Exception ex)
                {
                    return $"Product {productId}";
                }
            }
            else
            {
                return char.ToUpper(segment[0]) + segment.Substring(1);
            }

        }
    }
}
