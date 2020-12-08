using System.IO;

namespace ReportingSystem.Web.Models
{
    public class FileApiModel
    {
        public string Name { get; set; }

        public Stream FileStream { get; set; }
    }
}
