using Microsoft.AspNetCore.Diagnostics;

namespace PaymentService.Helpers.ExceptionHandler
{
    public class AppExceptionHandler(ILogger<AppExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if(exception is not NotImplementedException)
            {
                logger.LogError(exception.Message);
                var response = new ExceptionErrorResponse
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
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
