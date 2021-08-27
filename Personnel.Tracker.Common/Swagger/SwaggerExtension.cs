using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;

namespace Personnel.Tracker.Common.Swagger
{
    public static class SwaggerExtension
    {

        /// <summary>
        /// IMPORTANT NOTE  TO Show summer commits from controllers in the doc
        /// MAKE SURE THE PROJECT HAS the following PropertyGroup section in the csproj file.
        ///  <GenerateDocumentationFile>true</GenerateDocumentationFile>
        ///  
        /// Setting secion should be added to app setting
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static void AddCustomeSwagger(this IServiceCollection services)
        {
            SwaggerOptions options = new SwaggerOptions();
            using (var serviceProvider = services.BuildServiceProvider())
            {
                var configuration = serviceProvider.GetService<IConfiguration>();
                services.Configure<SwaggerOptions>(configuration.GetSection("swagger"));
                options = configuration.GetOptions<SwaggerOptions>("swagger");

            }

            if (!options.Enabled)
            {
                return;
            }


            services.AddSwaggerGen(opt =>
            {
                try
                {



                    opt.SwaggerDoc(options.Name, new OpenApiInfo { Title = options.Title, Version = options.Version });
                    if (options.IncludeSecurity)
                    {
                        opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                        {
                            Description =
                                "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                            Name = "Authorization",
                            In = ParameterLocation.Header,
                            Type = SecuritySchemeType.ApiKey
                        });
                    }


                    //var xmlFile = $"{Extensions.AssemblyIdentifier.GetAssemply().GetName().Name}.xml";
                    //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    //opt.IncludeXmlComments(xmlPath);


                    var xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    if (File.Exists(xmlPath))
                        opt.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
                }
                catch { }
            });
        }
        public static IApplicationBuilder UseCustomeSwagger(this IApplicationBuilder app, string swaggerKey)
        {

            var options = app.ApplicationServices.GetService<IConfiguration>()
              .GetOptions<SwaggerOptions>("swagger");
            if (!options.Enabled)
            {
                return app;
            }

            var routePrefix = string.IsNullOrWhiteSpace(options.RoutePrefix) ? "swagger" : options.RoutePrefix;

            app.UseStaticFiles()
                .UseSwagger(c => c.RouteTemplate = routePrefix + "/{documentName}/swagger.json");

            return options.ReDocEnabled
                ? app.UseReDoc(c =>
                {
                    c.RoutePrefix = routePrefix;
                    c.SpecUrl = $"{options.Name}/swagger.json";
                })
                : app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint($"/{routePrefix}/{options.Name}/swagger.json", options.Title);
                    c.RoutePrefix = routePrefix;
                });
        }

    }
}
