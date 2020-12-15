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

namespace ReportingSystem.Tests.Authentication
{
    [TestClass]
    public class JwtTokenServiceTests
    {
        [TestMethod]
        public void GenerateToken_Success()
        {
            var tokenExpiration = new TimeSpan(1, 0, 0);
            var settings = new SecuritySettings
            {
                SecurityType = SecurityTypeEnum.RS256,
                Audience = "local.auth.audience",
                Issuer = "local.auth.issuer",
                TokenExpiration = tokenExpiration,
                CertificateData = "***"
            };
            var mockSettings = new Mock<IOptions<SecuritySettings>>();
            mockSettings.Setup(m => m.Value).Returns(() => settings);

            var email = "test@test.com";
            var service = new JwtTokenService(new List<ISecurityService> { new Rs256SecurityService(mockSettings.Object) }, mockSettings.Object);
            var result = service.GenerateToken(email);

            var token = result;
            Assert.IsNotNull(token);

            var jwtToken = service.Read(token);
            Assert.IsNotNull(jwtToken);
            Assert.AreEqual(jwtToken.Audiences.FirstOrDefault(), settings.Audience);
            Assert.AreEqual(jwtToken.Issuer, settings.Issuer);
            Assert.AreEqual(jwtToken.Subject, email);
            Assert.AreEqual(jwtToken.ValidFrom.AddSeconds(tokenExpiration.TotalSeconds), jwtToken.ValidTo);
            Assert.IsTrue(jwtToken.Claims.Any(c => c.Type == "email" && c.Value == email));
        }

        [TestMethod]
        public void Read_Success()
        {
            var tokenString = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJleGFtcGxlQHRlc3QuY29tIiwiZW1haWwiOiJleGFtcGxlQHRlc3QuY29tIiwiaWF0IjoiMTU4Njg2MTk4OSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJleGFtcGxlQHRlc3QuY29tIiwibmJmIjoiMTU4Njg2MTk4OCIsImV4cCI6IjE1ODc4NzgxNTQiLCJpc3MiOiJsb2NhbC5hdXRoLmlzc3VlciIsImF1ZCI6ImxvY2FsLmF1dGguYXVkaWVuY2UiLCJqdGkiOiI0NWFiYjZkNC04ZjE2LTRjNDMtOTczNy1kOWYxMzc2OGM5OGIifQ.gddEru1-e2EzTD18Yloy9l_YuBgg-gSgyYKKrMoLe0k";

            var mockSettings = new Mock<IOptions<SecuritySettings>>();
            mockSettings.Setup(m => m.Value).Returns(() => new SecuritySettings());

            var service = new JwtTokenService(new List<ISecurityService> { new Rs256SecurityService(mockSettings.Object) }, mockSettings.Object);
            var result = service.Read(tokenString);

            var token = result;
            Assert.IsNotNull(token);
            Assert.AreEqual(token.SignatureAlgorithm, "HS256");
            Assert.AreEqual(token.Audiences.FirstOrDefault(), "local.auth.audience");
            Assert.AreEqual(token.Issuer, "local.auth.issuer");
            Assert.AreEqual(token.Subject, "example@test.com");
            Assert.IsTrue(token.Claims.Any(c => c.Type == "email" && c.Value == "example@test.com"));
        }

        [TestMethod]
        public void Read_TokenWithoutAudience_ReturnsError()
        {
            var tokenString = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJleGFtcGxlQHRlc3QuY29tIiwiZW1haWwiOiJleGFtcGxlQHRlc3QuY29tIiwiaWF0IjoiMTU4Njg2MTk4OSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJleGFtcGxlQHRlc3QuY29tIiwibmJmIjoiMTU4Njg2MTk4OCIsImV4cCI6IjE1ODc4NzgxNTQiLCJpc3MiOiJsb2NhbC5hdXRoLmlzc3VlciIsImp0aSI6IjQ1YWJiNmQ0LThmMTYtNGM0My05NzM3LWQ5ZjEzNzY4Yzk4YiJ9.GZ5UQ2tqlnTLiNyRX6SL_cymVgtAtpS5xTpdPDQr7DY";

            var mockSettings = new Mock<IOptions<SecuritySettings>>();
            mockSettings.Setup(m => m.Value).Returns(() => new SecuritySettings());

            try
            {
                var service = new JwtTokenService(new List<ISecurityService> { new Rs256SecurityService(mockSettings.Object) }, mockSettings.Object);
                var result = service.Read(tokenString);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Token does not contain audience");
            }
        }

        [TestMethod]
        public void ValidateToken_InvalidSecurityType_ReturnsError()
        {
            var settings = new SecuritySettings
            {
                Audience = "local.auth.audience",
                Issuer = "local.auth.issuer",
                SigningKey = "A3mbW9R4wTCHLpGRzcO2dKnESvkQz8B0W4QiTFijx6z+gFKuvl6XCI2y+rvKOLWk7R29TJc1+ZevxAP3KR/bCu2bFmYx+wOwWowZ1GZO+EI="
            };
            var mockSettings = new Mock<IOptions<SecuritySettings>>();
            mockSettings.Setup(m => m.Value).Returns(() => settings);

            var tokenString = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJleGFtcGxlQHRlc3QuY29tIiwiZW1haWwiOiJleGFtcGxlQHRlc3QuY29tIiwiaWF0IjoiMTU4Njg2MTk4OSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJleGFtcGxlQHRlc3QuY29tIiwibmJmIjoiMTU4Njg2MTk4OCIsImV4cCI6IjE1ODc4NzgxNTQiLCJpc3MiOiJsb2NhbC5hdXRoLmlzc3VlciIsImF1ZCI6ImxvY2FsLmF1dGguYXVkaWVuY2UiLCJqdGkiOiI0NWFiYjZkNC04ZjE2LTRjNDMtOTczNy1kOWYxMzc2OGM5OGIifQ.gddEru1-e2EzTD18Yloy9l_YuBgg-gSgyYKKrMoLe0k";

            try
            {
                var service = new JwtTokenService(new List<ISecurityService> { new Rs256SecurityService(mockSettings.Object) }, mockSettings.Object);
                var result = service.Validate(tokenString);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Invalid security type");
            }
        }

        [TestMethod]
        public void ValidateRS256Token_Success()
        {
            var settings = new SecuritySettings
            {
                SecurityType = SecurityTypeEnum.RS256,
                Audience = "local.auth.audience",
                Issuer = "local.auth.issuer",
                CertificateData = "***"
            };
            var tokenString = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJleGFtcGxlQHRlc3QuY29tIiwiZW1haWwiOiJleGFtcGxlQHRlc3QuY29tIiwiaWF0IjoiMTU4Njg2MTk4OSIsIm5iZiI6IjE1ODY4NjE5ODgiLCJleHAiOiIxNTg3ODc4MTU0IiwiaXNzIjoibG9jYWwuYXV0aC5pc3N1ZXIiLCJhdWQiOiJsb2NhbC5hdXRoLmF1ZGllbmNlIn0.pqYLyAEHTrRRH7ERD6OQGupnrOsLAI6turOk7pe17F0yCNRCxgkE5umK3mPrlxeiiaijHWbzOiy8kG3MydfaiJYMhqyep8tuwW2_TNtnjbnkzPSYP-g4K_mo8XpwiOQW_s-JJWQuX1aq6NoUKJyftvNCpDure24KnadlhrW09JI9DGBWflYrguyC_7cfZRXPv-kJ5a5uoItRQWGmwWqRBr4evV0n92MOxxjGQ3a0yFM1cu-LD3O0_ZtPO7q3PENfT4wMQxVmmWQxqOuERtnDS9E9wwIrQe9_XnCZTUNXfOlud8PA_5MBN9Esf3Rooveg7tYOfWq9GGs3yibKlaTx58-4D3p7-HrR8HZ2y5qrCD2jFdPPtJ8VBFPix0o6fPe7YTu_6L0uA3LDYLJZyHKtsJytGWMJEbfVYkTSbTnvzEWtIdBJkj8CRUawDU4CUTQ7hDXAllk3GO0RqijpX3SF-VT7CKYHZyAFb-717UAkM3ZWyEDq3x66YiIw6ZyAEL4T5ryTESmMkR-N1IyQgMTtc1m9syGlgtyspZNSms2mF95O9W7m-oWw7XlnseiVHW-YLC4H-SFkBN-OiTMYSK-1HbNnRbQjg6Y53kpkQEkElO7V5vmtf2K7uIF8ajTTBEfY2_nfF3rtJHYuRyK17vpx6BcFQRFu0MuSRHrKI6vdRoU";

            var mockSettings = new Mock<IOptions<SecuritySettings>>();
            mockSettings.Setup(m => m.Value).Returns(() => settings);

            var service = new JwtTokenService(new List<ISecurityService> { new Rs256SecurityService(mockSettings.Object) }, mockSettings.Object);
            var result = service.Validate(tokenString);

            var principal = result;
            Assert.IsNotNull(principal);
            var identity = principal.Identity;
            Assert.IsTrue(identity.IsAuthenticated);
            var claims = principal.Claims;
            Assert.IsTrue(claims.Any(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" && c.Value == "example@test.com"));
            Assert.IsTrue(claims.Any(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress" && c.Value == "example@test.com"));
            Assert.IsTrue(claims.Any(c => c.Type == "aud" && c.Value == "local.auth.audience"));
            Assert.IsTrue(claims.Any(c => c.Type == "iss" && c.Value == "local.auth.issuer"));
        }


        [TestMethod]
        public void ValidateRS256Token_InvalidSignature_ReturnsError()
        {
            var settings = new SecuritySettings
            {
                SecurityType = SecurityTypeEnum.RS256,
                Audience = "local.auth.audience",
                Issuer = "local.auth.issuer",
                CertificateData = "***"
            };
            var tokenString = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJlbWFpbCI6ImV4YW1wbGVAdGVzdC5jb20iLCJpYXQiOiIxNTg2ODYxOTg5IiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZSI6IiIsIm5iZiI6IjE1ODY4NjE5ODgiLCJleHAiOiIxNTg3ODc4MTU0IiwiaXNzIjoibG9jYWwuYXV0aC5pc3N1ZXIiLCJhdWQiOiJsb2NhbC5hdXRoLmF1ZGllbmNlIiwianRpIjoiNDVhYmI2ZDQtOGYxNi00YzQzLTk3MzctZDlmMTM3NjhjOThiIn0.u8rHV4OzCfpm8pwiB_XWta1kR6ok3GYiSF1aZ24y6vI";

            var mockSettings = new Mock<IOptions<SecuritySettings>>();
            mockSettings.Setup(m => m.Value).Returns(() => settings);

            try
            {
                var service = new JwtTokenService(new List<ISecurityService> { new Rs256SecurityService(mockSettings.Object) }, mockSettings.Object);
                var result = service.Validate(tokenString);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Invalid signature");
            }
        }

        [TestMethod]
        public void ValidateRS256Token_ExpiredToken_ReturnsError()
        {
            var settings = new SecuritySettings
            {
                SecurityType = SecurityTypeEnum.RS256,
                Audience = "local.auth.audience",
                Issuer = "local.auth.issuer",
                CertificateData = "***"
            };
            var tokenString = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJleGFtcGxlQHRlc3QuY29tIiwiZW1haWwiOiJleGFtcGxlQHRlc3QuY29tIiwiaWF0IjoiMTU4Njg2MTk4OSIsIm5iZiI6IjE1ODY4NjE5ODgiLCJleHAiOiIxNTg2ODYxOTg5IiwiaXNzIjoibG9jYWwuYXV0aC5pc3N1ZXIiLCJhdWQiOiJsb2NhbC5hdXRoLmF1ZGllbmNlIn0.r2nteqXmP1PDCmDIsJ77fL3HI0-V3zdODqSS7rNIr4MHoSzcJDWviRwACgLOrBKcxShezL2QjseiNnRiZW6BH053jiqXkNp13qzgtUIL8_ldXk4RqHT2HyaoV01MK8vpPECcxbgkWe_mSykAOaCemwhixTqdKS_r4gObuPoBs_T8zA1Y_5cIt63zg14hSZnFM8EeAd834eZYGx9bsUF4l7fmAsoWTHn15HrH-N6BxPcypxwUKG0ZWAEJzhX2tZt_NEfi4qCvEqwkpdwyz6-4Qwon_EqHM0gkM97l34PdSYewW9wlq0gNKDc9IgjrFbGEGnrArO2rpf3gK_SG2mbmqz6an3Bdcf79QD0txwaYMFmJ4hkm2_71qlyJ94-lTV-Va-vKomzqiAJOqCLCJr8ltjYy2sbQmvQt6BoCTaaL5qW-b9Q51yKFrXT69511VJicGYZCHeWc_eMVlary00gyqpWncxmCw6aFXq8pCtwIj0GVTSnMcG-MKY5KR9c19fs3XGhRIEAmQ-PxDkeW8qWIId9t6NO68F70lULCEj0KbGT12z0REoFQVYfSlu8qtlhbCxJH544kxTSL1-VzNR67xg4oAvIPnhMeMs150AeMC2JnRn6nFQ5RG3qdqc4JIsuoFdkiEKgVl86nqPUWuOvD4hi4SGuJXk97srBnF62J1oY";

            var mockSettings = new Mock<IOptions<SecuritySettings>>();
            mockSettings.Setup(m => m.Value).Returns(() => settings);

            try
            {
                var service = new JwtTokenService(new List<ISecurityService> { new Rs256SecurityService(mockSettings.Object) }, mockSettings.Object);
                var result = service.Validate(tokenString);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Token expired");
            }
        }

        [TestMethod]
        public void ValidateRS256Token_IncorrectAudience_ReturnsError()
        {
            var settings = new SecuritySettings
            {
                SecurityType = SecurityTypeEnum.RS256,
                Audience = "local.auth.audience",
                Issuer = "local.auth.issuer",
                CertificateData = "***"
            };
            var tokenString = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJleGFtcGxlQHRlc3QuY29tIiwiZW1haWwiOiJleGFtcGxlQHRlc3QuY29tIiwiaWF0IjoiMTU4Njg2MTk4OSIsIm5iZiI6IjE1ODY4NjE5ODgiLCJleHAiOiIxNTg3ODc4MTU0IiwiaXNzIjoibG9jYWwuYXV0aC5pc3N1ZXIiLCJhdWQiOiJsb2NhbC5hdXRoLmF1ZGllbmNlMSJ9.K6jIQ_4ovGqbp-mNr6c7oS5sYNz1ttsm4JEjSbRsKrE_7yE6x3vDW3WNiodwumhPE-w5pnCUspDtfmXLdnwklLaUAB-hXlcwOfGpdqBTxSpRC4K6lmJ6hXsN6DhZBB7LVPvsKJWQ73Pkas8c13ylPkFJVMTjuQQRiJyilNzzXgubZoo1zr7LowBqfPRGSRc3jXhIOWR883DUPaTI827x8ZVww7t8aD4Gj6wYdmf8t5mSEN9GYbiR_gDsN6gF6ii9qOgpGAn7G6I5pAgdnWOvj-G4HZ5S8DADkrN-_WYoenpEyRZtEGb13fxZCIkPdr5XzsJ9p6dXeSODBcdlKJqtxZ6j12xDOHBnrfEaoFqPHyHxTGF5TIj71o9mbUSbXnrtnE3RLuuD61iy6M6pBbkfqjxzIjW9BvlZr1FNwMmhheQCEHbTE4CqK83EhT8_lmHCFl-6gz6IhfzLwEOgovpP32zek-3tvlTN6R6Kt9UwECJv2lLUiTdZn01qnEU0Srmg6GH1X-CJvoRh2n0ZU3SLqrHRUZoLNhZ4Zn8ftciW9WW8xGizi_vlRmeWb0JFvZyPzN6b22tyq_YPuhWchMtSkidcw7BpivDNX5lKmdnjb9c4lIZd4rtVmGACASLxhv56p96umIyWzQk5OzD0NYR18xgU7QPMmULGWpxfQpIg1dk";

            var mockSettings = new Mock<IOptions<SecuritySettings>>();
            mockSettings.Setup(m => m.Value).Returns(() => settings);

            try
            {
                var service = new JwtTokenService(new List<ISecurityService> { new Rs256SecurityService(mockSettings.Object) }, mockSettings.Object);
                var result = service.Validate(tokenString);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Invalid audience: local.auth.audience1");
            }
        }

        [TestMethod]
        public void ValidateRS256Token_IncorrectIssuer_ReturnsError()
        {
            var settings = new SecuritySettings
            {
                SecurityType = SecurityTypeEnum.RS256,
                Audience = "local.auth.audience",
                Issuer = "local.auth.issuer",
                CertificateData = "***"
            };
            var tokenString = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJleGFtcGxlQHRlc3QuY29tIiwiZW1haWwiOiJleGFtcGxlQHRlc3QuY29tIiwiaWF0IjoiMTU4Njg2MTk4OSIsIm5iZiI6IjE1ODY4NjE5ODgiLCJleHAiOiIxNTg3ODc4MTU0IiwiaXNzIjoibG9jYWwuYXV0aC5pc3N1ZXIxIiwiYXVkIjoibG9jYWwuYXV0aC5hdWRpZW5jZSJ9.kdygs3zA8OqdjSqIjYVEbl65WNDRjkTDmz3CmO4UjjdZEYVcyFIvV8synevjAny6v_sb1QlQ8k-rbQ55sijp6vuQzRnDkrzCvTFR-zatRpnaiLAJsJdvFS7fYuUBDOfhvTfnKgxSuUKO1BiKYP8ZU7CNv5OApKuDJSZmmZ70rnTOW1xvToenXBGpuFoi6kndHRqsSxODvCbq3vRILobkCbmCScfOhWxIxQg-Fh-9UXQTIUET8mlPKSksa1nCteMdshq_H-c0Bs3IYDVBwsw-qFc7KU6gQB4J-mOikZJ3wBLU0Zt9dXIsnioAdsFklL2Cn9Nvs2c4fVVDbsdsv17Umv0Z846gzgUUmte2-UdKeUVe1F1oUp4TpPWkyr7XBH40k3xkEWMFFMZ73aN_XBZzUAlgbvljgS8MJOI26TbVJKXiKRYPh5fy6uDyweMfm7yWRvDMMNHap2JzYxS_YbmV5E8Xqc5rXSOxP_HsB-9XzHSOzSRn1eEl25DJniLmOFlwfKdGtIFlOz-X-ZCRsTdyYaZDb2pUKc9QUCHNwW2D6qmIglwixg8SBY-JQfQ6hto0FD9NNPBszTKP4yrcjbbbNdip1StuzbbOtQklk5PZ3pBcyxXZRZnIq7Yf5uV2Vl4Sr5OY1eyx-zY7MeTkTyq830P1nCTKoAgjPyeNddHwpwM";

            var mockSettings = new Mock<IOptions<SecuritySettings>>();
            mockSettings.Setup(m => m.Value).Returns(() => settings);

            try
            {
                var service = new JwtTokenService(new List<ISecurityService> { new Rs256SecurityService(mockSettings.Object) }, mockSettings.Object);
                var result = service.Validate(tokenString);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Invalid issuer: local.auth.issuer1");
            }
        }

        [TestMethod]
        public void ValidateRS256Token_CertificateWithoutPrivateKey_ReturnsError()
        {
            var settings = new SecuritySettings
            {
                SecurityType = SecurityTypeEnum.RS256,
                Audience = "local.auth.audience",
                Issuer = "local.auth.issuer",
                CertificateData = "***"
            };

            var tokenString = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJlbWFpbCI6ImV4YW1wbGVAdGVzdC5jb20iLCJpYXQiOiIxNTg2ODYxOTg5IiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZSI6IiIsIm5iZiI6IjE1ODY4NjE5ODgiLCJleHAiOiIxNTg3ODc4MTU0IiwiaXNzIjoibG9jYWwuYXV0aC5pc3N1ZXIiLCJhdWQiOiJsb2NhbC5hdXRoLmF1ZGllbmNlIiwianRpIjoiNDVhYmI2ZDQtOGYxNi00YzQzLTk3MzctZDlmMTM3NjhjOThiIn0.u8rHV4OzCfpm8pwiB_XWta1kR6ok3GYiSF1aZ24y6vI";

            var mockSettings = new Mock<IOptions<SecuritySettings>>();
            mockSettings.Setup(m => m.Value).Returns(() => settings);

            try
            {
                var service = new JwtTokenService(new List<ISecurityService> { new Rs256SecurityService(mockSettings.Object) }, mockSettings.Object);
                var result = service.Validate(tokenString);
            }
            catch(Exception ex)
            {
                Assert.AreEqual(ex.Message, "Certificate is not X509Certificate2 or does not have private key");
            }
        }
    }
}
