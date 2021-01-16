using ReportingSystem.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReportingSystem.Shared.Interfaces.DalServices
{
    public interface IAccountDalService
    {
        Task<AccountModel> GetAccountAsync(string username, string password);

        Task<AccountModel> GetByUsernameAsync(string username);

        Task<AccountModel> GetAccountAsync(Guid id);

        Task<AccountModel> CreateAccountAsync(string username, string password, IEnumerable<Guid> actionIds);

        Task AddActionsToAccountAsync(Guid id, IEnumerable<Guid> actionIds);
    }
}
