using Microsoft.AspNetCore.Identity;
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

        public async Task<OperationResult<Model.Personnel.Personnel>> Add(Query<Model.Personnel.Personnel> query)
        {
            var result = new OperationResult<Model.Personnel.Personnel>();
            try
            {

                if (query.Parameter == null)
                {
                    result.PrepareMissingParameterResult("Personnel");
                    return result;
                }

                if (query.Parameter.Name.IsEmpty())
                {
                    result.PrepareMissingParameterResult("Name");
                    return result;
                }

                if (query.Parameter.Surname.IsEmpty())
                {
                    result.PrepareMissingParameterResult("Surname");
                    return result;
                }


                if (query.Parameter.Email.IsEmpty())
                {
                    result.PrepareMissingParameterResult("Email");
                    return result;
                }


                if (query.Parameter.PasswordHash.IsEmpty())
                {
                    result.PrepareMissingParameterResult("PasswordHash");
                    return result;
                }


                var getPersonnelResult = await _personnelRepository.GetAsync(x => x.Email == query.Parameter.Email);

                if (getPersonnelResult.Result)
                {
                    result.PrepareExistedPropertyResult("Email");
                    result.ErrorMessage = "Personnel with same email exists!";
                    return result;
                }


                var hasher = new PasswordHasher<Model.Personnel.Personnel>();

                query.Parameter.PasswordHash = hasher.HashPassword(query.Parameter, query.Parameter.PasswordHash);

                result = await _personnelRepository.AddAsync(query.Parameter);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Add Personnel {@Query}", query);
                result.PrepareExceptionResult(ex);
            }
            return result;
        }

        public async Task<OperationResult<Model.Personnel.Personnel>> Get(Query<Model.Personnel.Personnel> query)
        {
            var result = new OperationResult<Model.Personnel.Personnel>();
            try
            {

                if (query.Parameter == null)
                {
                    result.PrepareMissingParameterResult("Personnel");
                    return result;
                }

                if (query.Parameter.PersonnelId == Guid.Empty)
                {
                    result.PrepareMissingParameterResult("PersonnelId");
                    return result;
                }



                result = await _personnelRepository.GetAsync(x => x.PersonnelId == query.Parameter.PersonnelId);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get Personnel {@Query}", query);
                result.PrepareExceptionResult(ex);
            }
            return result;
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

        public async Task<OperationResult<Model.Personnel.Personnel>> Update(Query<Model.Personnel.Personnel> query)
        {
            var result = new OperationResult<Model.Personnel.Personnel>();
            try
            {

                if (query.Parameter == null)
                {
                    result.PrepareMissingParameterResult("Personnel");
                    return result;
                }

                if (query.Parameter.PersonnelId == Guid.Empty)
                {
                    result.PrepareMissingParameterResult("PersonnelId");
                    return result;
                }

                if (query.Parameter.Name.IsEmpty())
                {
                    result.PrepareMissingParameterResult("Name");
                    return result;
                }

                if (query.Parameter.Surname.IsEmpty())
                {
                    result.PrepareMissingParameterResult("Surname");
                    return result;
                }


                if (query.Parameter.Email.IsEmpty())
                {
                    result.PrepareMissingParameterResult("Email");
                    return result;
                }


                var getPersonnelResult = await _personnelRepository.GetAsync(x => x.Email == query.Parameter.Email && x.PersonnelId != query.Parameter.PersonnelId);

                if (getPersonnelResult.Result)
                {
                    result.PrepareExistedPropertyResult("Email");
                    result.ErrorMessage = "Personnel with same email exists!";
                    return result;
                }


                var getDbPersonnelResult = await _personnelRepository.GetAsync(x => x.PersonnelId == query.Parameter.PersonnelId);

                if (getDbPersonnelResult.Result)
                {
                    getDbPersonnelResult.Response.Name = query.Parameter.Name;
                    getDbPersonnelResult.Response.Surname = query.Parameter.Surname;
                    getDbPersonnelResult.Response.Email = query.Parameter.Email;
                    getDbPersonnelResult.Response.PersonnelRole = query.Parameter.PersonnelRole;

                    result = await _personnelRepository.UpdateAsync(getDbPersonnelResult.Response);

                }
                else
                {
                    result.PrepareNotFoundResult();
                    result.ErrorMessage = "Personnel was not found!!";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UPDATE Personnel {@Query}", query);
                result.PrepareExceptionResult(ex);
            }
            return result;
        }
    }
}
