using System.Threading.Tasks;

namespace ReportingSystem.Shared.Interfaces.Authentification
{
    public interface IAuthorizationService
    {
        Task<bool> UserHasAccess(string username, string action);
    }
}
