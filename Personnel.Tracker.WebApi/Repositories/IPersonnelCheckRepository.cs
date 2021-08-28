using Personnel.Tracker.Common.Sql.Repository;
using Personnel.Tracker.Model.Action;
using Personnel.Tracker.Model.Personnel;
using System.Threading.Tasks;

namespace Personnel.Tracker.WebApi.Repositories
{
    public interface IPersonnelCheckRepository : IBaseRepository<Model.Personnel.PersonnelCheck>
    {
        Task<PaggedOperationResult<DayAttendance>> GetPersonnelDayAttencance(Query<PersonnelCheck> query);
    }
}
