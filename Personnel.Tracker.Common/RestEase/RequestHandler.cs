using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Personnel.Tracker.Model.Action;
using Personnel.Tracker.Model.Base;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Personnel.Tracker.Common.RestEase
{
    public class RequestHandler : DelegatingHandler
    {

        private readonly IHttpContextAccessor _context;
        private readonly ILogger<RequestHandler> _logger;


        public RequestHandler(IHttpContextAccessor contex, ILogger<RequestHandler> logger)
        {
            _context = contex;
            _logger = logger;
        }
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage result = null;
            try
            {


                result = await base.SendAsync(request, cancellationToken);
                if (result == null || result.StatusCode != HttpStatusCode.OK)
                {

                    if (_logger != null)
                    {
                        _logger.LogError("Faild to execute request {Path} {@Request} {@Result} ", request.RequestUri.AbsolutePath, request, result);
                    }
                    if (result == null)
                    {
                        result = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(
                                        new OperationResult() { 
                                            ErrorCode = ErrorCode.Service,
                                            ErrorMessage = "ServiceNotAvilable"                                        
                                        }), System.Text.Encoding.UTF8, "application/json")
                        };
                    }else if(result.StatusCode == HttpStatusCode.NotFound)
                    {
                        result = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(
                                        new OperationResult()
                                        {
                                            ErrorCode = ErrorCode.NotFound,
                                            ErrorMessage = ErrorCode.NotFound
                                        }), System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        result = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(
                                        new OperationResult()
                                        {
                                            ErrorCode = ErrorCode.Exception,
                                            ErrorMessage = ErrorCode.Service,
                                            SysErrorMessage = await result.Content.ReadAsStringAsync()
                                        }), System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                }


            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception {Path}", request.RequestUri.AbsolutePath);
                }

                result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(
                                        new OperationResult()
                                        {
                                            ErrorCode = ErrorCode.Service,
                                            ErrorMessage = "ServiceNotAvilable"
                                        }), System.Text.Encoding.UTF8, "application/json")
                };
            }
            return result;
        }
    }
}
