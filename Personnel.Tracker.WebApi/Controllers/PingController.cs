using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Personnel.Tracker.WebApi.Controllers
{
    [ApiController]
    [Route("ping")]
    public class PingController : ControllerBase
    {         
        private readonly ILogger<PingController> _logger; 
        public PingController(ILogger<PingController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            return "pong";
        }
    }
}
