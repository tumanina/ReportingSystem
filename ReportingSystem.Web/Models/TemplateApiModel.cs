using System;
using System.Collections.Generic;

namespace ReportingSystem.Web.Models
{
    public class TemplateApiModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual IEnumerable<TemplateVersionApiModel> Versions { get; set; }
    }
}
