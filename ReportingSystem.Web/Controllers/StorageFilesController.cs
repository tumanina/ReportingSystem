using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReportingSystem.Shared.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace ReportingSystem.Web.Controllers
{
    [ApiController]
    [Route("api/v1/files")]
    public class StorageFilesController : ControllerBase
    {
        private readonly ILogger<StorageFilesController> _logger;
        private readonly IFileStorageService _fileStorageService;

        public StorageFilesController(ILogger<StorageFilesController> logger, IFileStorageService fileStorageService)
        {
            _logger = logger;
            _fileStorageService = fileStorageService;
        }

        [HttpPost]
        [Route("upload")]
        public async Task<bool> UploadFile()
        {
            var result = false;
            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files[0];
                var filePath = Path.GetTempFileName();

                if (file.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                        result = await _fileStorageService.UploadFile(file.FileName, stream);
                    }
                }
            }

            return result;
        }
    }
}
