using System;

namespace ReportingSystem.Web.Models
{
    public class TokenApiModel
    {
        public string Token { get; set; }
        public DateTime ExpiredAt { get; set; }
    }
}
