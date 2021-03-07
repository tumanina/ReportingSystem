using AuthService.Client.Interfaces;
using AuthService.Common.Interfaces.Services;
using Microsoft.Extensions.Options;
using ReportingSystem.Shared.Configuration;
using ReportingSystem.Shared.Interfaces.Authentification;
using ReportingSystem.Shared.Interfaces.DalServices;
using ReportingSystem.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReportingSystem.Logic.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountClient _accountClient;

        public AccountService(IAccountClient accountClient, IOptions<AuthServiceSettings> authServiceSettings)
        {
            _accountClient = accountClient;

            if (authServiceSettings.Value != null)
            {
                _accountClient.ConfigureClient(authServiceSettings.Value.Url);
            }
        }

        public async Task<AccountModel> GetAccountAsync(string username, string password)
        {
            var getAccountResult = await _accountClient.GetAccountAsync(username, password);

            if (!getAccountResult.Success)
            {
                throw new UnauthorizedAccessException(string.Join(',', getAccountResult.Errors));
            }

            var account = getAccountResult.Data;

            if (account == null)
            {
                throw new UnauthorizedAccessException("Account not found");
            }

            return new AccountModel { Id = account.Id, Username = account.UserName };
        }

        public Task<AccountModel> GetByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }
    }
}
