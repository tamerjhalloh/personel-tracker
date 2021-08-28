using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Personnel.Tracker.Common.Authentication;

namespace Personnel.Tracker.WebApi.Controllers
{
    [ApiController]
    [Route("personnels")]
    [JwtAuth]
    public class PersonnelController : BaseController
    {         
        private readonly ILogger<PersonnelController> _logger; 
        public PersonnelController(ILogger<PersonnelController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            return "Get Personnels ";
        }

        [HttpPost]
        public string Add()
        {
            return "Added";
        }

        [HttpPut]
        public string Update()
        {
            return "Updated";
        }
       
        [HttpDelete]
        public string Delete()
        {
            return "Deleted";
        }

    }
}
