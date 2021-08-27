using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Personnel.Tracker.WebApi.Controllers
{
    [ApiController]
    [Route("personnels")]
    public class PersonnelController : ControllerBase
    {         
        private readonly ILogger<PersonnelController> _logger; 
        public PersonnelController(ILogger<PersonnelController> logger)
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
