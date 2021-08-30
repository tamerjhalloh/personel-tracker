using Microsoft.EntityFrameworkCore;
using Personnel.Tracker.Model.Action;
using Personnel.Tracker.Model.Base;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Personnel.Tracker.Common.Sql.Repository
{
    public class BaseRepository<TEntity, TContext> : IBaseRepository<TEntity>
        where TEntity : class, IEntity, new()
        where TContext : DbContext, new()
    {

        private readonly SqlExecptionHandler _sqlExecptionHandler;
        private readonly TContext _context;
        public BaseRepository(SqlExecptionHandler sqlExecptionHandler, TContext context)
        {
            _sqlExecptionHandler = sqlExecptionHandler;
            _context = context;
        }

        public async Task<OperationResult<TEntity>> AddAsync(TEntity entity)
        {
            OperationResult<TEntity> result = new OperationResult<TEntity>();
            try
            {

                var addedEntity = _context.Entry(entity);
                addedEntity.State = EntityState.Added;
                await _context.SaveChangesAsync();
                await _context.Entry(entity).ReloadAsync();
                _context.Entry(entity).State = EntityState.Detached;
                result.Response = entity;
                result.Result = true;

            }
            catch (Exception ex)
            {
                _sqlExecptionHandler.HandelException(ex, result); 
            }
            return result;
        }

        public async Task<OperationResult<TEntity>> DeleteAsync(TEntity entity)
        {
            OperationResult<TEntity> result = new OperationResult<TEntity>();
            try
            {

                var deletedEntity = _context.Entry(entity);
                deletedEntity.State = EntityState.Deleted;
                await _context.SaveChangesAsync();
                result.Response = entity;
                result.Result = true;
            }
            catch (Exception ex)
            {
                _sqlExecptionHandler.HandelException(ex, result);
            }
            return result;
        }

        public async Task<OperationResult<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter, string includeExpression = null, Query query = null)
        {
            OperationResult<TEntity> result = new OperationResult<TEntity>();
            try
            {
                IQueryable<TEntity> sqlQuery = _context.Set<TEntity>().Where(filter);
               

               
                if (!string.IsNullOrEmpty(includeExpression))
                {
                    var includes = includeExpression.Split(';');
                    foreach (string include in includes)
                    {
                        if (!string.IsNullOrEmpty(include))
                            sqlQuery = sqlQuery.Include(include);
                    }
                }

                result.Message = sqlQuery.ToSql();
                if(query != null)
                {
                    sqlQuery = sqlQuery.OrderBy(query);
                }
                result.Response = await sqlQuery.FirstOrDefaultAsync();
                _context.Entry(result.Response).State = EntityState.Detached;

                if (result.Response == null)
                {
                    result.Result = false;
                    result.ErrorMessage = ErrorCode.NotFound;
                    result.ErrorCode = ErrorCode.NotFound;
                    result.ErrorCategory = ErrorCategory.Error;

                }
                else
                {
                    result.Result = true;
                }
            }
            catch (Exception ex)
            {
                _sqlExecptionHandler.HandelException(ex, result);
            }
            return result;
        }

        public async Task<PaggedOperationResult<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter = null,
            Query query = null,
            string includeExpression = null)
        {
            PaggedOperationResult<TEntity> result = new PaggedOperationResult<TEntity>();
            try
            {
                if (query == null)
                    query = new Query();
                 
                IQueryable<TEntity> queryable;
                if (filter == null)
                {
                    queryable = _context.Set<TEntity>().Select(a => a);
                }
                else
                {
                    queryable = _context.Set<TEntity>().Where(filter);
                }

                if (!string.IsNullOrEmpty(includeExpression))
                {
                    var includes = includeExpression.Split(';');
                    foreach (string include in includes)
                    {
                        if (!string.IsNullOrEmpty(include))
                            queryable = queryable.Include(include);
                    }
                }

                result.Message = queryable.ToSql();

                result.TotalCount = await queryable.CountAsync();
                result.Response = await queryable.OrderBy(query).Page(query.PageSize,query.PageIndex).AsNoTracking().ToListAsync();
                result.PageIndex = query.PageIndex;
                result.PageSize = query.PageSize; 
                result.Result = true;
            }
            catch (Exception ex)
            {
                _sqlExecptionHandler.HandelException(ex, result);
            }

            return result;
        }

        public async Task<OperationResult<TEntity>> UpdateAsync(TEntity entity)
        {
            OperationResult<TEntity> result = new OperationResult<TEntity>();
            try
            {
                var updatedEntity = _context.Entry(entity);
                updatedEntity.State = EntityState.Modified;
                await _context.SaveChangesAsync();
                 
                await _context.Entry(entity).ReloadAsync();
                _context.Entry(entity).State = EntityState.Detached;

                result.Response = entity;
                result.Result = true;
            }
            catch (Exception ex)
            {
                _sqlExecptionHandler.HandelException(ex, result);
            }
            return result;

        }
    }
}
