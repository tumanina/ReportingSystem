using System;
using System.Collections.Generic;

namespace ReportingSystem.Dal.Entities
{
    public class AccountEntity
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public virtual ICollection<AccountActionEntity> Actions { get; set; }
    }
}
