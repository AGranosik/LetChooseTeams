using Serilog;

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
            try
            {
                await _requestDelegate(context);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                context.Response.StatusCode = 400;
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
