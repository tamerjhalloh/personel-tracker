using Personnel.Tracker.Common.Authentication;
using System;
using System.Threading.Tasks;

namespace Personnel.Tracker.WebApi.Services
{
    public interface IIdentityService
    {
        // Tokens
        Task AddRefreshTokenAsync(Guid userId);
        Task<JsonWebToken> CreateAccessTokenAsync(string refreshToken);
        Task RevokeRefreshTokenAsync(string refreshToken, Guid userId);


        // Login, Password .. etc
        Task<Model.Personnel.Personnel> GetAsync(Guid userId);
        Task<JsonWebToken> SignInAsync(string username, string password);
        Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
     
        
       
    }
}
