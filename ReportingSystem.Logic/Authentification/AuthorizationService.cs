using AuthService.Client.Interfaces;
using Microsoft.Extensions.Options;
using ReportingSystem.Shared.Configuration;
using ReportingSystem.Shared.Interfaces.Authentification;
using ReportingSystem.Shared.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ReportingSystem.Logic.Authentification
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IAuthorizationClient _authorizationClient;

        public AuthorizationService(IAuthorizationClient authorizationClient, IOptions<AuthServiceSettings> authServiceSettings)
        {
            _authorizationClient = authorizationClient;

            if (authServiceSettings.Value != null)
            {
                _authorizationClient.ConfigureClient(authServiceSettings.Value.Url);
            }
        }

        public bool UserHasAccess(AccountModel account, string action)
        {
            action = action?.ToLower() ?? string.Empty;
            return account.Actions.Any(c => c.Name.Equals(action, StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task<bool> UserHasAccess(string username, string action)
        {
            var getUserHasAccessResult = await _authorizationClient.UserHasAccess(username, action);

            if (!getUserHasAccessResult.Success)
            {
                throw new UnauthorizedAccessException(string.Join(',', getUserHasAccessResult.Errors));
            }

            return getUserHasAccessResult.Data;
        }
    }
}
