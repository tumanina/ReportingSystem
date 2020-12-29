using ReportingSystem.Shared.Interfaces;
using ReportingSystem.Shared.Interfaces.DalServices;
using ReportingSystem.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReportingSystem.Logic.Services
{
    public class TemplateService : ITemplateService
    {
        private readonly ITemplateDalService _templateDalService;
        private readonly ITemplateVersionDalService _templateVersionDalService;

        public TemplateService(ITemplateDalService templateDalService, ITemplateVersionDalService templateVersionDalService)
        {
            _templateDalService = templateDalService;
            _templateVersionDalService = templateVersionDalService;
        }

        public async Task<IEnumerable<TemplateModel>> GetTemplates()
        {
            return await _templateDalService.GetTemplates();
        }

        public async Task<TemplateModel> GetTemplate(Guid id)
        {
            return await _templateDalService.GetTemplate(id);
        }

        public async Task AddTemplateVersion(Guid templateId, string version, string fileName)
        {
            await _templateVersionDalService.AddTemplateVersion(new TemplateVersionModel { FileName = fileName, TemplateId = templateId, Version = version });
        }
    }
}
