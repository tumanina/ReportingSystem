using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReportingSystem.Shared.Interfaces.Authentification;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ReportingSystem.Web.Authentication
{
    internal class JwtTokenAuthorizationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string AuthenticationScheme = "LocalAuth";
        private readonly IAccountService _accountService;
        private readonly IJwtTokenService _tokenService;

        public JwtTokenAuthorizationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IJwtTokenService tokenService,
            IAccountService accountService)
            : base(options, logger, encoder, clock)
        {
            _tokenService = tokenService;
            _accountService = accountService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (AuthenticationHeaderValue.TryParse(Request.Headers["Authorization"], out AuthenticationHeaderValue authHeader))
            {
                try
                {
                    if (string.IsNullOrEmpty(authHeader?.Parameter) || !authHeader.Scheme.Equals("Bearer", StringComparison.OrdinalIgnoreCase))
                    {
                        return AuthenticateResult.Fail("Invalid Authorization Header");
                    }

                    var jwtTokenString = authHeader.Parameter;
                    var token = _tokenService.Read(jwtTokenString);

                    var getUserResult = await _accountService.GetByUsernameAsync(ResolveEmailAsync(token.Claims));

                    var user = await _accountService.GetByUsernameAsync(ResolveEmailAsync(token.Claims));

                    if (user == null)
                    {
                        return AuthenticateResult.Fail("User not found");
                    }

                    var principal = _tokenService.Validate(jwtTokenString);

                    if (principal == null)
                    {
                        return AuthenticateResult.Fail("Token validation failed");
                    }

                    return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
                }
                catch (Exception ex)
                {
                    return AuthenticateResult.Fail(ex.Message);
                }
            }
            return AuthenticateResult.Fail("Missing Authorization Header");
        }

        private string ResolveEmailAsync(IEnumerable<Claim> claims)
        {
            if (claims == null || !claims.Any())
                throw new Exception("No claims found");

            var email = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email || x.Type == JwtRegisteredClaimNames.Email);

            if (email == null)
                throw new Exception("Email claim not found");

            return email.Value;
        }
    }
}
