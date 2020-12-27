using ReportingSystem.Shared.Models;
using System;
using System.Threading.Tasks;

namespace ReportingSystem.Shared.Interfaces.DalServices
{
	public interface ITemplateVersionDalService
    {
		Task<Guid> AddTemplateVersion(TemplateVersionModel templateVersion);
		Task DeleteTemplateVersion(TemplateVersionModel templateVersion);
	}
}
