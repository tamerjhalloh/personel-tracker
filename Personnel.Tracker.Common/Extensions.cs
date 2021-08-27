using Microsoft.Extensions.Configuration;
using Personnel.Tracker.Model.Action;
using Personnel.Tracker.Model.Base;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Personnel.Tracker.Common
{
    public static class Extensions
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> objects, Query query)
        {
            if (string.IsNullOrWhiteSpace(query.OrderBy))
                query.OrderBy = "CreationTime";
            if (!query.OrderBy.Contains("."))
            {
                var props = typeof(T).GetProperties();
                if (!props.Any(p => p.Name.ToLower().Equals(query.OrderBy, System.StringComparison.OrdinalIgnoreCase)))
                {
                    if (props.Any(p => p.Name.ToLower().Equals("CreationTime".ToLower())))
                    {
                        query.OrderBy = "CreationTime";
                    }
                    else
                    {
                        query.OrderBy = typeof(T).Name + "Id";
                    }
                }
            }

            if (!Enum.IsDefined(typeof(OrderDir), query.OrderDir))
            {
                query.OrderDir = OrderDir.DESC;
            }
            if (query.OrderDir == OrderDir.DESC)
                return objects == null ? null : objects.OrderByDesc($"{query.OrderBy}");
            else
                return objects == null ? null : objects.OrderBy($"{query.OrderBy}");
        }

        public static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> query, string propertyName)
        {
            var entityType = typeof(TSource);

            //Create x=>x.PropName
            var propertyInfo = entityType.GetProperty(propertyName);
            ParameterExpression arg = Expression.Parameter(entityType, "x");
            MemberExpression property = Expression.Property(arg, propertyName);
            var selector = Expression.Lambda(property, new ParameterExpression[] { arg });

            //Get System.Linq.Queryable.OrderBy() method.
            var enumarableType = typeof(System.Linq.Queryable);
            var method = enumarableType.GetMethods()
                 .Where(m => m.Name == "OrderBy" && m.IsGenericMethodDefinition)
                 .Where(m =>
                 {
                     var parameters = m.GetParameters().ToList();
                 //Put more restriction here to ensure selecting the right overload                
                 return parameters.Count == 2;//overload that has 2 parameters
                 }).Single();
            //The linq's OrderBy<TSource, TKey> has two generic types, which provided here
            MethodInfo genericMethod = method
                 .MakeGenericMethod(entityType, propertyInfo.PropertyType);

            /*Call query.OrderBy(selector), with query and selector: x=> x.PropName
              Note that we pass the selector as Expression to the method and we don't compile it.
              By doing so EF can extract "order by" columns and generate SQL for it.*/
            var newQuery = (IOrderedQueryable<TSource>)genericMethod
                 .Invoke(genericMethod, new object[] { query, selector });
            return newQuery;
        }

        public static IOrderedQueryable<TSource> OrderByDesc<TSource>(this IQueryable<TSource> query, string propertyName)
        {
            var entityType = typeof(TSource);

            //Create x=>x.PropName
            var propertyInfo = entityType.GetProperty(propertyName);
            ParameterExpression arg = Expression.Parameter(entityType, "x");
            MemberExpression property = Expression.Property(arg, propertyName);
            var selector = Expression.Lambda(property, new ParameterExpression[] { arg });

            //Get System.Linq.Queryable.OrderBy() method.
            var enumarableType = typeof(System.Linq.Queryable);
            var method = enumarableType.GetMethods()
                 .Where(m => m.Name == "OrderByDescending" && m.IsGenericMethodDefinition)
                 .Where(m =>
                 {
                     var parameters = m.GetParameters().ToList();
                 //Put more restriction here to ensure selecting the right overload                
                 return parameters.Count == 2;//overload that has 2 parameters
                 }).Single();
            //The linq's OrderBy<TSource, TKey> has two generic types, which provided here
            MethodInfo genericMethod = method
                 .MakeGenericMethod(entityType, propertyInfo.PropertyType);

            /*Call query.OrderBy(selector), with query and selector: x=> x.PropName
              Note that we pass the selector as Expression to the method and we don't compile it.
              By doing so EF can extract "order by" columns and generate SQL for it.*/
            var newQuery = (IOrderedQueryable<TSource>)genericMethod
                 .Invoke(genericMethod, new object[] { query, selector });
            return newQuery;
        }

         

        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            // need to detect whether they use the same
            // parameter instance; if not, they need fixing
            ParameterExpression param = expr1.Parameters[0];
            if (ReferenceEquals(param, expr2.Parameters[0]))
            {
                // simple version
                return Expression.Lambda<Func<T, bool>>(
                    Expression.AndAlso(expr1.Body, expr2.Body), param);
            }
            // otherwise, keep expr1 "as is" and invoke expr2
            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(
                    expr1.Body,
                    Expression.Invoke(expr2, param)), param);
        }

        public static IQueryable<T> Page<T>(this IQueryable<T> objects, int pageSize, int page)
        {
            if (pageSize < 1)
                pageSize = 10;

            if (page < 0)
                page = 0;

            return objects == null ? null : objects.Skip(page * pageSize).Take(pageSize);
        }


        public static TModel GetOptions<TModel>(this IConfiguration configuration, string section) where TModel : new()
        {
            var model = new TModel();
            configuration.GetSection(section).Bind(model);

            return model;
        }



        public static T Bind<T>(this T model, Expression<Func<T, object>> expression, object value)
          => model.Bind<T, object>(expression, value);

        private static TModel Bind<TModel, TProperty>(this TModel model, Expression<Func<TModel, TProperty>> expression,
         object value)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
            {
                memberExpression = ((UnaryExpression)expression.Body).Operand as MemberExpression;
            }

            var propertyName = memberExpression.Member.Name.ToLowerInvariant();
            var modelType = model.GetType();
            var field = modelType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .SingleOrDefault(x => x.Name.ToLowerInvariant().StartsWith($"<{propertyName}>"));
            if (field == null)
            {
                if (modelType.BaseType != null)
                {
                    field = modelType.BaseType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                   .SingleOrDefault(x => x.Name.ToLowerInvariant().StartsWith($"<{propertyName}>"));
                    if (field == null && modelType.BaseType.BaseType != null)
                    {
                        field = modelType.BaseType.BaseType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                            .SingleOrDefault(x => x.Name.ToLowerInvariant().StartsWith($"<{propertyName}>"));
                    }
                }
            }

            if (field == null)
            {
                return model;
            }

            field.SetValue(model, value);

            return model;
        }
    }
}
