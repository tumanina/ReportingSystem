using ReportingSystem.Shared.Models;
using System;
using System.Threading.Tasks;

namespace ReportingSystem.Shared.Interfaces.Authentification
{
    public interface IAccountService
    {
        Task<AccountModel> GetByUsernameAsync(string username);
        Task<AccountModel> GetAccountAsync(string username, string password);
    }
}
