using ReportingSystem.Shared.Models;
using System.Threading.Tasks;

namespace ReportingSystem.Shared.Interfaces.Authentification
{
    public interface IAuthorizationService
    {
        Task<TokenModel> Login(string username, string action);
        bool UserHasAccess(AccountModel account, string action);
        Task<bool> UserHasAccess(string username, string action);
    }
}
