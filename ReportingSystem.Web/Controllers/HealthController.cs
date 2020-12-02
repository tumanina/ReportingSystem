using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ReportingSystem.Web.Controllers
{
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("ping")]
        public string Ping()
        {
            return "response";
        }
    }
}
