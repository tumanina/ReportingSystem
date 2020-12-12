using System;
using System.Collections.Generic;

namespace ReportingSystem.Dal.Entities
{
    public class ActionEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<AccountActionEntity> Accounts { get; set; }
    }
}
