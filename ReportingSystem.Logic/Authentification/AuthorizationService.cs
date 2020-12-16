using ReportingSystem.Shared.Interfaces.Authentification;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ReportingSystem.Logic.Authentification
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IAccountService _accountService;

        public AuthorizationService(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<bool> UserHasAccess(string username, string action)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(action))
            {
                throw new Exception("Required parameter is null or empty");
            }

            var account = await _accountService.GetByUsernameAsync(username.ToLower());

            action = action?.ToLower() ?? string.Empty;
            return account.Actions.Any(c => c.Name.Equals(action, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
