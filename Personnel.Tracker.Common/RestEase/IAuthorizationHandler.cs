using Personnel.Tracker.Common.Authentication;
using Personnel.Tracker.Model.Action;
using System.Threading.Tasks;

namespace Personnel.Tracker.Common.RestEase
{
    public interface IAuthorizationHandler
    {

        Task<OperationResult<JsonWebToken>> GetWebToken();
    }
}
