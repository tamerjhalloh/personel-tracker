
using Personnel.Tracker.Common.Authentication;
using Personnel.Tracker.Model.Action;
using Personnel.Tracker.Model.Auth;
using Personnel.Tracker.Model.Criteria;
using RestEase;
using System.Threading.Tasks;

namespace Personnel.Tracker.Portal.Services
{
    public interface IPersonnelService : IAuthService
    {
        [AllowAnyStatusCode]
        [Post("personnels/search")]
        Task<PaggedOperationResult<Model.Personnel.Personnel>> Search([Body] Query<SearchPersonnelCriteria> query);
    }
}
