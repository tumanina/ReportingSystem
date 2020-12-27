using ReportingSystem.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReportingSystem.Shared.Interfaces.DalServices
{
    public interface IReportDalService
	{
		Task<IEnumerable<ReportModel>> GetReports();
		Task<ReportModel> GetReport(long id);
		Task<Guid> AddReport(ReportModel model);
		Task DeleteReport(Guid id);
	}
}
