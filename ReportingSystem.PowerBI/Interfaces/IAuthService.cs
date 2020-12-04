using ReportingSystem.PowerBI.Model;
using System.Threading.Tasks;

namespace ReportingSystem.PowerBI.Interfaces
{
    public interface IAuthService
    {
        Task<TokenModel> Login(string customerId, string userName, string password);
    }
}
