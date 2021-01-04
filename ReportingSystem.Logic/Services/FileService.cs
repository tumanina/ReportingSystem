using ReportingSystem.Shared.Interfaces;
using ReportingSystem.Shared.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ReportingSystem.Logic.Services
{
    public class FileService : IFileService
    {
        private readonly IFileStorage _fileStorage;

        public FileService(IFileStorage fileStorage)
        {
            _fileStorage = fileStorage;
        }

        public async Task<FileModel> GetFile(string fileName)
        {
            return await _fileStorage.GetFile(fileName);
        }

        public async Task<IEnumerable<string>> GetFiles()
        {
            return await _fileStorage.GetFileNames();
        }

        public async Task UploadFile(string fileName, Stream fileContent)
        {
            await _fileStorage.UploadFile(fileName, fileContent);
        }
    }
}
