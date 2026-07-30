using OptimalCoder.Blueprint.API.Validation;
using OptimalCoder.Blueprint.Infra.Logger;
using OptimalCoder.Blueprint.Shared.Exceptions;
using System.Net;

namespace OptimalCoder.Blueprint.API.Exceptions
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IOptimalLogger logger)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var response = ex switch
            {
                ValidationException => CreateErrorResponse(
                    StatusCodes.Status400BadRequest,
                    ex.Message),

                NotFoundException => CreateErrorResponse(
                    StatusCodes.Status404NotFound,
                    ex.Message),

                UnauthorizedAccessException => CreateErrorResponse(
                    StatusCodes.Status401Unauthorized,
                    ex.Message),

                _ => CreateErrorResponse(
                    StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred.")
            };

            context.Response.StatusCode = response.Status;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(response);
        }

        private static ErrorResponse CreateErrorResponse(
        int status,
        string message)
        {
            return new ErrorResponse
            {
                Status = status,
                Success = false,
                Message = message
        
            };
        }
    }
}
