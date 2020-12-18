using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ReportingSystem.Shared.Configuration;
using ReportingSystem.Shared.Enums;
using ReportingSystem.Shared.Interfaces.Authentification;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace ReportingSystem.Logic.Authentification
{
    public class Rs256SecurityService : ISecurityService
    {
        private readonly SecuritySettings _securitySettings;

        public Rs256SecurityService(IOptions<SecuritySettings> securitySettings)
        {
            _securitySettings = securitySettings.Value;
        }

        public SecurityTypeEnum Type { get { return SecurityTypeEnum.RS256; } }

        public string GenerateToken(JwtPayload payload)
        {
            var token = new JwtSecurityToken(new JwtHeader(GetSigningCredentials()), payload);

            var tokenHandler = new JwtSecurityTokenHandler();
            var encodedJwt = tokenHandler.WriteToken(token);

            return encodedJwt;
        }

        public SecurityKey GetSecurityKey()
        {
            if (string.IsNullOrEmpty(_securitySettings.CertificateData))
            {
                throw new Exception("Setting CertificateData is null or empty");
            }

            var rawData = Convert.FromBase64String(_securitySettings.CertificateData);
            var certificate = new X509Certificate2(rawData, _securitySettings.CertificatePassword, X509KeyStorageFlags.Exportable);

            if (certificate == null || !certificate.HasPrivateKey)
            {
                throw new Exception("Certificate is not X509Certificate2 or does not have private key");
            }

            if (certificate.PrivateKey is RSACryptoServiceProvider rsaProvider)
            {
                return new RsaSecurityKey(rsaProvider.ExportParameters(true));
            }
            if (certificate.PrivateKey is RSA)
            {
                return new RsaSecurityKey(certificate.GetRSAPrivateKey());
            }

            return null;
        }


        private SigningCredentials GetSigningCredentials()
        {
            var securityKey = GetSecurityKey();

            if (securityKey == null)
            {
                return null;
            }

            return new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256, SecurityAlgorithms.Sha256Digest);
        }
    }
}
