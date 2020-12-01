using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ReportingSystem.Web.Controllers
{
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<Controller> _logger;

        public HealthController(ILogger<Controller> logger)
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
