using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
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


        public static IMvcCoreBuilder AddCustomMvc(this IServiceCollection services)
        {
            using (var serviceProvider = services.BuildServiceProvider())
            {
                var configuration = serviceProvider.GetService<IConfiguration>();
                services.Configure<MapOptions>(configuration.GetSection("app"));
            }
             
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return services
                .AddMvcCore()
                .AddNewtonsoftJson(o =>
                {
                    o.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    o.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    o.SerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
                    o.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
                    o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    o.SerializerSettings.Formatting = Formatting.Indented;
                    o.SerializerSettings.Converters.Add(new StringEnumConverter());
                })
                .AddDataAnnotations()
                .AddApiExplorer() 
                .AddAuthorization();

        }



        public static bool EqualsInsensitive(this string value, string target)
        {
            return value != null && value.Equals(target, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks if string is not null, not empty and has not only spaces 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNotEmpty(this string value)
        {
            return !string.IsNullOrEmpty(value) && value.Trim() != string.Empty;
        }


        /// <summary>
        /// Checks if string is null, empty or has only spaces 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrEmpty(value) || value.Trim() == string.Empty;
        }


        public static void PrepareExceptionResult(this OperationResult result, Exception ex, string errorMessage = "")
        {
            result.Result = false;
            result.SysErrorMessage = ex.Message;
            result.ErrorCategory = ErrorCategory.Exception;
            result.ErrorCode = ErrorCode.Exception;
            result.ErrorMessage = errorMessage.IsEmpty() ? result.ErrorMessage : errorMessage;

        }
        public static void PrepareExceptionResult<T>(this OperationResult<T> result, Exception ex, string errorMessage = "")
        {
            result.Result = false;
            result.SysErrorMessage = ex.Message;
            result.ErrorCategory = ErrorCategory.Exception;
            result.ErrorCode = ErrorCode.Exception;
            result.ErrorMessage = errorMessage.IsEmpty() ? result.ErrorMessage : errorMessage;

        }
        public static void PrepareExceptionResult<T>(this PaggedOperationResult<T> result, Exception ex, string errorMessage = "")
        {
            result.Result = false;
            result.SysErrorMessage = ex.Message;
            result.ErrorCategory = ErrorCategory.Exception;
            result.ErrorCode = ErrorCode.Exception;
            result.ErrorMessage = errorMessage.IsEmpty() ? result.ErrorMessage : errorMessage;

        }

        public static void PrepareMissingParameterResult(this OperationResult result, string missingParameter)
        {
            result.ErrorMessage = missingParameter;
            result.ErrorCategory = ErrorCategory.Validation;
            result.ErrorCode = ErrorCode.MissingParameter;
            result.Result = false;

        }
        public static void PrepareMissingParameterResult<T>(this OperationResult<T> result, string missingParameter)
        {
            result.ErrorMessage = missingParameter;
            result.ErrorCategory = ErrorCategory.Validation;
            result.ErrorCode = ErrorCode.MissingParameter;
            result.Result = false;

        }

        public static void PrepareMissingParameterResult<T>(this PaggedOperationResult<T> result, string missingParameter)
        {
            result.ErrorMessage = missingParameter;
            result.ErrorCategory = ErrorCategory.Validation;
            result.ErrorCode = ErrorCode.MissingParameter;
            result.Result = false;

        }

        public static void PrepareNotFoundResult(this OperationResult result, string errorMessage = "")
        {
            result.ErrorMessage = errorMessage.IsEmpty() ? result.ErrorMessage : errorMessage;
            result.ErrorCategory = ErrorCategory.Validation;
            result.ErrorCode = ErrorCode.NotFound;
            result.Result = false;

        }

        public static void PrepareNotFoundResult<T>(this OperationResult<T> result, string errorMessage = "")
        {
            result.ErrorMessage = errorMessage.IsEmpty() ? result.ErrorMessage : errorMessage;
            result.ErrorCategory = ErrorCategory.Validation;
            result.ErrorCode = ErrorCode.NotFound;
            result.Result = false;

        }
        public static void PrepareNotFoundResult<T>(this PaggedOperationResult<T> result, string errorMessage = "")
        {
            result.ErrorMessage = errorMessage.IsEmpty() ? result.ErrorMessage : errorMessage;
            result.ErrorCategory = ErrorCategory.Validation;
            result.ErrorCode = ErrorCode.NotFound;
            result.Result = false;

        }

        public static void PrepareInvalidParameterResult<T>(this OperationResult<T> result, string parameter, string errorMessage)
        {
            result.ErrorMessage = $"{parameter} - {errorMessage}";
            result.ErrorCategory = ErrorCategory.Validation;
            result.ErrorCode = ErrorCode.Validation;
            result.Result = false;

        }

        public static void PrepareInvalidParameterResult<T>(this PaggedOperationResult<T> result, string parameter, string errorMessage)
        {
            result.ErrorMessage = $"{parameter} - {errorMessage}";
            result.ErrorCategory = ErrorCategory.Validation;
            result.ErrorCode = ErrorCode.Validation;
            result.Result = false;

        }


        public static void PrepareExistedPropertyResult<T>(this OperationResult<T> result, string errorMessage = "")
        {
            result.ErrorMessage = errorMessage.IsEmpty() ? result.ErrorMessage : errorMessage;
            result.ErrorCategory = ErrorCategory.Validation;
            result.ErrorCode = ErrorCode.ExistsBefore;
            result.Result = false;

        }
        public static void PrepareExistedPropertyResult<T>(this PaggedOperationResult<T> result, string errorMessage = "")
        {
            result.ErrorMessage = errorMessage.IsEmpty() ? result.ErrorMessage : errorMessage;
            result.ErrorCategory = ErrorCategory.Validation;
            result.ErrorCode = ErrorCode.ExistsBefore;
            result.Result = false;

        }


        public static PaggedOperationResult<T> Clone<T, S>(this PaggedOperationResult<T> result, PaggedOperationResult<S> source)
        {
            result.ErrorMessage = source.ErrorMessage;
            result.ErrorCategory = source.ErrorCategory;
            result.ErrorCode = source.ErrorCode;
            result.Result = source.Result;
            result.TotalCount = source.TotalCount;
            result.PageIndex = source.PageIndex;
            result.PageSize = source.PageSize;
            return result;
        }
        public static PaggedOperationResult<T> Clone<T>(this PaggedOperationResult<T> result, PaggedOperationResult<T> source)
        {
            result.ErrorMessage = source.ErrorMessage;
            result.ErrorCategory = source.ErrorCategory;
            result.ErrorCode = source.ErrorCode;
            result.Result = source.Result;
            result.TotalCount = source.TotalCount;
            result.PageIndex = source.PageIndex;
            result.PageSize = source.PageSize;
            result.Response = source.Response;
            return result;
        }

    }
}
