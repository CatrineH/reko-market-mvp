using Scalar.AspNetCore;

namespace reko_mini_project.Server.Configurations;

public static class ScalarExtensions
{
    private const string _securitySchemeName = "OAuth2";
    private const string _clientIdConfigKey = "Scalar:OAuthClientId";
    private const string _audienceConfigKey = "AzureAd:Audience";
    private const string _redirectUriConfigKey = "Scalar:RedirectUri";

    public static WebApplication MapScalarDocs(this WebApplication app)
    {
        app.MapOpenApi();

        var clientId = app.Configuration[_clientIdConfigKey]
            ?? throw new InvalidOperationException($"{_clientIdConfigKey} is not configured.");
        var audience = app.Configuration[_audienceConfigKey]
            ?? throw new InvalidOperationException($"{_audienceConfigKey} is not configured.");
        var redirectUri = app.Configuration[_redirectUriConfigKey]
            ?? throw new InvalidOperationException($"{_redirectUriConfigKey} is not configured.");

        app.MapScalarApiReference("/scalar", options =>
        {
            options
                .AddPreferredSecuritySchemes(_securitySchemeName)
                .AddAuthorizationCodeFlow(_securitySchemeName, flow =>
                {
                    flow.ClientId = clientId;
                    flow.SelectedScopes = [$"{audience}/.default"];
                    flow.Pkce = Pkce.Sha256;
                    flow.RedirectUri = redirectUri;
                });
        });

        return app;
    }
}
