using ReportingSystem.Shared.Models;
using System.Threading.Tasks;

namespace ReportingSystem.Shared.Interfaces.DalServices
{
    public interface IAccountDalService
    {
        Task<AccountModel> GetAccountAsync(string username, string password);

        Task<AccountModel> GetByUsernameAsync(string username);
    }
}
