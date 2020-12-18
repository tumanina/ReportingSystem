using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReportingSystem.Logic.Authentification;
using ReportingSystem.Shared.Configuration;
using ReportingSystem.Shared.Enums;
using ReportingSystem.Shared.Interfaces.Authentification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ReportingSystem.Tests.Authentication
{
    [TestClass]
    public class JwtTokenServiceTests
    {
        private const string _email = "test@test.com";
        private const string _audience = "local.auth.audience";
        private const string _issuer = "local.auth.issuer";
        private const string _signingKey = "***";
        private const string _certificateData = "***";

        [TestMethod]
        public void GenerateRS256Token_Success()
        {
            var mockSettings = new Mock<IOptions<SecuritySettings>>();
            mockSettings.Setup(m => m.Value).Returns(() => GetRS256TestSettings());

            var service = new JwtTokenService(new List<ISecurityService> { new Rs256SecurityService(mockSettings.Object) }, mockSettings.Object);
            var result = service.GenerateToken(_email);

            var token = result;
            Assert.IsNotNull(token);

            var principal = service.Validate(token);
            Assert.IsNotNull(principal);
            var identity = principal.Identity;
            Assert.IsTrue(identity.IsAuthenticated);
            var claims = principal.Claims;
            Assert.IsTrue(claims.Any(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" && c.Value == _email));
            Assert.IsTrue(claims.Any(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress" && c.Value == _email));
            Assert.IsTrue(claims.Any(c => c.Type == "aud" && c.Value == _audience));
            Assert.IsTrue(claims.Any(c => c.Type == "iss" && c.Value == _issuer));
        }

        [TestMethod]
        public void GenerateHS256Token_Success()
        {
            var mockSettings = new Mock<IOptions<SecuritySettings>>();
            mockSettings.Setup(m => m.Value).Returns(() => GetHS256TestSettings());

            var service = new JwtTokenService(new List<ISecurityService> { new Hs256SecurityService(mockSettings.Object) }, mockSettings.Object);
            var result = service.GenerateToken(_email);

            var token = result;
            Assert.IsNotNull(token);

            var principal = service.Validate(token);
            Assert.IsNotNull(principal);
            var identity = principal.Identity;
            Assert.IsTrue(identity.IsAuthenticated);
            var claims = principal.Claims;
            Assert.IsTrue(claims.Any(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" && c.Value == _email));
            Assert.IsTrue(claims.Any(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress" && c.Value == _email));
            Assert.IsTrue(claims.Any(c => c.Type == "aud" && c.Value == _audience));
            Assert.IsTrue(claims.Any(c => c.Type == "iss" && c.Value == _issuer));
        }

        [TestMethod]
        public void Read_Success()
        {
            var tokenString = GenerateRS256Token();

            var mockSettings = new Mock<IOptions<SecuritySettings>>();
            mockSettings.Setup(m => m.Value).Returns(() => new SecuritySettings());

            var service = new JwtTokenService(new List<ISecurityService>(), mockSettings.Object);
            var result = service.Read(tokenString);

            var token = result;
            Assert.IsNotNull(token);
            Assert.AreEqual(token.SignatureAlgorithm, "RS256");
            Assert.AreEqual(token.Audiences.FirstOrDefault(), _audience);
            Assert.AreEqual(token.Issuer, _issuer);
            Assert.AreEqual(token.Subject, _email);
            Assert.IsTrue(token.Claims.Any(c => c.Type == "email" && c.Value == _email));
        }

        [TestMethod]
        public void Read_TokenWithoutAudience_ReturnsError()
        {
            var settings = GetRS256TestSettings();
            settings.Audience = null;
            var tokenString = GenerateRS256Token(settings);

            var mockSettings = new Mock<IOptions<SecuritySettings>>();
            mockSettings.Setup(m => m.Value).Returns(() => new SecuritySettings());

            try
            {
                var service = new JwtTokenService(new List<ISecurityService> { new Rs256SecurityService(mockSettings.Object) }, mockSettings.Object);
                var result = service.Read(tokenString);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Token does not contain audience");
            }
        }

        [TestMethod]
        public void ValidateRS256Token_Success()
        {
            var tokenString = GenerateRS256Token();

            var mockSettings = new Mock<IOptions<SecuritySettings>>();
            mockSettings.Setup(m => m.Value).Returns(() => GetRS256TestSettings());

            var service = new JwtTokenService(new List<ISecurityService> { new Rs256SecurityService(mockSettings.Object) }, mockSettings.Object);
            var result = service.Validate(tokenString);

            var principal = result;
            Assert.IsNotNull(principal);
            var identity = principal.Identity;
            Assert.IsTrue(identity.IsAuthenticated);
            var claims = principal.Claims;
            Assert.IsTrue(claims.Any(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" && c.Value == _email));
            Assert.IsTrue(claims.Any(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress" && c.Value == _email));
            Assert.IsTrue(claims.Any(c => c.Type == "aud" && c.Value == _audience));
            Assert.IsTrue(claims.Any(c => c.Type == "iss" && c.Value == _issuer));
        }

        [TestMethod]
        public void ValidateToken_InvalidSecurityType_ReturnsError()
        {
            var settings = new SecuritySettings
            {
                Audience = _audience,
                Issuer = _issuer,
                SigningKey = _signingKey
            };
            var mockSettings = new Mock<IOptions<SecuritySettings>>();
            mockSettings.Setup(m => m.Value).Returns(() => settings);

            var tokenString = GenerateRS256Token();

            try
            {
                var service = new JwtTokenService(new List<ISecurityService> { new Rs256SecurityService(mockSettings.Object) }, mockSettings.Object);
                var result = service.Validate(tokenString);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Invalid security type");
            }
        }

        [TestMethod]
        public void ValidateRS256Token_InvalidSignature_ReturnsError()
        {
            var settings = GetRS256TestSettings();
            settings.SigningKey = "123";

            var tokenString = GenerateHS256Token();

            var mockSettings = new Mock<IOptions<SecuritySettings>>();
            mockSettings.Setup(m => m.Value).Returns(() => GetRS256TestSettings());

            try
            {
                var service = new JwtTokenService(new List<ISecurityService> { new Rs256SecurityService(mockSettings.Object) }, mockSettings.Object);
                var result = service.Validate(tokenString);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Invalid signature");
            }
        }

        [TestMethod]
        public void ValidateRS256Token_IncorrectAudience_ReturnsError()
        {
            var newAudience = "local.auth.audience1";
            var settings = GetRS256TestSettings();
            settings.Audience = "local.auth.audience1";
            var tokenString = GenerateRS256Token(settings);

            var mockSettings = new Mock<IOptions<SecuritySettings>>();
            mockSettings.Setup(m => m.Value).Returns(() => GetRS256TestSettings());

            try
            {
                var service = new JwtTokenService(new List<ISecurityService> { new Rs256SecurityService(mockSettings.Object) }, mockSettings.Object);
                var result = service.Validate(tokenString);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, $"Invalid audience: {newAudience}");
            }
        }

        [TestMethod]
        public void ValidateRS256Token_IncorrectIssuer_ReturnsError()
        {
            var newIssuer = "local.auth.issuer1";
            var settings = GetRS256TestSettings();
            settings.Issuer = newIssuer;
            var tokenString = GenerateRS256Token(settings);

            var mockSettings = new Mock<IOptions<SecuritySettings>>();
            mockSettings.Setup(m => m.Value).Returns(() => GetRS256TestSettings());

            try
            {
                var service = new JwtTokenService(new List<ISecurityService> { new Rs256SecurityService(mockSettings.Object) }, mockSettings.Object);
                var result = service.Validate(tokenString);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, $"Invalid issuer: {newIssuer}");
            }
        }

        [TestMethod]
        public void ValidateRS256Token_CertificateWithoutPrivateKey_ReturnsError()
        {
            var settings = GetRS256TestSettings();
            settings.CertificateData = "***";

            var mockSettings = new Mock<IOptions<SecuritySettings>>();
            mockSettings.Setup(m => m.Value).Returns(() => settings);

            try
            {
                var tokenString = GenerateRS256Token();
                var service = new JwtTokenService(new List<ISecurityService> { new Rs256SecurityService(mockSettings.Object) }, mockSettings.Object);
                var result = service.Validate(tokenString);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Certificate is not X509Certificate2 or does not have private key");
            }
        }

        [TestMethod]
        public void ValidateHS256Token_Success()
        {
            var tokenString = GenerateHS256Token();

            var mockSettings = new Mock<IOptions<SecuritySettings>>();
            mockSettings.Setup(m => m.Value).Returns(() => GetHS256TestSettings());

            var service = new JwtTokenService(new List<ISecurityService> { new Hs256SecurityService(mockSettings.Object) }, mockSettings.Object);
            var result = service.Validate(tokenString);

            Assert.IsNotNull(result);
            var principal = result;
            Assert.IsNotNull(principal);
            var identity = principal.Identity;
            Assert.IsTrue(identity.IsAuthenticated);
            var claims = principal.Claims;
            Assert.IsTrue(claims.Any(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" && c.Value == _email));
            Assert.IsTrue(claims.Any(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress" && c.Value == _email));
            Assert.IsTrue(claims.Any(c => c.Type == "aud" && c.Value == _audience));
            Assert.IsTrue(claims.Any(c => c.Type == "iss" && c.Value == _issuer));
        }

        [TestMethod]
        public void ValidateHS256Token_InvalidSigninature_ReturnsError()
        {
            var settings = GetHS256TestSettings();
            settings.SigningKey = "123";

            var mockSettings = new Mock<IOptions<SecuritySettings>>();
            mockSettings.Setup(m => m.Value).Returns(() => settings);

            try
            {
                var tokenString = GenerateHS256Token();
                var service = new JwtTokenService(new List<ISecurityService> { new Hs256SecurityService(mockSettings.Object) }, mockSettings.Object);
                var result = service.Validate(tokenString);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Invalid signature");
            }
        }

        [TestMethod]
        public void ValidateHS256Token_IncorrectAudience_ReturnsError()
        {
            var settings = GetHS256TestSettings();
            settings.Audience = "local.auth.audience1";

            var tokenString = GenerateHS256Token();

            var mockSettings = new Mock<IOptions<SecuritySettings>>();
            mockSettings.Setup(m => m.Value).Returns(() => settings);

            try
            {
                var service = new JwtTokenService(new List<ISecurityService> { new Hs256SecurityService(mockSettings.Object) }, mockSettings.Object);
                var result = service.Validate(tokenString);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Invalid audience: local.auth.audience");
            }
        }

        [TestMethod]
        public void ValidateHS256Token_IncorrectIssuer_ReturnsError()
        {
            var settings = GetHS256TestSettings();
            settings.Issuer = "local.auth.issuer1";

            var tokenString = GenerateHS256Token();

            var mockSettings = new Mock<IOptions<SecuritySettings>>();
            mockSettings.Setup(m => m.Value).Returns(() => settings);

            try
            {
                var service = new JwtTokenService(new List<ISecurityService> { new Hs256SecurityService(mockSettings.Object) }, mockSettings.Object);
                var result = service.Validate(tokenString);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Invalid issuer: local.auth.issuer");
            }
        }

        [TestMethod]
        public void ValidateHS256Token_NoSecurityService_ReturnsError()
        {
            var tokenString = GenerateHS256Token();

            var mockSettings = new Mock<IOptions<SecuritySettings>>();
            mockSettings.Setup(m => m.Value).Returns(() => GetHS256TestSettings());

            try
            {
                var service = new JwtTokenService(new List<ISecurityService>(), mockSettings.Object);
                var result = service.Validate(tokenString);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Service for type 'HS256' not found");
            }
        }

        private string GenerateRS256Token(SecuritySettings settings = null)
        {
            var mockSettings = new Mock<IOptions<SecuritySettings>>();
            mockSettings.Setup(m => m.Value).Returns(() => settings ?? GetRS256TestSettings());

            var service = new JwtTokenService(new List<ISecurityService> { new Rs256SecurityService(mockSettings.Object) }, mockSettings.Object);
            return service.GenerateToken(_email);
        }

        private string GenerateHS256Token()
        {
            var mockSettings = new Mock<IOptions<SecuritySettings>>();
            mockSettings.Setup(m => m.Value).Returns(() => GetHS256TestSettings());

            var service = new JwtTokenService(new List<ISecurityService> { new Hs256SecurityService(mockSettings.Object) }, mockSettings.Object);
            return service.GenerateToken(_email);
        }

        private SecuritySettings GetRS256TestSettings(string audience = null, string issue = null, string certificateData = null)
        {
            return new SecuritySettings
            {
                SecurityType = SecurityTypeEnum.RS256,
                Audience = audience ?? _audience,
                Issuer = issue ?? _issuer,
                TokenExpiration = new TimeSpan(1, 0, 0),
                CertificateData = certificateData ?? _certificateData
            };
        }

        private SecuritySettings GetHS256TestSettings(string audience = null, string issue = null)
        {
            return new SecuritySettings
            {
                SecurityType = SecurityTypeEnum.HS256,
                Audience = audience ?? _audience,
                TokenExpiration = new TimeSpan(1, 0, 0),
                Issuer = issue ?? _issuer,
                SigningKey = _signingKey
            };
        }
    }
}
