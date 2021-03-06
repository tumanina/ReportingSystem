﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReportingSystem.Shared.Interfaces;
using ReportingSystem.Web.Authentication;
using ReportingSystem.Web.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ReportingSystem.Web.Controllers
{
    [Route("api/v1/files")]
    [Authorization]
    public class StorageFilesController : BaseController
    {
        private readonly IFileService _fileStorageService;

        public StorageFilesController(ILogger<StorageFilesController> logger, IFileService fileStorageService)
            :base(logger)
        {
            _fileStorageService = fileStorageService;
        }

        [HttpGet]
        public async Task<BaseApiDataModel<IEnumerable<string>>> GetFiles()
        {
            return await Execute(async () =>
            {
                return  await _fileStorageService.GetFiles();
            });
        }

        [HttpPost]
        [Route("upload")]
        public async Task<BaseApiModel> UploadFile()
        {
            return await Execute(async () =>
            {
                if (Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files[0];
                    var filePath = Path.GetTempFileName();

                    if (file.Length > 0)
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                            await _fileStorageService.UploadFile(file.FileName, stream);
                        }
                    }
                }
            });
        }
    }
}
