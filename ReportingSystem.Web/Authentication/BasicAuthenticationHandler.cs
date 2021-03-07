using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReportingSystem.Shared.Interfaces.Authentification;
using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ReportingSystem.Web.Authentication
{
    internal class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string AuthenticationScheme = "Basic";
        private readonly IAccountService _accountService;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IAccountService accountService)
            : base(options, logger, encoder, clock)
        {
            _accountService = accountService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (AuthenticationHeaderValue.TryParse(Request.Headers["Authorization"], out AuthenticationHeaderValue authHeader))
            {
                try
                {
                    if (string.IsNullOrEmpty(authHeader?.Parameter) || !authHeader.Scheme.Equals("Basic", StringComparison.OrdinalIgnoreCase))
                    {
                        return AuthenticateResult.Fail("Invalid Authorization Header");
                    }

                    var credentials = Encoding.ASCII.GetString(Convert.FromBase64String(authHeader.Parameter)).Split(':');
                    if (credentials.Length != 2)
                    {
                        return AuthenticateResult.Fail("Invalid value in Authorization heaer");
                    }

                    var account = await _accountService.GetAccountAsync(credentials[0], credentials[1]);

                    if (account == null)
                    {
                        return AuthenticateResult.Fail("User not found");
                    }

                    var claims = new[] 
                    {
                        new Claim(ClaimTypes.Email, account.Username),
                    };
                    var identity = new ClaimsIdentity(claims, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);

                    return AuthenticateResult.Success(ticket);
                }
                catch (Exception ex)
                {
                    return AuthenticateResult.Fail(ex.Message);
                }
            }
            return AuthenticateResult.Fail("Missing Authorization Header");
        }
    }
}
