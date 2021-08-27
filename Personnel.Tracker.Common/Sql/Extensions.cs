using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Personnel.Tracker.Common.Sql
{
    public static class Extensions
    {
        public static IServiceCollection AddEFSqlContext<TContext>(this IServiceCollection services, string connection) where TContext : DbContext
        {
            var assemblyName = System.Reflection.Assembly.GetCallingAssembly().FullName;
            IConfiguration configuration;
            using (var serviceProvider = services.BuildServiceProvider())
            {
                configuration = serviceProvider.GetService<IConfiguration>();
            }
            services.AddTransient<SqlExecptionHandler>();
            return services.AddDbContext<TContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString(connection),
                assembly => assembly.MigrationsAssembly(assemblyName));
            }
            , ServiceLifetime.Transient); ;
        }



        public static IApplicationBuilder UpdateDatabase<TContext>(this IApplicationBuilder app) where TContext : DbContext
        {
            try
            {
                using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
                {
                    using (var context = serviceScope.ServiceProvider.GetService<TContext>())
                    {
                        context.Database.Migrate();
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
            };
            
            return app;
        }


        private static object Private(this object obj, string privateField) => obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);
        private static T Private<T>(this object obj, string privateField) => (T)obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);

        public static string ToSql<TEntity>(this IQueryable<TEntity> query) where TEntity : class
        {
            try
            {
                using var enumerator = query.Provider.Execute<IEnumerable<TEntity>>(query.Expression).GetEnumerator();
                var relationalCommandCache = enumerator.Private("_relationalCommandCache");
                var selectExpression = relationalCommandCache.Private<SelectExpression>("_selectExpression");
                var factory = relationalCommandCache.Private<IQuerySqlGeneratorFactory>("_querySqlGeneratorFactory");

                var sqlGenerator = factory.Create();
                var command = sqlGenerator.GetCommand(selectExpression);

                string sql = command.CommandText;
                return sql;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }

    }
}
