using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.IO;
using ReportingSystem.Shared.Interfaces;
using Microsoft.Extensions.Logging;
using ReportingSystem.Web.Models;
using System;

namespace ReportingSystem.Web.Controllers
{
    [Route("api/v1/templates")]
    [ApiController]
    public class TemplatesController : BaseController
    {
        private readonly ITemplateManager _templateManager;

        public TemplatesController(ILogger<TemplatesController> logger, ITemplateManager templateManager)
            : base(logger)
        {
            _templateManager = templateManager;
        }

        [HttpGet]
        public async Task<BaseApiDataModel<IEnumerable<TemplateApiModel>>> GetAll()
        {
            return await Execute(async () =>
            {
                var templates = await _templateManager.GetTemplates();

                return templates.Select(t => new TemplateApiModel { Id = t.Id, Name = t.Name });
            });
        }

        [HttpPost]
        [Route("{templateId}/upload")]
        public async Task<BaseApiModel> UploadTemplateFile(Guid templateId, string version)
        {
            if (Request.Form.Files.Count == 0)
            {
                return new BaseApiModel
                {
                    Errors = new List<string> { "BadRequest: Request does not contain any files." }
                };
            }

            return await Execute(async () =>
            {
                var file = Request.Form.Files[0];
                var filePath = Path.GetTempFileName();

                if (file.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                        await _templateManager.UploadTemplateFile(templateId, stream, file.FileName, version);
                    }
                }
            });
        }
    }
}