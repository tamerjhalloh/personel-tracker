using Microsoft.Extensions.Logging;
using Personnel.Tracker.Common;
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
                // Validation
                if (query.Parameter == null)
                {
                    result.PrepareMissingParameterResult("PersonnelCheck");
                    return result;
                }

                if (query.Parameter.PersonnelId == Guid.Empty)
                {
                    result.PrepareMissingParameterResult("PersonnelId");
                    return result;
                }


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
                result.PrepareExceptionResult(ex);
            }

            return result;
        }

        public async Task<OperationResult<PersonnelCheck>> AddAsync(Query<PersonnelCheck> query)
        {
            var result = new OperationResult<PersonnelCheck>();
            try
            {

                // Validation
                if(query.Parameter == null)
                {
                    result.PrepareMissingParameterResult("PersonnelCheck");
                    return result;
                } 

                if (query.Parameter.PersonnelId == Guid.Empty)
                {
                    result.PrepareMissingParameterResult("PersonnelId");
                    return result;
                }

                if (query.Parameter.PersonnelCheckType < 0)
                {
                    result.PrepareMissingParameterResult("PersonnelCheckType");
                    return result;
                }


                var lastCheck = await GetLastPersonnelCheck(new Query<Model.Personnel.Personnel>(new Model.Personnel.Personnel { PersonnelId = query.Parameter.PersonnelId }));

                if(lastCheck.Result)
                {

                    if(lastCheck.Response != null)
                    {
                        if(lastCheck.Response.PersonnelCheckType == query.Parameter.PersonnelCheckType)
                        {
                            result.ErrorMessage = $"Same personnel is doing same action({lastCheck.Response.PersonnelCheckType})";
                        }
                        else
                        {
                            result = await _personnelCheckRepository.AddAsync(query.Parameter);
                        }
                    }
                    else
                    {
                        if(query.Parameter.PersonnelCheckType == Model.Base.PersonnelCheckType.In)
                        {
                            result = await _personnelCheckRepository.AddAsync(query.Parameter);
                        }
                        else
                        {
                            result.ErrorMessage = "Personnel has to check in first";
                        }
                    } 
                }
                else
                {
                    _logger.LogError("Last personnel check coould not be checked up!");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while getting last personnel check");
                result.PrepareExceptionResult(ex);
            }

            return result;
        }

        public async Task<PaggedOperationResult<DayAttendance>> GetPersonnelDayAttencance(Query<PersonnelCheck> query)
        {
            var result = new PaggedOperationResult<DayAttendance>();
            try
            {
                // Validation
                if (query.Parameter == null)
                {
                    result.PrepareMissingParameterResult("PersonnelCheck");
                    return result;
                }

                //if (query.Parameter.PersonnelId == Guid.Empty)
                //{
                //    result.PrepareMissingParameterResult("PersonnelId");
                //    return result;
                //}

                result = await _personnelCheckRepository.GetPersonnelDayAttencance(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while getting last personnel check");
                result.PrepareExceptionResult(ex);
            }

            return result;
        }
    }
}
