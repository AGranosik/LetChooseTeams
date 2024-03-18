using System.Text.Json;
using LCT.Api.Configuration.Models;
using LCT.Domain.Common.Exceptions;
using LCT.Infrastructure.Common.Exceptions;
using Serilog;
using Serilog.Context;

namespace LCT.Api.Configuration
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private const string _domainErrorMessage = "Domain error occured.";
        private const string _infrastructureErrorMessage = "Infrastructure error occured.";
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
                catch(DomainError ex)
                {
                    Log.Warning(ex, _domainErrorMessage);
                    var responseModel = JsonSerializer.Serialize(new ErrorResponseModel(requestId, _domainErrorMessage));
                    response.StatusCode = 400;
                    await response.WriteAsync(responseModel);
                }
                catch(InfrastructureException ex)
                {
                    Log.Error(ex, _infrastructureErrorMessage);
                    var responseModel = JsonSerializer.Serialize(new ErrorResponseModel(requestId, _infrastructureErrorMessage));
                    response.StatusCode = 400;
                    await response.WriteAsync(responseModel);
                }
                catch (Exception ex) 
                {
                    Log.Error(ex, ex.Message);
                    var responseModel = JsonSerializer.Serialize(new ErrorResponseModel(requestId , ex.Message));
                    response.StatusCode = 500;
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
