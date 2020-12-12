using Microsoft.EntityFrameworkCore;
using ReportingSystem.Dal.DbContexts;
using ReportingSystem.Dal.Entities;
using ReportingSystem.Dal.Mappers;
using ReportingSystem.Shared.Interfaces;
using ReportingSystem.Shared.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ReportingSystem.Dal.Services
{
    internal class AccountDalService : IAccountDalService
    {
        private readonly ReportingDbContext _dbContext;

        public AccountDalService(ReportingDbContext dbContext)
        {
            _dbContext = dbContext;
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
    }
}
