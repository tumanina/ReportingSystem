using System;

namespace ReportingSystem.Dal.Entities
{
    public class AccountActionEntity
    {
        public Guid AccountId { get; set; }

        public virtual AccountEntity Account { get; set; }

        public Guid ActionId { get; set; }

        public virtual ActionEntity Action { get; set; }
    }
}
