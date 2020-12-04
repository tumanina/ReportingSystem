﻿using ReportingSystem.Shared.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace ReportingSystem.Logic
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IFileStorage _fileStorage;

        public FileStorageService(IFileStorage fileStorage)
        {
            _fileStorage = fileStorage;
        }

        public async Task<bool> UploadFile(string fileName, Stream fileContent)
        {
            return await _fileStorage.UploadFile(fileName, fileContent);
        }
    }
}