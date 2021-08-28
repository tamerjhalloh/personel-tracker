using Personnel.Tracker.Common.Sql;
using Personnel.Tracker.Common.Sql.Repository;
using Personnel.Tracker.WebApi.Contexts;
using Personnel.Tracker.Model.Action;
using Personnel.Tracker.Model.Personnel;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using Personnel.Tracker.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Personnel.Tracker.WebApi.Repositories
{
    public class PersonnelCheckRepository : BaseRepository<Model.Personnel.PersonnelCheck, PersonnelContext>, IPersonnelCheckRepository
    {
        private readonly PersonnelContext _context;
        private readonly SqlExecptionHandler sqlExecptionHandler;
        private readonly ILogger<PersonnelCheckRepository> _logger;
        public PersonnelCheckRepository(PersonnelContext context, SqlExecptionHandler handler, ILogger<PersonnelCheckRepository> logger) : base(handler, context)
        {
            _context = context;
            sqlExecptionHandler = handler;
            _logger = logger;
        }

        public async Task<PaggedOperationResult<DayAttendance>> GetPersonnelDayAttencance(Query<PersonnelCheck> query)
        {
            var result = new PaggedOperationResult<DayAttendance>();
            try
            {

                var items = _context.PersonnelChecks.Select(a => a);

                if(query.Parameter.PersonnelId != Guid.Empty)
                {
                    items = items.Where(x => x.PersonnelId == query.Parameter.PersonnelId);
                }

                if (query.Parameter.CreationTime != null && query.Parameter.CreationTime.Value.Year > 2000)
                {
                    items = items.Where(x => x.CreationTime.Value.Date == query.Parameter.CreationTime.Value.Date);
                }


                var results = await items.ToListAsync();

                var attendances = results.GroupBy(x => new { x.PersonnelId, x.CreationTime.Value.Date })
                                        .Select(group => new DayAttendance
                                        {
                                            Date = group.Key.Date,
                                            PersonnelId = group.Key.PersonnelId,
                                            FirstCheckIn = group.Where(x => x.PersonnelCheckType == Model.Base.PersonnelCheckType.In).Min(x => x.CreationTime.Value),
                                            FirstCheckOut = group.Where(x => x.PersonnelCheckType == Model.Base.PersonnelCheckType.Out).Min(x => x.CreationTime.Value),
                                            LastCheckIn = group.Where(x => x.PersonnelCheckType == Model.Base.PersonnelCheckType.In).Max(x => x.CreationTime.Value),
                                            LastCheckOut = group.Where(x => x.PersonnelCheckType == Model.Base.PersonnelCheckType.Out).Max(x => x.CreationTime.Value),
                                            LastCheck = group.Where(x => x.CreationTime == group.Max(y => y.CreationTime)).FirstOrDefault().CreationTime.Value,
                                            LastCheckType = group.Where(x => x.CreationTime == group.Max(y => y.CreationTime)).FirstOrDefault().PersonnelCheckType
                                        }).OrderByDescending(x=> x.Date).ToList();

                //  result.Message = attendances.ToSql();

                //  result.Response = await attendances.ToListAsync();


                result.Response = attendances;

                result.Result = true; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while getting personnel day attendance");
                result.PrepareExceptionResult(ex);
            }

            return result;
        }
    }
}
