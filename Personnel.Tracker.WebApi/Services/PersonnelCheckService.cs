using Microsoft.Extensions.Logging;
using Personnel.Tracker.Model.Action;
using Personnel.Tracker.Model.Personnel;
using Personnel.Tracker.WebApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Personnel.Tracker.WebApi.Services
{
    public class PersonnelCheckService : IPersonnelCheckService
    {

        private readonly ILogger<PersonnelCheckService> _logger;
        private readonly IPersonnelCheckRepository _personnelCheckRepository;

        public PersonnelCheckService(ILogger<PersonnelCheckService> logger, IPersonnelCheckRepository personnelCheckRepository)
        {
            _logger = logger;
            _personnelCheckRepository = personnelCheckRepository;
        }

        public async Task<OperationResult<PersonnelCheck>> GetLastPersonnelCheck(Query<Model.Personnel.Personnel> query)
        {
            var result = new OperationResult<PersonnelCheck>();
            try
            {
                query.OrderBy = "CreationTime";
                query.OrderDir = Model.Base.OrderDir.DESC;
                query.PageSize = 1;

                var getResult = await _personnelCheckRepository.GetListAsync(x => x.PersonnelId == query.Parameter.PersonnelId, query);

                if (getResult.Result)
                {
                    result.Response = getResult.Response != null && getResult.Response.Any() ? getResult.Response.FirstOrDefault() : null;
                    result.Result = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while getting last personnel check");
            }

            return result;
        }
    }
}
