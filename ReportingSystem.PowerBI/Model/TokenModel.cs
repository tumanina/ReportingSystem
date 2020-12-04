using System;

namespace ReportingSystem.PowerBI.Model
{
    public class TokenModel
    {
        public string Token { get; set; }

        public DateTime ExpiredDateTimeUtc { get; set; }
    }
}
