using ReportingSystem.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReportingSystem.Shared.Interfaces.DalServices
{
	public interface ITemplateDalService
	{
		Task<IEnumerable<TemplateModel>> GetTemplates(string customerCode);
		Task<TemplateModel> GetTemplate(string customerCode, long id);
		Task<Guid> AddTemplate(TemplateModel model);
		Task DeleteTemplate(TemplateModel model);
	}
}
