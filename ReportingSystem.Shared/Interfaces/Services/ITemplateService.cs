using ReportingSystem.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReportingSystem.Shared.Interfaces
{
    public interface ITemplateService
    {
        Task<IEnumerable<TemplateModel>> GetTemplates();
        Task<TemplateModel> GetTemplate(Guid id);
        Task AddTemplateVersion(Guid templateId, string version, string fileName);
    }
}
