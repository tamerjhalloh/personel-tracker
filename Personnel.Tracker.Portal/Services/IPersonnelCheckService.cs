using Personnel.Tracker.Model.Action;
using Personnel.Tracker.Model.Personnel;
using RestEase;
using System.Threading.Tasks;

namespace Personnel.Tracker.Portal.Services
{
    public interface IPersonnelCheckService : IAuthService
    {
        [AllowAnyStatusCode]
        [Post("personnel-checks/last")]
        Task<OperationResult<PersonnelCheck>> GetLastCheck();

        [AllowAnyStatusCode]
        [Post("personnel-checks/add")]
        Task<OperationResult<PersonnelCheck>> SetPersonnelCheck([Body] Query<PersonnelCheck> query);
    }
}
