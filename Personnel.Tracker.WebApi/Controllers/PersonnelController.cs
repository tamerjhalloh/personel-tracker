using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Personnel.Tracker.WebApi.Controllers
{
    [ApiController]
    [Route("personnels")]
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
