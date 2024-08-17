using Microsoft.AspNetCore.Diagnostics;

namespace SendMoneyService.Helpers.ExceptionHandler
{
    public class AppNotImplementedExceptionHandler(ILogger<AppNotImplementedExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is NotImplementedException)
            {
                logger.LogError(exception.Message);
                var response = new ExceptionErrorResponse
                {
                    StatusCode = StatusCodes.Status501NotImplemented,
                    Title = "Something went wrong",
                    ErrorMessage = exception.Message,
                };
                await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
                return true;
            }
            return false;
        }
    }
}
