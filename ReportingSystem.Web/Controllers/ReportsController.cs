using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReportingSystem.Shared.Interfaces;
using ReportingSystem.Web.Authentication;
using ReportingSystem.Web.Models;
using System.Threading.Tasks;

namespace ReportingSystem.Web.Controllers
{
    [Route("api/v1/reports")]
    [Authentication]
    public class ReportsController : BaseController
    {
        private readonly IReportManager _reportManager;

        public ReportsController(ILogger<ReportsController> logger, IReportManager reportManager)
            :base(logger)
        {
            _reportManager = reportManager;
        }

        [HttpPost]
        [Route("deploy")]
        public async Task<BaseApiDataModel<ReportApiModel>> Deploy(DeployReportApiModel reportModel)
        {
            return await Execute<ReportApiModel>(async () =>
            {
                var deployedReport = await _reportManager.Deploy(reportModel.FileName, reportModel.GroupId, reportModel.Name);

                return new ReportApiModel { Id = deployedReport.Id, Name = deployedReport.Name, DatasetId = deployedReport.DatasetId };
            });
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<BaseApiModel> DeleteReport(string id, string groupId)
        {
            return await Execute(async () =>
            {
                await _reportManager.DeleteReport(groupId, id);
            });
        }
    }
}
