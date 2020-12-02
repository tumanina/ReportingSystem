using System.IO;

namespace ReportingSystem.Shared.Models
{
    public class FileModel
    {
        public string Name { get; set; }

        public Stream FileStream { get; set; }
    }
}
