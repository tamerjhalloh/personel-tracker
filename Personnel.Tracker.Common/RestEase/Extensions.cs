using System;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestEase;

namespace Personnel.Tracker.Common.RestEase
{
    public static class Extensions
    {
        public static void RegisterServiceForwarder<T>(this IServiceCollection services, string serviceName, int timeoutMinutes = 3)
            where T : class
        {

            RequestHandler handler;
            using (var serviceProvider = services.BuildServiceProvider())
            {
                handler = serviceProvider.GetService<RequestHandler>();
            }
            if(handler == null) 
                services.AddTransient<RequestHandler>();

            var clientName = typeof(T).ToString();
            var options = ConfigureOptions(services);
            switch (options.LoadBalancer?.ToLowerInvariant())
            { 
                default:
                    ConfigureDefaultClient(services, clientName, serviceName, options, timeoutMinutes);
                    break;
            }

            ConfigureForwarder<T>(services, clientName);
        }

        private static RestEaseOptions ConfigureOptions(IServiceCollection services)
        {
            IConfiguration configuration;
            using (var serviceProvider = services.BuildServiceProvider())
            {
                configuration = serviceProvider.GetService<IConfiguration>();
            }

           




            services.Configure<RestEaseOptions>(configuration.GetSection("restEase"));

            return configuration.GetOptions<RestEaseOptions>("restEase");
        }

      
        private static void ConfigureDefaultClient(IServiceCollection services, string clientName,
            string serviceName, RestEaseOptions options, int timeoutMinutes = 3)
        {

            
            services.AddHttpClient(clientName, client =>
            {
                var service = options.Services.FirstOrDefault(s => s.Name.Equals(serviceName,
                    StringComparison.InvariantCultureIgnoreCase));
                if (service == null)
                {
                    using (var serviceProvider = services.BuildServiceProvider())
                    {
                      var  logger = serviceProvider.GetService<ILogger<RestEaseOptions>>();
                        if(logger != null)
                        {
                            logger.LogError($"RestEase service: {serviceName} was not found.", serviceName);
                            return;
                        }
                           
                    } 
                    
                   // throw new Exception($"RestEase service: {serviceName} was not found.");
                   
                }

                client.BaseAddress = new UriBuilder
                {
                    Scheme = service.Scheme,
                    Host = service.Host,
                    Port = service.Port,
                    Path = service.Path
                    
                }.Uri;


                client.Timeout = timeoutMinutes > 0 ?  new TimeSpan(0, timeoutMinutes, 0) : new TimeSpan(0, 30, 0);

                //IConfiguration configuration;
                //using (var serviceProvider = services.BuildServiceProvider())
                //{
                //    configuration = serviceProvider.GetService<IConfiguration>();
                //}

                IHttpContextAccessor _httpContextAccessor;
                using (var serviceProvider = services.BuildServiceProvider())
                {
                    _httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
                }
                if (_httpContextAccessor != null && _httpContextAccessor.HttpContext  != null
                        &&  _httpContextAccessor.HttpContext.Request != null
                        && _httpContextAccessor.HttpContext.Request.Cookies != null
                        && _httpContextAccessor.HttpContext.Request.Cookies.ContainsKey("_accesstoken"))
                {
                    string token = _httpContextAccessor.HttpContext.Items["_accesstoken"] as string;
                    var auth = token == null ? _httpContextAccessor.HttpContext.Request.Cookies["_accesstoken"]
                    : token;//.TryGetValue("Authorization", auth);
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth);
//                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {auth}");
                }
                else
                {
                    // Get Token 

                    IAuthorizationHandler handler;
                      using (var serviceProvider = services.BuildServiceProvider())
                    {
                        handler = serviceProvider.GetService<IAuthorizationHandler>();
                    }
                      if(handler != null)
                    {
                        var getTokenResult = handler.GetWebToken().Result;
                        if (getTokenResult.Result)
                        {
                            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {getTokenResult.Response.AccessToken}");
                        }
                        
                    }
                }



            }).AddHttpMessageHandler<RequestHandler>(); 
        }

        private static void ConfigureForwarder<T>(IServiceCollection services, string clientName) where T : class
        {
            services.AddTransient<T>(c =>
            new RestClient(c.GetService<IHttpClientFactory>().CreateClient(clientName))
            {
                RequestQueryParamSerializer = new QueryParamSerializer()
            }.For<T>()); 
         

          
        }

       
    }
}
