using System.Text.Json;
using LCT.Api.Configuration.Models;
using Serilog;
using Serilog.Context;

namespace LCT.Api.Configuration
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _requestDelegate;

        public ErrorHandlingMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        public async Task Invoke(HttpContext context)
        {
            var requestId = Guid.NewGuid();
            using (LogContext.PushProperty("RequestId", requestId))
            {
                var response = context.Response;
                try
                {
                    await _requestDelegate(context);
                }
                catch (Exception ex) 
                {
                    Log.Error(ex, ex.Message);
                    var responseModel = JsonSerializer.Serialize(new ErrorResponseModel(requestId , ex.Message));
                    response.StatusCode = 400;
                    await response.WriteAsync(responseModel);
                }
            }
        }
    }

    public static class ErrorHandlingMiddlewareExtension
    {
        public static IApplicationBuilder UserErrorLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}
