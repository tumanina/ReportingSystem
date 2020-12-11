using ReportingSystem.Shared.Models;
using System.Threading.Tasks;

namespace ReportingSystem.Shared.Interfaces.Authentification
{
    public interface IAccountService
    {
        Task<AccountModel> GetByUsernameAsync(string username);
    }
}
