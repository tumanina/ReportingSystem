using System.Collections.Generic;

namespace ReportingSystem.Shared.Models
{
    public class GroupModel : BaseModel
    {
        public string Name { get; set; }

        public IEnumerable<ReportModel> Reports { get; set; }
    }
}
