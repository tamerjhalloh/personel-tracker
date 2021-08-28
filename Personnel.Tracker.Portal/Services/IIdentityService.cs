
using Personnel.Tracker.Common.Authentication;
using Personnel.Tracker.Model.Action;
using Personnel.Tracker.Model.Auth;
using RestEase;
using System.Threading.Tasks;

namespace Personnel.Tracker.Portal.Services
{
    public interface IIdentityService
    {
        [AllowAnyStatusCode]
        [Post("sign-in")]
        Task<JsonWebToken> SignIn([Body] SignIn model);

        [AllowAnyStatusCode]
        [Get("me")]
        Task<Model.Personnel.Personnel> Me([Header("Authorization")] string token); 

        [AllowAnyStatusCode]
        [Get("access-tokens/refresh/{refreshToken}")]
        Task<Response<JsonWebToken>> RefreshToken([Path] string refreshToken); 

        [AllowAnyStatusCode]
        [Put("me/password")]
        Task<OperationResult> ChangePassword([Body] Query<ChangePassword> model);
    }
}
