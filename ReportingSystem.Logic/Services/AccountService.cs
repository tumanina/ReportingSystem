using ReportingSystem.Shared.Interfaces.Authentification;
using ReportingSystem.Shared.Models;
using System.Threading.Tasks;

namespace ReportingSystem.Logic.Services
{
    public class AccountService : IAccountService
    {
        public Task<AccountModel> GetByUsernameAsync(string username)
        {
            throw new System.NotImplementedException();
        }
    }
}
