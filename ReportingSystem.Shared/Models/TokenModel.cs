using System;

namespace ReportingSystem.Shared.Models
{
    public class TokenModel
    {
        public string Token { get; set; }
        public DateTime ExpiredAt { get; set; }
    }
}
