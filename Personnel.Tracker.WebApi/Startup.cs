using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Personnel.Tracker.Common.Authentication;
using Personnel.Tracker.Common.Sql;
using Personnel.Tracker.Common.Swagger;
using Personnel.Tracker.WebApi.Contexts;
using Personnel.Tracker.WebApi.Repositories;
using Personnel.Tracker.WebApi.Services;
using System.Linq;

namespace Personnel.Tracker.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(options =>
                   options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                   );

            services.AddDistributedMemoryCache();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddJwt(Configuration);


            services.AddEFSqlContext<PersonnelContext>("db-conn");

            services.AddCustomeSwagger();


            services.AddScoped<IPasswordHasher<Model.Personnel.Personnel>, PasswordHasher<Model.Personnel.Personnel>>();


            // Repositoreis
            services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();

            services.AddTransient<IPersonnelRepository, PersonnelRepository>();
            services.AddTransient<IPersonnelCheckRepository, PersonnelCheckRepository>(); 


            // Services
            services.AddTransient<IIdentityService, IdentityService>();

            services.AddTransient<IPersonnelService, PersonnelService>();
            services.AddTransient<IPersonnelCheckService, PersonnelCheckService>();



            // Enables Cors
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                    builder.SetIsOriginAllowed(_ => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            UpdateDatabase(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCustomeSwagger("Personnel Tracker Api");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static void UpdateDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<PersonnelContext>())
                {
                    context.Database.Migrate();


                    var adminUser = context.Personnels.FirstOrDefault(u => u.Email == "admin@admin.com");
                    if (adminUser == null)
                    {
                        adminUser = new Model.Personnel.Personnel
                        {   
                            Email = "admin@admin.com",
                            PersonnelRole = Model.Base.PersonnelRole.Admin,
                            Name = "Admin",
                            Surname = "User" 
                        };

                        var hasher = new PasswordHasher<Model.Personnel.Personnel>();
                        adminUser.PasswordHash = hasher.HashPassword(adminUser, "admin2021");
                        context.Personnels.Add(adminUser);
                        context.SaveChanges();
                    }

                }
            }
        }
    }
}
