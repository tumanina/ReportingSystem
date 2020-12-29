using ReportingSystem.Shared.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ReportingSystem.Shared.Interfaces
{
    public interface IFileStorage
    {
        Task UploadFile(string fileName, Stream fileContent);
        Task<IEnumerable<string>> GetFileNames();
        Task<FileModel> GetFile(string fileName);
        Task CreateDirectory(string directoryName);
        Task MoveFile(string fileName, string destinationDirectoryPath);
    }
}
