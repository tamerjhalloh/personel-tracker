using RestEase;
using System.Net.Http.Headers;

namespace Personnel.Tracker.Portal.Services
{
    public interface IAuthService
    {
        [Header("Authorization")]
        AuthenticationHeaderValue Authorization { get; set; }
    }
}
