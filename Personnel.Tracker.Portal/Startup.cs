using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Personnel.Tracker.Common;
using Personnel.Tracker.Common.RestEase;
using Personnel.Tracker.Portal.Services;

namespace Personnel.Tracker.Portal
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
            var builder = services.AddControllersWithViews(); 

            #if DEBUG
            builder.AddRazorRuntimeCompilation();
            #endif

            builder.AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null); 

            services.AddCustomMvc();
            services.AddDistributedMemoryCache();

            services.AddAuthentication("CookieAuth")
                 .AddCookie("CookieAuth", config =>
                 {
                     config.Cookie.Name = "_Personnel_";
                     config.LoginPath = "/member/login";

                 });

            services.RegisterServiceForwarder<IIdentityService>("rest-api");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=member}/{action=login}/{id?}");
            });
        }
    }
}
