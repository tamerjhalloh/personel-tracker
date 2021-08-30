using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Personnel.Tracker.Common.Authentication;
using Personnel.Tracker.Model.Action;
using Personnel.Tracker.Model.Criteria;
using Personnel.Tracker.WebApi.Services;
using System;
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
        public async Task<IActionResult> Get(string id)
        {
            Guid personnelId = Guid.TryParse(id, out personnelId) ? personnelId : Guid.Empty;
            return Ok(await _personnelService.Get(new Query<Model.Personnel.Personnel>(new Model.Personnel.Personnel { PersonnelId = personnelId })));
        }

        [HttpPost]
        public async Task<IActionResult> Add(Query<Model.Personnel.Personnel> query)
        {
            return Ok(await _personnelService.Add(query));
        }

        [HttpPut]
        public async Task<IActionResult> Update(Query<Model.Personnel.Personnel> query)
        {
            return Ok(await _personnelService.Update(query));
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
