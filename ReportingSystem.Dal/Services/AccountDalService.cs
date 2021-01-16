using Microsoft.EntityFrameworkCore;
using ReportingSystem.Dal.DbContexts;
using ReportingSystem.Dal.Entities;
using ReportingSystem.Dal.Mappers;
using ReportingSystem.Shared.Interfaces.DalServices;
using ReportingSystem.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReportingSystem.Dal.Services
{
    public class AccountDalService : IAccountDalService
    {
        private readonly ReportingDbContext _dbContext;

        public AccountDalService(ReportingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AccountModel> GetAccountAsync(Guid id)
        {
            var account = await _dbContext.Set<AccountEntity>().FirstOrDefaultAsync(e => e.Id == id);
            return account.Map();
        }

        public async Task<AccountModel> GetAccountAsync(string username, string password)
        {
            return await _dbContext.Set<AccountEntity>().AsNoTracking().Map().FirstOrDefaultAsync(e => e.Username == username && e.Password == password);
        }

        public async Task<AccountModel> GetByUsernameAsync(string username)
        {
            var account = await _dbContext.Accounts.Where(a => a.Username == username)
                .Include(a => a.Actions).ThenInclude(c => c.Action)
                .FirstOrDefaultAsync();

            return account?.Map();
        }

        public async Task<AccountModel> CreateAccountAsync(string username, string password, IEnumerable<Guid> actionIds)
        {
            var entity = new AccountEntity { Username = username, Password = password };
            entity.CreatedDate = DateTime.UtcNow;
            _dbContext.Accounts.Add(entity);
            _dbContext.AccountActions.AddRange(actionIds.Select(id => new AccountActionEntity { AccountId = entity.Id, ActionId = id }));

            await _dbContext.SaveChangesAsync();

            return entity.Map();
        }

        public async Task AddActionsToAccountAsync(Guid accountId, IEnumerable<Guid> actionIds)
        {
            _dbContext.AccountActions.AddRange(actionIds.Select(id => new AccountActionEntity { AccountId = accountId, ActionId = id }));
            await _dbContext.SaveChangesAsync();
        }
    }
}
