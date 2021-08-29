using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Personnel.Tracker.Model.Action;
using Personnel.Tracker.Model.Auth;
using Personnel.Tracker.Model.Criteria;
using Personnel.Tracker.Model.Personnel;
using Personnel.Tracker.Portal.Helpers;
using Personnel.Tracker.Portal.Models;
using Personnel.Tracker.Portal.Services;

namespace Personnel.Tracker.Portal.Controllers
{
    public class ApiController : AuthController
    {
        private readonly ILogger<ApiController> _logger;

        private readonly IIdentityService _identityService;
        private readonly IPersonnelCheckService _personnelCheckService;
        private readonly IPersonnelService _personnelService;



        public ApiController(ILogger<ApiController> logger, IIdentityService identityService, IPersonnelCheckService personnelCheckService, IPersonnelService personnelService)
        {
            _logger = logger;
            _identityService = identityService;
            _personnelCheckService = personnelCheckService;
            _personnelService = personnelService;
        }



        [HttpPost("api/sign-in")]
        public async Task<IActionResult> SignIn(SignIn model)
        {
            var result = new OperationResult<object>();

            try
            {
                var token = await _identityService.SignIn(model);

                if (token != null && !string.IsNullOrEmpty(token.AccessToken))
                {
                    var personnel = await _identityService.Me($"Bearer {token.AccessToken}");

                    if (personnel != null)
                    {
                        var identiyUser = new UserIdentity
                        {
                            Id = personnel.PersonnelId,
                            Email = personnel.Email,
                            Name = personnel.Name,
                            Surname = personnel.Surname,
                            RefreshToken = token.RefreshToken,
                            Token = token.AccessToken,
                            Role = personnel.PersonnelRole.ToString()
                        };

                        await UserIdentityHelper.SetIdentity(HttpContext, identiyUser);

                        result.Result = true;
                        result.Message = "Login succeedded.";
                        return Ok(result);
                    }
                    else
                    {
                        result.ErrorMessage = "Failed to login in";
                        return NotFound(result);
                    }
                }
                else
                {
                    result.ErrorMessage = "Incorrect username or password!";
                    return Ok(result);
                }

            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Exception while sign in");
            }

            return Ok(result);

        }


        [HttpPost("api/last")]
        public async Task<IActionResult> Last()
        {
            var result = new OperationResult<object>();

            try
            {
                var lastResult = await _personnelCheckService.GetLastCheck();

                result.Response = lastResult.Response;
                result.Result = true;
                return Ok(result);

            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Exception while get last check");
            }

            return Ok(result);

        }

        [HttpPost("api/set-personnel-check")]
        public async Task<IActionResult> SetPersonnelCheck(PersonnelCheck check)
        {
            var result = new OperationResult<object>();

            try
            {
                var lastResult = await _personnelCheckService.SetPersonnelCheck(new Query<PersonnelCheck>(check));

                result.Response = lastResult.Response;
                result.Result = true;
                return Ok(result);

            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Exception while set check");
            }

            return Ok(result);

        }


        [HttpPost("api/my-attendances")]
        public async Task<IActionResult> MyAttendances(PersonnelCheck check)
        {
            var result = new OperationResult<object>();

            try
            {
                var getResult = await _personnelCheckService.GetMyPersonnelDayAttencance(new Query<PersonnelCheck>(check));
                if (getResult.Result)
                {
                    result.Response = getResult.Response;
                    result.Result = true;
                    return Ok(result);
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Exception while getting my attendcies");
            }

            return Ok(result);

        }

        [HttpPost("api/attendances")]
        public async Task<IActionResult> Attendances(PersonnelCheck check)
        {
            var result = new OperationResult<object>();

            try
            {
                var getResult = await _personnelCheckService.GetPersonnelDayAttencance(new Query<PersonnelCheck>(check));
                if (getResult.Result)
                {
                    result.Response = getResult.Response;
                    result.Result = true;
                    return Ok(result);
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Exception while getting attendcies");
            }

            return Ok(result);

        }

        [HttpGet("api/personnel/search")]
        public async Task<IActionResult> SearchPersonnel(string query)
        {
            var result = new OperationResult<object>();

            try
            {
                var getResult = await _personnelService.Search(new Query<SearchPersonnelCriteria>(new SearchPersonnelCriteria { Search = query }));
                if (getResult.Result) 
                {
                    result.Response = getResult.Response;
                    result.Result = true;
                    return Ok(result);
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Exception while getting attendcies");
            }

            return Ok(result);

        }


    }
}