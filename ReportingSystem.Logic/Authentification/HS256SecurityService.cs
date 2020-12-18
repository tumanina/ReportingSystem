using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ReportingSystem.Shared.Configuration;
using ReportingSystem.Shared.Enums;
using ReportingSystem.Shared.Interfaces.Authentification;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace ReportingSystem.Logic.Authentification
{
    public class Hs256SecurityService : ISecurityService
    {
        private readonly SecuritySettings _securitySettings;

        public Hs256SecurityService(IOptions<SecuritySettings> securitySettings)
        {
            _securitySettings = securitySettings.Value;
        }

        public SecurityTypeEnum Type { get { return SecurityTypeEnum.HS256; } }

        public string GenerateToken(JwtPayload payload)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_securitySettings.SigningKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var header = new JwtHeader(credentials);
            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            return handler.WriteToken(secToken);
        }

        public SecurityKey GetSecurityKey()
        {
            if (string.IsNullOrEmpty(_securitySettings.SigningKey))
            {
                throw new Exception("Setting SigningKey is null or empty");
            }

            byte[] key = Encoding.ASCII.GetBytes(_securitySettings.SigningKey);
            var hmac = new HMACSHA256(key);

            return new SymmetricSecurityKey(hmac.Key);
        }
    }
}
