using Microsoft.EntityFrameworkCore;
using ReportingSystem.Dal.DbContexts;
using ReportingSystem.Dal.Mappers;
using ReportingSystem.Shared.Interfaces.DalServices;
using ReportingSystem.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReportingSystem.Dal.Services
{
    public class ReportDalService : IReportDalService
    {
        private readonly ReportingDbContext _dbContext;

        public ReportDalService(ReportingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ReportModel> GetReport(Guid id)
        {
            var query = _dbContext.Reports.Where(r => r.Id == id).Include(r => r.Group).Include(r => r.Template);
            var report = await query.FirstOrDefaultAsync();
            if (report == null)
            {
                return null;
            }

            return report?.Map();
        }

        public async Task<IEnumerable<ReportModel>> GetReports()
        {
            var reports = await _dbContext.Reports.Include(r => r.Group).Include(r => r.Template).ToListAsync();
            return reports.Select(r => r.Map());
        }

        public async Task<Guid> AddReport(ReportModel model)
        {
            var entity = model.Map();
            entity.CreatedDate = DateTime.UtcNow;
            _dbContext.Reports.Add(entity);

            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task DeleteReport(Guid id)
        {
            var report = await _dbContext.Reports.FirstOrDefaultAsync(r => r.Id == id);
            if (report == null)
            {
                throw new Exception($"Report with id {id} not exists.");
            }

            _dbContext.Reports.Remove(report);
            await _dbContext.SaveChangesAsync();
        }
    }
}
