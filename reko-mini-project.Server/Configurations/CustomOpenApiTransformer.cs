using Microsoft.OpenApi;
using Microsoft.AspNetCore.OpenApi;

namespace reko_mini_project.Server.Configurations;

public class CustomOpenApiTransformer(IConfiguration configuration) : IOpenApiDocumentTransformer
{
    private const string _authorityConfigKey = "AzureAd:Authority";
    private const string _audienceConfigKey = "AzureAd:Audience";
    private const string _title = "Reko Market API";
    private const string _schemaKey = "OAuth2";

    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Info = new OpenApiInfo
        {
            Title = _title
        };

        var authority = configuration[_authorityConfigKey]
            ?? throw new InvalidOperationException($"{_authorityConfigKey} is not configured.");
        var audience = configuration[_audienceConfigKey]
            ?? throw new InvalidOperationException($"{_audienceConfigKey} is not configured.");

        var schemaKey = _schemaKey;
        var scopes = new Dictionary<string, string>
        {
            { $"{audience}/.default", "All scopes" },
        };

        var securitySchemes = new Dictionary<string, IOpenApiSecurityScheme>
        {
            [schemaKey] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Scheme = _schemaKey,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{authority}/oauth2/v2.0/authorize"),
                        TokenUrl = new Uri($"{authority}/oauth2/v2.0/token"),
                        RefreshUrl = new Uri($"{authority}/oauth2/v2.0/token"),
                        Scopes = scopes,
                    },
                },
            },
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = securitySchemes;

        var securityRequirement = new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(schemaKey, document)] = [.. scopes.Keys],
        };
        document.Security = [securityRequirement];

        return Task.CompletedTask;
    }
}