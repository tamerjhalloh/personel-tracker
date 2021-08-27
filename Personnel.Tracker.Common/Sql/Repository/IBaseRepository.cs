using Personnel.Tracker.Model.Action;
using Personnel.Tracker.Model.Base;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Personnel.Tracker.Common.Sql.Repository
{
	public interface IBaseRepository<TEntity> where TEntity : class, IEntity, new()
	{
	 
		/// <summary>
		///  To get item list
		/// </summary>
		/// <param name="filter">filter used by framework</param>
		/// <param name="includeExpression">Get nested objects</param>
		/// <returns></returns>
		Task<PaggedOperationResult<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter = null,
			Query query = null,
			string includeExpression = null);

		Task<OperationResult<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter, string includeExpression = null, Query query = null);
		Task<OperationResult<TEntity>> AddAsync(TEntity entity);
		Task<OperationResult<TEntity>> UpdateAsync(TEntity entity);
		Task<OperationResult<TEntity>> DeleteAsync(TEntity entity);
	}
}
