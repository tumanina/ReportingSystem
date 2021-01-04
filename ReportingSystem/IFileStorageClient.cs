using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReportingSystem
{
    public interface IFileStorageClient
    {
        Task<IEnumerable<string>> GetFileNames();
    }
}
