using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Personnel.Tracker.Common.Authentication;
using Personnel.Tracker.Model.Action;
using Personnel.Tracker.Model.Criteria;
using Personnel.Tracker.WebApi.Services;
using System.Threading.Tasks;

namespace Personnel.Tracker.WebApi.Controllers
{
    [ApiController]
    [Route("personnels")]
    [JwtAuth]
    public class PersonnelController : BaseController
    {
        private readonly ILogger<PersonnelController> _logger;
        private readonly IPersonnelService _personnelService;
        public PersonnelController(ILogger<PersonnelController> logger, IPersonnelService personnelService)
        {
            _logger = logger;
            _personnelService = personnelService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok("Get Personnels ");
        }

        [HttpPost]
        public async Task<IActionResult> Add()
        {
            return Ok("Added");
        }

        [HttpPut]
        public async Task<IActionResult> Update()
        {
            return Ok("Updated");
        }

        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            return Ok("Deleted");
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search(Query<SearchPersonnelCriteria> query)
        {
            return Ok(await _personnelService.SearchPersonnel(query));
        }


    }
}
