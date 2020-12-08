using ReportingSystem.Shared.Models;
using System.IO;
using System.Threading.Tasks;

namespace ReportingSystem.Shared.Interfaces
{
    public interface IFileService
    {
        Task UploadFile(string fileName, Stream fileContent);
        Task<FileModel> GetFile(string fileName);
    }
}
