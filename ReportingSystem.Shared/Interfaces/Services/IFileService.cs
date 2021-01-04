using ReportingSystem.Shared.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ReportingSystem.Shared.Interfaces
{
    public interface IFileService
    {
        Task<IEnumerable<string>> GetFiles();
        Task<FileModel> GetFile(string fileName);
        Task UploadFile(string fileName, Stream fileContent);
    }
}
