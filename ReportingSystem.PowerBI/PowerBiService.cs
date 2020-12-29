using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;
using ReportingSystem.PowerBI.Interfaces;
using ReportingSystem.Shared.Configuration;
using ReportingSystem.Shared.Enums;
using ReportingSystem.Shared.Interfaces;
using ReportingSystem.Shared.Models;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ReportingSystem.PowerBI
{
    public class PowerBiService : IReportEngineTool
    {
        private readonly PowerBiConfiguration _powerBiConfiguration;
        private readonly IAuthService _authService;

        public ReportEngineToolEnum ReportEngineTool =>  ReportEngineToolEnum.PowerBi;

        public PowerBiService(IAuthService authService, IOptions<PowerBiConfiguration> powerBiConfiguration)
        {
            _authService = authService;
            _powerBiConfiguration = powerBiConfiguration.Value;
        }

        public async Task<ReportEngineToolReportModel> CreateReport(string groupReportId, string templateReportId, string name)
        {
            if (Guid.TryParse(templateReportId, out Guid powerBiReportId) && Guid.TryParse(groupReportId, out Guid powerBiGroupId))
            {
                return await Execute(async (client) =>
                {
                    var clonedReport = await client.Reports.CloneReportInGroupAsync(powerBiGroupId, powerBiReportId, new CloneReportRequest(name));

                    return new ReportEngineToolReportModel
                    {
                        ReportEngineTool = ReportEngineTool,
                        ReportId = clonedReport.Id.ToString(),
                        DatasetId = clonedReport.DatasetId
                    };
                });
            }

            return null;
        }

        public async Task<string> CreateGroup(string name)
        {
            return await Execute(async (client) =>
            {
                var group = await client.Groups.CreateGroupAsync(new GroupCreationRequest(name));

                return group.Id.ToString();
            });
        }

        public async Task DeleteReport(string groupId, string reportId)
        {
            if (Guid.TryParse(reportId, out Guid powerBiReportId) && Guid.TryParse(groupId, out Guid powerBiGroupId))
            {
                await Execute(async (client) =>
                {
                    await client.Reports.DeleteReportInGroupAsync(powerBiGroupId, powerBiReportId);
                });
            }
        }
        public async Task<ReportEngineToolReportModel> Deploy(Stream file, string groupId, string datasetName)
        {
            if (Guid.TryParse(groupId, out Guid powerBiGroupId))
            {
                return await Execute(async (client) =>
                {
                    if (file is MemoryStream)
                    {
                        file.Position = 0;
                    }
                    var import = await client.Imports.PostImportWithFileAsyncInGroup(powerBiGroupId, file, datasetName, nameConflict: ImportConflictHandlerMode.CreateOrOverwrite);

                    while (client.Imports.GetImportInGroup(powerBiGroupId, import.Id).ImportState.Equals("Publishing"))
                    {
                        Thread.Sleep(2000);
                    }

                    var report = client.Imports.GetImportInGroup(powerBiGroupId, import.Id).Reports[0];
                    report = client.Reports.GetReportInGroup(powerBiGroupId, report.Id);

                    return new ReportEngineToolReportModel { ReportId = report.Id.ToString(), DatasetId = report.DatasetId, ReportEngineTool = ReportEngineTool };
                });
            }
            return null;
        }

        private async Task<T> Execute<T>(Func<PowerBIClient, Task<T>> func)
        {
            var tokenCredentials = await GetTokenCredentials();

            using (var client = new PowerBIClient(tokenCredentials))
            {
                return await func(client);
            }
        }

        private async Task Execute(Func<PowerBIClient, Task> func)
        {
            var tokenCredentials = await GetTokenCredentials();

            using (var client = new PowerBIClient(tokenCredentials))
            {
                await func(client);
            }
        }

        private async Task<TokenCredentials> GetTokenCredentials()
        {
            var token = await _authService.Login(_powerBiConfiguration.ClientId, _powerBiConfiguration.UserName, _powerBiConfiguration.Password);

            if (token == null)
            {
                throw new Exception("Authorization failed");
            }

            return new TokenCredentials(token.Token, "Bearer");
        }
    }
}
