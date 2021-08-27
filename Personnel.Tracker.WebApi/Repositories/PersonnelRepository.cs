using Personnel.Tracker.Common.Sql;
using Personnel.Tracker.Common.Sql.Repository;
using Personnel.Tracker.WebApi.Contexts;

namespace Personnel.Tracker.WebApi.Repositories
{
    public class PersonnelRepository : BaseRepository<Model.Personnel.Personnel, PersonnelContext>, IPersonnelRepository
    {
        private readonly PersonnelContext _context;
        private readonly SqlExecptionHandler sqlExecptionHandler;
        public PersonnelRepository(PersonnelContext context, SqlExecptionHandler handler) : base(handler, context)
        {
            _context = context;
            sqlExecptionHandler = handler;
        }
    }
}
