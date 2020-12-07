using System.IO;
using System.Threading.Tasks;

namespace ReportingSystem.Shared.Interfaces
{
    public interface IFileStorageService
    {
        Task UploadFile(string fileName, Stream fileContent);
    }
}
