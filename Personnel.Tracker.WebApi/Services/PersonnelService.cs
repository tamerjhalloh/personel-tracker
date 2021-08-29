using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Personnel.Tracker.Common;
using Personnel.Tracker.Model.Action;
using Personnel.Tracker.Model.Criteria;
using Personnel.Tracker.WebApi.Repositories;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Personnel.Tracker.WebApi.Services
{
    public class PersonnelService : IPersonnelService
    {

        private readonly ILogger<PersonnelService> _logger;
        private readonly IPersonnelRepository _personnelRepository;

        public PersonnelService(ILogger<PersonnelService> logger, IPersonnelRepository personnelRepository)
        {
            _logger = logger;
            _personnelRepository = personnelRepository;
        }


        public async Task<PaggedOperationResult<Model.Personnel.Personnel>> SearchPersonnel(Query<SearchPersonnelCriteria> query)
        {
            PaggedOperationResult<Model.Personnel.Personnel> result = new PaggedOperationResult<Model.Personnel.Personnel>();
            try
            {
                Expression<Func<Model.Personnel.Personnel, bool>> filter = r => 1 == 1;
                if (query.Parameter.PersonnelIds != null && query.Parameter.PersonnelIds.Count > 0)
                    filter = filter.AndAlso(r => query.Parameter.PersonnelIds.Contains(r.PersonnelId));

                if (query.Parameter.Search.IsNotEmpty())
                    filter = filter.AndAlso(r => EF.Functions.Like(r.Name, $"%{query.Parameter.Search.Trim().Replace(" ", "%")}%") ||
                                                EF.Functions.Like(r.Surname, $"%{query.Parameter.Search.Trim().Replace(" ", "%")}%"));

                result = await _personnelRepository.GetListAsync(filter, query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SearchPersonnel {@Query}", query);
                result.PrepareExceptionResult(ex);
            }
            return result;
        }
    }
}
