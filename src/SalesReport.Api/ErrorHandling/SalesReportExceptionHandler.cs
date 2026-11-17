using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SalesReport.Api.ErrorHandling;

public class SalesReportExceptionHandler(ILogger<SalesReportExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "The sales report could not be generated.");

        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "The sales report could not be generated.",
        };

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problem, options: null, contentType: "application/problem+json", cancellationToken);

        return true;
    }
}
