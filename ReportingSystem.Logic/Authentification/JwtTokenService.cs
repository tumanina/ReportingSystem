using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ReportingSystem.Shared.Configuration;
using ReportingSystem.Shared.Enums;
using ReportingSystem.Shared.Interfaces.Authentification;
using ReportingSystem.Shared.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace ReportingSystem.Logic.Authentification
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IEnumerable<ISecurityService> _securityServices;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly SecuritySettings _securitySettings;

        public JwtTokenService(IEnumerable<ISecurityService> securityServices, IOptions<SecuritySettings> securitySettings)
        {
            _securityServices = securityServices;
            _tokenHandler = new JwtSecurityTokenHandler();
            _securitySettings = securitySettings.Value;
        }

        public JwtSecurityToken Read(string token)
        {
            try
            {
                var jwtToken = _tokenHandler.ReadJwtToken(token);

                if (string.IsNullOrEmpty(jwtToken.Subject))
                {
                    throw new Exception("Token does not contain subject");
                }

                if (string.IsNullOrEmpty(jwtToken.Audiences.FirstOrDefault()))
                {
                    throw new Exception("Token does not contain audience");
                }

                return jwtToken;
            }
            catch
            {
                throw new Exception("Token does not contain audience");
            }

        }

        public ClaimsPrincipal Validate(string token)
        {
            var result = new ClaimsPrincipal();
            var type = _securitySettings.SecurityType;

            if (!Enum.IsDefined(typeof(SecurityTypeEnum), type))
            {
                throw new Exception("Invalid security type");
            }

            if (string.IsNullOrEmpty(_securitySettings.Audience))
            {
                throw new Exception("Setting Audience is null or empty");
            }

            try
            {
                var securityService = GetSecurityService(_securitySettings.SecurityType);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    RequireSignedTokens = true,
                    IssuerSigningKey = securityService.GetSecurityKey(),
                    ValidAudience = _securitySettings.Audience,
                    ValidIssuer = _securitySettings.Issuer,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    RequireExpirationTime = true,
                    ValidateLifetime = true
                };
                var principal = _tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                if (validatedToken != null)
                {
                    return principal;
                }
            }
            catch (ArgumentException)
            {
                throw new Exception("Token validation failed");
            }
            catch (SecurityTokenExpiredException)
            {
                throw new Exception("Token expired");
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                throw new Exception("Invalid signature");
            }
            catch (SecurityTokenInvalidIssuerException ex)
            {
                throw new Exception($"Invalid issuer: {ex.InvalidIssuer}");
            }
            catch (SecurityTokenInvalidAudienceException ex)
            {
                throw new Exception($"Invalid audience: {ex.InvalidAudience}");
            }

            return result;
        }

        private ISecurityService GetSecurityService(SecurityTypeEnum type)
        {
            var securityService = _securityServices.FirstOrDefault(s => s.Type == type);
            if (securityService == null)
            {
                throw new Exception($"Service for type '{type}' not found");
            }

            return securityService;
        }
    }
}
