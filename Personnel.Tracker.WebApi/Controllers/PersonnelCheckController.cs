using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Personnel.Tracker.Common;
using Personnel.Tracker.Common.Authentication;
using Personnel.Tracker.Model.Action;
using Personnel.Tracker.Model.Personnel;
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

        
        [HttpPost("add")]
        public async Task<IActionResult> AddAsync(Query<PersonnelCheck> query)
        {
            if(query.Parameter != null)
                query.Parameter.Bind(x => x.PersonnelId, UserId);

            return Ok(await _personnelCheckService.AddAsync(query));
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
