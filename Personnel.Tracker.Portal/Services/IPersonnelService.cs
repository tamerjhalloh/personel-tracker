
using Personnel.Tracker.Common.Authentication;
using Personnel.Tracker.Model.Action;
using Personnel.Tracker.Model.Auth;
using Personnel.Tracker.Model.Criteria;
using RestEase;
using System;
using System.Threading.Tasks;

namespace Personnel.Tracker.Portal.Services
{
    public interface IPersonnelService : IAuthService
    {
        [AllowAnyStatusCode]
        [Post("personnels/search")]
        Task<PaggedOperationResult<Model.Personnel.Personnel>> Search([Body] Query<SearchPersonnelCriteria> query);

        [AllowAnyStatusCode]
        [Post("personnels")]
        Task<OperationResult<Model.Personnel.Personnel>> Add([Body] Query<Model.Personnel.Personnel> query);

        [AllowAnyStatusCode]
        [Put("personnels")]
        Task<OperationResult<Model.Personnel.Personnel>> Update([Body] Query<Model.Personnel.Personnel> query);

        [AllowAnyStatusCode]
        [Get("personnels")]
        Task<OperationResult<Model.Personnel.Personnel>> Get([Query] Guid id);

    }
}
