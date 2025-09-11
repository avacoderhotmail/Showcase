﻿using System.Net.Http.Headers;

namespace Showcase.Client.Services;

public class AuthorizationMessageHandler : DelegatingHandler
{
    private readonly IAuthApiService _auth;

    public AuthorizationMessageHandler(IAuthApiService auth) => _auth = auth;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _auth.GetTokenAsync();
        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}
