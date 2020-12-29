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
    public class TemplateDalService : ITemplateDalService
    {
        private readonly ReportingDbContext _dbContext;

        public TemplateDalService(ReportingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<TemplateModel>> GetTemplates()
        {
            var templates = await _dbContext.Templates.Include(t => t.Versions).ToListAsync();
            return templates.Select(r => r.Map());
        }

        public async Task<TemplateModel> GetTemplate(Guid id)
        {
            var query = _dbContext.Templates.Where(t => t.Id == id).Include(t => t.Versions);
            var template = await query.FirstOrDefaultAsync();
            if (template == null)
            {
                return null;
            }

            return template?.Map();
        }

        public async Task<Guid> AddTemplate(TemplateModel model)
        {
            var entity = model.Map();
            _dbContext.Templates.Add(entity);

            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task DeleteTemplate(TemplateModel model)
        {
            var entity = model.Map();
            _dbContext.Templates.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
