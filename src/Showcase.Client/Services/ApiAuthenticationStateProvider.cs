using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace Showcase.Client.Services;

public class ApiAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IAuthApiService _authService;

    public ApiAuthenticationStateProvider(IAuthApiService authService)
    {
        _authService = authService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _authService.GetTokenAsync();
        if (string.IsNullOrEmpty(token))
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        var claims = JwtParser.ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(
            claims,
            "jwt",
            ClaimTypes.Name,
            "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");

        var user = new ClaimsPrincipal(identity);

        return new AuthenticationState(user);
    }

    public void NotifyUserAuthentication(string token)
    {
        var claims = JwtParser.ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(
            claims,
            "jwt",
            ClaimTypes.Name,
            "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");

        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public void NotifyUserLogout()
    {
        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
    }

    //private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    //{
    //    var claims = new List<Claim>();
    //    var payload = jwt.Split('.')[1];
    //    var jsonBytes = ParseBase64WithoutPadding(payload);
    //    var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

    //    if (keyValuePairs != null)
    //    {
    //        foreach (var kvp in keyValuePairs)
    //        {
    //            var claimType = kvp.Key == "role" ? ClaimTypes.Role : kvp.Key;
    //            claims.Add(new Claim(claimType, kvp.Value?.ToString() ?? string.Empty));
    //        }

    //    }

    //    return claims;
    //}

    //private static byte[] ParseBase64WithoutPadding(string base64)
    //{
    //    switch (base64.Length % 4)
    //    {
    //        case 2: base64 += "=="; break;
    //        case 3: base64 += "="; break;
    //    }
    //    return Convert.FromBase64String(base64);
    //}
}
