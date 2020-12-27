using System.Collections.Generic;

namespace ReportingSystem.Dal.Entities
{
    public class GroupEntity : BaseEntity
    {
        public string Name { get; set; }

        public virtual ICollection<ReportEntity> Reports { get; set; }
    }
}
