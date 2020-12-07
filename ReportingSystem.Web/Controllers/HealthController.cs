using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReportingSystem.Web.Models;

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
        public BaseApiDataModel<string> Ping()
        {
            return new BaseApiDataModel<string> { Data = "response" };
        }
    }
}
