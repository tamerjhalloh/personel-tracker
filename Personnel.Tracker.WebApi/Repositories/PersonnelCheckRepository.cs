using Personnel.Tracker.Common.Sql;
using Personnel.Tracker.Common.Sql.Repository;
using Personnel.Tracker.WebApi.Contexts;

namespace Personnel.Tracker.WebApi.Repositories
{
    public class PersonnelCheckRepository : BaseRepository<Model.Personnel.PersonnelCheck, PersonnelContext>, IPersonnelCheckRepository
    {
        private readonly PersonnelContext _context;
        private readonly SqlExecptionHandler sqlExecptionHandler;
        public PersonnelCheckRepository(PersonnelContext context, SqlExecptionHandler handler) : base(handler, context)
        {
            _context = context;
            sqlExecptionHandler = handler;
        }
    }
}
