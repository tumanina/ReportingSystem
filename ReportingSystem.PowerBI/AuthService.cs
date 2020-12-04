using Microsoft.Identity.Client;
using ReportingSystem.PowerBI.Interfaces;
using ReportingSystem.PowerBI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace ReportingSystem.PowerBI
{
    internal class AuthService : IAuthService
    {
        public async Task<TokenModel> Login(string clientId, string userName, string password)
        {
            var app = PublicClientApplicationBuilder.Create(clientId)
                        .WithAuthority("https://login.microsoftonline.com/organizations")
                        .Build();

            var scopes = new List<string> { "https://analysis.windows.net/powerbi/api/.default" };

            SecureString sec_pass = new SecureString();
            Array.ForEach(password.ToArray(), sec_pass.AppendChar);
            sec_pass.MakeReadOnly();

            var token = await app.AcquireTokenByUsernamePassword(scopes, userName, sec_pass).ExecuteAsync();
            return new TokenModel { Token = token.AccessToken, ExpiredDateTimeUtc = token.ExpiresOn.UtcDateTime };
        }
    }
}
