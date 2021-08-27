using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Personnel.Tracker.Common.Authentication;
using Personnel.Tracker.Model.Action;
using Personnel.Tracker.Model.Auth;
using Personnel.Tracker.WebApi.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Personnel.Tracker.WebApi.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IRefreshTokenRepository _tokenRepository;
        private readonly IPersonnelRepository _personnelRepository;
        private readonly IJwtHandler _jwtHandler;
        private readonly IPasswordHasher<Model.Personnel.Personnel> _passwordHasher;

        private readonly ILogger<IdentityService> _logger;

        public IdentityService(IRefreshTokenRepository tokenRepository,
            IPersonnelRepository personnelRepository,
            IJwtHandler jwtHandler,
            IPasswordHasher<Model.Personnel.Personnel> passwordHasher,
            ILogger<IdentityService> logger)
        {
            _tokenRepository = tokenRepository;
            _personnelRepository = personnelRepository;
            _jwtHandler = jwtHandler;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }


        public async Task<Model.Personnel.Personnel> GetAsync(Guid userId)
        {
            var result = new Model.Personnel.Personnel();
            try
            {
                var userResult = await _personnelRepository.GetAsync(x=> x.PersonnelId == userId);
                if (!userResult.Result)
                {
                    throw new SysException(Codes.UserNotFound,  $"User with id: '{userId}' was not found.");
                }              
                result =  userResult.Response;
            }
            catch (Exception ex)
            { 
                _logger.LogError(ex, "Could not get user {@userId}", userId);
            }

            return result;
        }

        public bool ValidatePassword(Model.Personnel.Personnel personnel, string password, IPasswordHasher<Model.Personnel.Personnel> passwordHasher)
          => passwordHasher.VerifyHashedPassword(personnel, personnel.PasswordHash, password) != PasswordVerificationResult.Failed;

        public async Task<JsonWebToken> SignInAsync(string username, string password)
        {
            JsonWebToken result = null;
            try
            {
                var personnelResult = await _personnelRepository.GetAsync(x => x.Email == username);
                if (!personnelResult.Result || !ValidatePassword(personnelResult.Response, password, _passwordHasher))
                {
                    _logger.LogError("Could not login {@userResult} {username} {password}", personnelResult, username, password);
                    throw new SysException(Codes.InvalidCredentials, "Invalid credentials.");
                }

                var model = personnelResult.Response;
                model.LastLoginTime = DateTime.Now;

                await _personnelRepository.UpdateAsync(model);

                var refreshToken = new RefreshToken(model, _passwordHasher);
                //TODO: Add calims to user
                var claims = new Dictionary<string, string>();
                var jwt = _jwtHandler.CreateToken(model.PersonnelId.ToString("N"), model.PersonnelRole.ToString(), claims);
                jwt.RefreshToken = refreshToken.Token;

                await _tokenRepository.AddAsync(refreshToken);

                result = jwt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not sign in user {@username}", username);
            }

            return result;
        }
        public async Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            try
            {
                var personnelResult = await _personnelRepository.GetAsync(x => x.PersonnelId == userId);
                if (!personnelResult.Result)
                {
                    throw new SysException(Codes.UserNotFound,  $"User with id: '{userId}' was not found.");
                }
               
                if (!ValidatePassword(personnelResult.Response, currentPassword, _passwordHasher))
                {
                    throw new SysException(Codes.InvalidCurrentPassword,  "Invalid current password.");
                }

                var model = personnelResult.Response;

                model.PasswordHash = _passwordHasher.HashPassword(model, newPassword);
               
                await _personnelRepository.UpdateAsync(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not change user password {@userId}", userId);
            }
        }


        // Tokens
        public async Task AddRefreshTokenAsync(Guid userId)
        {
            var userResult = await _personnelRepository.GetAsync(x => x.PersonnelId == userId);
            if (!userResult.Result)
            {
                throw new Exception($"User: '{userId}' was not found.");
            }
            await _tokenRepository.AddAsync(new RefreshToken(userResult.Response, _passwordHasher));
        }

        public async Task<JsonWebToken> CreateAccessTokenAsync(string token)
        {
            var refreshToken = await _tokenRepository.GetAsync(token);
            if (refreshToken == null)
            {
                throw new SysException(Codes.RefreshTokenNotFound,
                    "Refresh token was not found.");
            }
            if (refreshToken.Revoked)
            {
                throw new SysException(Codes.RefreshTokenAlreadyRevoked,
                    $"Refresh token: '{refreshToken.Id}' was revoked.");
            }
            var userResult = await _personnelRepository.GetAsync(x => x.PersonnelId == refreshToken.UserId);
            if (!userResult.Result)
            {
                throw new SysException(Codes.UserNotFound,
                    $"User: '{refreshToken.UserId}' was not found.");
            }

            //TODO: Add calims to user
            var claims = new Dictionary<string, string>();
            var jwt = _jwtHandler.CreateToken(userResult.Response.PersonnelId.ToString("N"), userResult.Response.PersonnelRole.ToString(), claims);
            jwt.RefreshToken = refreshToken.Token;

            return jwt;
        }

        public async Task RevokeRefreshTokenAsync(string token, Guid userId)
        {
            var refreshToken = await _tokenRepository.GetAsync(token);
            if (refreshToken == null || refreshToken.UserId != userId)
            {
                throw new SysException(Codes.RefreshTokenNotFound,
                    "Refresh token was not found.");
            }
            refreshToken.Revoke();
            await _tokenRepository.UpdateAsync(refreshToken);
        }

    }
}
