using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Showcase.Client.Services;

public class AuthorizationMessageHandler : DelegatingHandler
{
    private readonly IAuthApiService _auth;

    public AuthorizationMessageHandler(IAuthApiService auth)
    {
        _auth = auth;
        //InnerHandler = new HttpClientHandler(); // final handler
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _auth.GetTokenAsync();

        if (string.IsNullOrWhiteSpace(token) || AuthApiService.IsExpiringSoon(token))
        {
            var refreshed = await _auth.RefreshTokenAsync();
            if(!refreshed) return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);

            token = await _auth.GetTokenAsync();
        }
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
