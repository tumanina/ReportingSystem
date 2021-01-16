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
        private readonly IAccountDalService _dalService;

        public AccountService(IAccountDalService dalService)
        {
            _dalService = dalService;
        }

        public async Task<AccountModel> GetByUsernameAsync(string username)
        {
            return await _dalService.GetByUsernameAsync(username);
        }

        public async Task<AccountModel> GetAccountAsync(Guid id)
        {
            return await _dalService.GetAccountAsync(id);
        }

        public async Task<AccountModel> GetAccountAsync(string username, string password)
        {
            return await _dalService.GetAccountAsync(username, password);
        }

        public async Task<AccountModel> CreateAccount(string username, string password, IEnumerable<Guid> actionIds)
        {
            return await _dalService.CreateAccountAsync(username, password, actionIds);
        }

        public async Task AddActionsToAccount(Guid accountId, IEnumerable<Guid> actionIds)
        {
            await _dalService.AddActionsToAccountAsync(accountId, actionIds);
        }
    }
}
