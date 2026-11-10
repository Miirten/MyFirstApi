using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace MyFirstApi.Auth
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "ApiKey";
        private const string ApiKeyHeaderName = "X-API-KEY";

        private readonly string _expectedApiKey;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IConfiguration configuration
        )
            : base(options, logger, encoder)
        {
            _expectedApiKey =
                configuration["ApiKeySettings:ApiKey"]
                ?? throw new InvalidOperationException(
                    "API key not configured. Set ApiKeySettings:ApiKey in configuration."
                );
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var providedKey))
            {
                return Task.FromResult(AuthenticateResult.Fail("Missing API Key"));
            }

            if (!string.Equals(providedKey, _expectedApiKey, StringComparison.Ordinal))
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));
            }

            var claims = new[] { new Claim(ClaimTypes.Name, "ApiKeyClient") };

            var identity = new ClaimsIdentity(claims, SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
