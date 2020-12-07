using ReportingSystem.Shared.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ReportingSystem.Shared.Interfaces
{
    public interface IFileStorage
    {
        Task UploadFile(string fileName, Stream fileContent);
        Task<IEnumerable<string>> GetFileNames(string storagePath);
        Task<FileModel> GetFile(string storagePath, string fileName);
        Task CreateDirectory(string storagePath, string directoryName);
        Task MoveFile(string storagePath, string fileName, string destinationDirectoryPath);
    }
}
