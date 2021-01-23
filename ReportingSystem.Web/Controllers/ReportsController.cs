using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReportingSystem.Shared.Interfaces;
using ReportingSystem.Web.Authentication;
using ReportingSystem.Web.Models;
using System;
using System.Threading.Tasks;

namespace ReportingSystem.Web.Controllers
{
    [Route("api/v1/reports")]
    [Authorization]
    public class ReportsController : BaseController
    {
        private readonly IReportManager _reportManager;

        public ReportsController(ILogger<ReportsController> logger, IReportManager reportManager)
            :base(logger)
        {
            _reportManager = reportManager;
        }

        [HttpPost]
        [Route("{reportId}/deploy")]
        public async Task<BaseApiModel> Deploy(Guid reportId, DeployReportApiModel reportModel)
        {
            return await Execute(async () =>
            {
                await _reportManager.Deploy(reportModel.ReportEngineTool, reportId, reportModel.TemplateVersionId);
            });
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<BaseApiModel> DeleteReport(Guid id)
        {
            return await Execute(async () =>
            {
                await _reportManager.DeleteReport(id);
            });
        }
    }
}
