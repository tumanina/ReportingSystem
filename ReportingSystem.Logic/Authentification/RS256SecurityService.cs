using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ReportingSystem.Shared.Configuration;
using ReportingSystem.Shared.Enums;
using ReportingSystem.Shared.Interfaces.Authentification;
using System;
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
    }
}
