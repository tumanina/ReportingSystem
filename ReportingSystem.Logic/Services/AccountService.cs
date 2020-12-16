using ReportingSystem.Shared.Interfaces;
using ReportingSystem.Shared.Interfaces.Authentification;
using ReportingSystem.Shared.Models;
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
    }
}
