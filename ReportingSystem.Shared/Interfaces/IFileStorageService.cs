using System.IO;
using System.Threading.Tasks;

namespace ReportingSystem.Shared.Interfaces
{
    public interface IFileStorageService
    {
        Task<bool> UploadFile(string fileName, Stream fileContent);
    }
}
