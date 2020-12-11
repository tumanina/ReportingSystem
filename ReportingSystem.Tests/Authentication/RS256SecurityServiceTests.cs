using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReportingSystem.Logic.Authentification;
using ReportingSystem.Shared.Configuration;
using ReportingSystem.Shared.Enums;
using System;

namespace ReportingSystem.Tests.Authentication
{
    [TestClass]
    public class RS256SecurityServiceTests
    {
        [TestMethod]
        public void GetSecurityKey_Success()
        {
            var settings = new SecuritySettings
            {
                SecurityType = SecurityTypeEnum.RS256,
                Audience = "local.auth.audience",
                Issuer = "local.auth.issuer",
                CertificateData = "***"
            };
            var mockSettings = new Mock<IOptions<SecuritySettings>>();
            mockSettings.Setup(m => m.Value).Returns(() => settings);
            var service = new Rs256SecurityService(mockSettings.Object);
            var result = service.GetSecurityKey();

            Assert.IsTrue(result != null);
        }

        [TestMethod]
        public void GetSecurityKey_CertificateDataIsEmpty_Success()
        {
            var settings = new SecuritySettings
            {
                SecurityType = SecurityTypeEnum.RS256,
                Audience = "local.auth.audience",
                Issuer = "local.auth.issuer"
            };
            var mockSettings = new Mock<IOptions<SecuritySettings>>();
            mockSettings.Setup(m => m.Value).Returns(() => settings);
            var service = new Rs256SecurityService(mockSettings.Object);

            try
            {
                var result = service.GetSecurityKey();
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("Setting CertificateData is null or empty"));
            }
        }

        [TestMethod]
        public void GetSecurityKey_CertificateWithoutPrivateKey_ReturnsError()
        {
            var settings = new SecuritySettings
            {
                SecurityType = SecurityTypeEnum.RS256,
                Audience = "local.auth.audience",
                Issuer = "local.auth.issuer",
                CertificateData = "***"
            };
            var mockSettings = new Mock<IOptions<SecuritySettings>>();
            mockSettings.Setup(m => m.Value).Returns(() => settings);
            var service = new Rs256SecurityService(mockSettings.Object);

            try
            {
                var result = service.GetSecurityKey();
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("Certificate is not X509Certificate2 or does not have private key"));
            }
        }
    }
}
