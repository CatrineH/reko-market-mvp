using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace reko_mini_project.Tests;

/// <summary>
/// Fake authentication handler for integration tests.
/// Bypasses real JWT validation by authenticating every request unconditionally.
///
/// Usage:
///   - Default identity: fixed oid + Supplier role — no setup required.
///   - Override per-test via ClaimsOverride before the request; reset to null in teardown.
///
/// Example:
///   TestAuthHandler.ClaimsOverride = [new Claim(ClaimTypes.Role, "Admin")];
///   try { ... await _client.PostAsync(...); ... }
///   finally { TestAuthHandler.ClaimsOverride = null; }
/// </summary>
public sealed class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "TestAuth";

    /// <summary>Fixed oid used by default. Tests that check OwnerId should reference this constant.</summary>
    public const string DefaultOwnerId = "00000000-test-0000-0000-supplier00001";

    /// <summary>
    /// Set before a request to override the default claims. Must be reset to null after the test.
    /// </summary>
    public static IEnumerable<Claim>? ClaimsOverride { get; set; }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = ClaimsOverride ?? DefaultClaims();
        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private static IEnumerable<Claim> DefaultClaims() =>
    [
        new Claim("oid", DefaultOwnerId),
        new Claim(ClaimTypes.Role, "Admin"),
    ];
}
