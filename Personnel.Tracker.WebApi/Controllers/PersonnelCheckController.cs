using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Personnel.Tracker.Common.Authentication;
using Personnel.Tracker.WebApi.Services;
using System.Threading.Tasks;

namespace Personnel.Tracker.WebApi.Controllers
{
    [ApiController]
    [Route("personnel-checks")]
    [JwtAuth]
    public class PersonnelCheckController : BaseController
    {
        private readonly ILogger<PersonnelCheckController> _logger;
        private readonly IPersonnelCheckService _personnelCheckService;
        public PersonnelCheckController(ILogger<PersonnelCheckController> logger, IPersonnelCheckService personnelCheckService)
        {
            _logger = logger;
            _personnelCheckService = personnelCheckService;
        }

        [HttpGet]
        public string Get()
        {
            return "Get Personnels ";
        }

        [HttpGet("last")]
        public async Task<IActionResult> GetLastCheck()
        {
            return Ok(await _personnelCheckService.GetLastPersonnelCheck(new Model.Action.Query<Model.Personnel.Personnel>(new Model.Personnel.Personnel
            {
                PersonnelId = UserId
            })));
        }

        [HttpPost]
        public string Add()
        {
            return "Added";
        }

        //[HttpPut]
        //public string Update()
        //{
        //    return "Updated";
        //}

        //[HttpDelete]
        //public string Delete()
        //{
        //    return "Deleted";
        //}

    }
}
