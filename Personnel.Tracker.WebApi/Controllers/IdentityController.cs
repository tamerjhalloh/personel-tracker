using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Personnel.Tracker.Common;
using Personnel.Tracker.Common.Authentication;
using Personnel.Tracker.Model.Auth;
using Personnel.Tracker.WebApi.Services;
using System;
using System.Threading.Tasks;

namespace Personnel.Tracker.WebApi.Controllers
{
    [ApiController]
    [Route("")]
    public class IdentityController : BaseController
    {

        private readonly ILogger<PersonnelController> _logger;
        private readonly IIdentityService _identityService;
        private readonly IAccessTokenService _accessTokenService;
        public IdentityController(ILogger<PersonnelController> logger,
            IIdentityService identityService,
            IAccessTokenService accessTokenService)
        {
            _logger = logger;
            _identityService = identityService;
            _accessTokenService = accessTokenService;
        }


        // User, Login ....
        /// <summary>
        /// Reads Identity username and get personnel
        /// </summary>
        /// <returns></returns>
        [HttpGet("me")]
        [JwtAuth]
        public async Task<IActionResult> Get()
        {
            var result = await _identityService.GetAsync(UserId);
            return Ok(result);
        }

        /// <summary>
        /// Logins user into system
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn(SignIn command)
            => Ok(await _identityService.SignInAsync(command.Username, command.Password));


        /// <summary>
        /// Changes personnel password
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut("me/password")]
        [JwtAuth]
        public async Task<ActionResult> ChangePassword(ChangePassword command)
        {
            await _identityService.ChangePasswordAsync(new Guid(), command.CurrentPassword, command.NewPassword);

            return NoContent();
        }



        // Tokens
        /// <summary>
        /// Refreshed JWT access token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        [HttpGet("access-tokens/refresh/{refreshToken}")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshAccessToken(string refreshToken)
           => Ok(await _identityService.CreateAccessTokenAsync(refreshToken));

        /// <summary>
        /// Revokes access token
        /// </summary>
        /// <param name="command">RevokeAccessToken</param>
        /// <returns></returns>
        [HttpPost("access-tokens/revoke")]
        public async Task<IActionResult> RevokeAccessToken(RevokeAccessToken command)
        {
            await _accessTokenService.DeactivateCurrentAsync(
                command.Bind(c => c.UserId, UserId).UserId.ToString("N"));

            return NoContent();
        }

        /// <summary>
        /// Revoke resfresh token
        /// </summary>
        /// <param name="refreshToken">refreshToken</param>
        /// <param name="command">RevokeRefreshToken</param>
        /// <returns></returns>
        [HttpPost("refresh-tokens/{refreshToken}/revoke")]
        public async Task<IActionResult> RevokeRefreshToken(string refreshToken, RevokeRefreshToken command)
        {
            await _identityService.RevokeRefreshTokenAsync(command.Bind(c => c.Token, refreshToken).Token,
                command.Bind(c => c.UserId, UserId).UserId);

            return NoContent();
        }
    }
}
