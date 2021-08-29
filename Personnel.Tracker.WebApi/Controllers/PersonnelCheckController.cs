using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Personnel.Tracker.Common;
using Personnel.Tracker.Common.Authentication;
using Personnel.Tracker.Model.Action;
using Personnel.Tracker.Model.Personnel;
using Personnel.Tracker.WebApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IPersonnelService _personnelService;

        public PersonnelCheckController(ILogger<PersonnelCheckController> logger, IPersonnelCheckService personnelCheckService, IPersonnelService personnelService)
        {
            _logger = logger;
            _personnelCheckService = personnelCheckService;
            _personnelService = personnelService;
        }

        [HttpGet]
        public string Get()
        {
            return "Get Personnels ";
        }

        [HttpPost("last")]
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
            if (query.Parameter != null)
                query.Parameter.Bind(x => x.PersonnelId, UserId);

            return Ok(await _personnelCheckService.AddAsync(query));
        }

        [HttpPost("my-attendances")]
        public async Task<IActionResult> GetMyAttendancesAsync(Query<PersonnelCheck> query)
        {
            if (query.Parameter != null)
                query.Parameter.Bind(x => x.PersonnelId, UserId);

            if (query.Parameter.PersonnelId == Guid.Empty)
            {
                var result = new OperationResult();
                result.PrepareMissingParameterResult("PersonnelId");
                return Ok(result);
            }


            return Ok(await _personnelCheckService.GetPersonnelDayAttencance(query));
        }

        [HttpPost("attendances")]
        public async Task<IActionResult> GetAttendancesAsync(Query<PersonnelCheck> query)
        {

            var result = await _personnelCheckService.GetPersonnelDayAttencance(query);

            if (result.Result)
            {
                if (result.Response != null && result.Response.Any())
                {
                    var presonnelResult = await _personnelService.SearchPersonnel(new Query<Model.Criteria.SearchPersonnelCriteria>(new Model.Criteria.SearchPersonnelCriteria
                    {
                        PersonnelIds = result.Response.Select(x => x.PersonnelId).ToList()
                    })
                    {
                        PageSize = int.MaxValue
                    });

                    if(presonnelResult.Result)
                    {
                        var personnels = presonnelResult.Response ?? new List<Model.Personnel.Personnel>();

                        foreach (var item in result.Response)
                        {
                            item.Personnel = personnels.FirstOrDefault(x => x.PersonnelId == item.PersonnelId);
                        }
                    }
                }
            }

            return Ok(result);
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
