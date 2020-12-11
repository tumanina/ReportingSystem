using ReportingSystem.Shared.Enums;

namespace ReportingSystem.Shared.Configuration
{
    public class SecuritySettings
    {
        public string CertificateData { get; set; }
        public string CertificatePassword { get; set; }
        public string SigningKey { get; set; }
        public SecurityTypeEnum SecurityType { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
    }
}
