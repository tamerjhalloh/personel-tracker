using Personnel.Tracker.Model.Action;
using Personnel.Tracker.Model.Criteria;
using System.Threading.Tasks;

namespace Personnel.Tracker.WebApi.Services
{
    public interface IPersonnelService
    {
        Task<PaggedOperationResult<Model.Personnel.Personnel>> SearchPersonnel(Query<SearchPersonnelCriteria> query);
    }
}
