using Personnel.Tracker.Model.Action;
using Personnel.Tracker.Model.Criteria;
using System.Threading.Tasks;

namespace Personnel.Tracker.WebApi.Services
{
    public interface IPersonnelService
    {
        Task<OperationResult<Model.Personnel.Personnel>> Add(Query<Model.Personnel.Personnel> query);
        Task<OperationResult<Model.Personnel.Personnel>> Update(Query<Model.Personnel.Personnel> query);
        Task<PaggedOperationResult<Model.Personnel.Personnel>> SearchPersonnel(Query<SearchPersonnelCriteria> query);
        Task<OperationResult<Model.Personnel.Personnel>> Get(Query<Model.Personnel.Personnel> query);
        Task<OperationResult<Model.Personnel.Personnel>> ChangePassword(Query<Model.Personnel.Personnel> query);


    }
}
