using ReportingSystem.Shared.Models;
using System.Threading.Tasks;

namespace ReportingSystem.Shared.Interfaces.Authentification
{
    public interface IAuthorizationService
    {
        bool UserHasAccess(AccountModel account, string action);
        Task<bool> UserHasAccess(string username, string action);
    }
}
