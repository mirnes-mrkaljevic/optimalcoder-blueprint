using FluentValidation;
using OptimalCoder.Blueprint.API.Validation;
using OptimalCoder.Blueprint.IAM.Authentication;
using OptimalCoder.Blueprint.Infra.Logger;
using OptimalCoder.Blueprint.Shared.Exceptions;

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
                ValidationException validationEx => CreateErrorResponse(
                    StatusCodes.Status400BadRequest,
                    "VALIDATION_FAILED",
                    ex.Message),

                NotFoundException => CreateErrorResponse(
                    StatusCodes.Status404NotFound,
                     "NOT_FOUND",
                    ex.Message),

                UnauthorizedException unauthorizedEx => CreateErrorResponse(
                    StatusCodes.Status401Unauthorized,
                    unauthorizedEx.Code,
                    ex.Message),

                _ => CreateErrorResponse(
                    StatusCodes.Status500InternalServerError,
                    "INTERNAL_ERROR",
                    "An unexpected error occurred.")
            };

            context.Response.StatusCode = response.Status;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(response);
        }

        private static ErrorResponse CreateErrorResponse(
        int status,
        string code,
        string message)
        {
            return new ErrorResponse
            {
                Status = status,
                Success = false,
                Code = code,
                Message = message
        
            };
        }
    }
}
