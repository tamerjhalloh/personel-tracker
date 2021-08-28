using Personnel.Tracker.Model.Action;
using Personnel.Tracker.Model.Personnel;
using System.Threading.Tasks;

namespace Personnel.Tracker.WebApi.Services
{
    public interface IPersonnelCheckService
    {
        Task<OperationResult<PersonnelCheck>> GetLastPersonnelCheck(Query<Model.Personnel.Personnel> query);
        Task<OperationResult<PersonnelCheck>> AddAsync(Query<PersonnelCheck> query);
    }
}
