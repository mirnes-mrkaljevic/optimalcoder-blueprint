using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using OptimalCoder.Blueprint.IAM.Authentication;
using OptimalCoder.Blueprint.Infra.Logger;
using OptimalCoder.Blueprint.Shared.Exceptions;
using Serilog.Core;

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
                await HandleExceptionAsync(httpContext, logger, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, IOptimalLogger logger, Exception ex)
        {
            ErrorResponse response;

            switch (ex)
            {
                case ValidationException validationEx:
                    response = CreateErrorResponse(StatusCodes.Status400BadRequest, "VALIDATION_FAILED", ex.Message);
                    break;

                case NotFoundException notFoundEx:
                    logger.Error(notFoundEx, notFoundEx.Message);
                    response = CreateErrorResponse(StatusCodes.Status404NotFound, "NOT_FOUND", ex.Message);
                    break;
                case UnauthorizedException unauthorizedEx:
                    response = CreateErrorResponse(StatusCodes.Status401Unauthorized, unauthorizedEx.Code, ex.Message);
                    break;
                default:
                    logger.Error(ex, ex.Message);
                    response = CreateErrorResponse(StatusCodes.Status500InternalServerError, "INTERNAL_ERROR", "An unexpected error occurred.");
                    break;

            }
           
            context.Response.StatusCode = response.Status;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(response);
        }

        private static ErrorResponse CreateErrorResponse(int status, string code, string message)
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
