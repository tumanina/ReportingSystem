using System.Collections.Generic;
using System.Linq;

namespace ReportingSystem.Web.Models
{
    public class BaseApiModel
    {
        public bool Success
        {
            get
            {
                return Errors == null || !Errors.Any();
            }
        }

        public IEnumerable<string> Errors { get; set; }
    }

    public class BaseApiDataModel<T> : BaseApiModel
    {
        public T Data { get; set; }
    }
}
