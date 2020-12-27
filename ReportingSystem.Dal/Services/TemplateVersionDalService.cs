using System;
using System.Threading.Tasks;
using ReportingSystem.Dal.DbContexts;
using ReportingSystem.Dal.Mappers;
using ReportingSystem.Shared.Interfaces.DalServices;
using ReportingSystem.Shared.Models;

namespace ReportingSystem.Dal.Services
{
    public class TemplateVersionDalService : ITemplateVersionDalService
    {
        private readonly ReportingDbContext _dbContext;

        public TemplateVersionDalService(ReportingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> AddTemplateVersion(TemplateVersionModel templateVersion)
        {
            var entity = templateVersion.Map();
            _dbContext.TemplateVersions.Add(entity);

            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task DeleteTemplateVersion(TemplateVersionModel templateVersion)
        {
            var entity = templateVersion.Map();
            _dbContext.TemplateVersions.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
