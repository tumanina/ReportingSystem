using ReportingSystem.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ReportingSystem.Shared.Interfaces
{
    public interface ITemplateManager
    {
        Task<IEnumerable<TemplateModel>> GetTemplates();
        Task UploadTemplateFile(Guid templateId, Stream fileContent, string fileName, string version);
    }
}
