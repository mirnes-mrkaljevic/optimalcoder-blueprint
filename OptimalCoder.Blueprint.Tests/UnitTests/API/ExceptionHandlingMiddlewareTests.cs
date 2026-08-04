using FluentValidation;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using OptimalCoder.Blueprint.API.Exceptions;
using OptimalCoder.Blueprint.IAM.Authentication;
using OptimalCoder.Blueprint.Infra.Logger;
using OptimalCoder.Blueprint.Shared.Exceptions;
using System.Text.Json;

namespace OptimalCoder.Blueprint.Tests.UnitTests.API
{
    [TestFixture]
    public class ExceptionHandlingMiddlewareTests
    {
        [Test]
        public async Task ExceptionLogged_WhenOccurs()
        {
            var context = new DefaultHttpContext();

            var loggerMock = new Mock<IOptimalLogger>();

            var exceptionText = "Exception Occured!";
            var exception = new Exception(exceptionText);

            RequestDelegate next = _ =>
                throw exception;

            var middleware = new ExceptionHandlingMiddleware(next);

            await middleware.InvokeAsync(context, loggerMock.Object);

            loggerMock.Verify(x => x.Error(exception, exceptionText), Times.Once);
        }

        [Test]
        [TestCase(typeof(ValidationException), StatusCodes.Status400BadRequest, "VALIDATION_FAILED", "Validation failed")]
        [TestCase(typeof(NotFoundException), StatusCodes.Status404NotFound, "NOT_FOUND", "Not found")]
        [TestCase(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized, "UNAUTHORIZED", "Unauthorized")]
        [TestCase(typeof(Exception), StatusCodes.Status500InternalServerError, "INTERNAL_ERROR", "An unexpected error occurred.")]
        public async Task FormattedErrorResponseReturned_WhenExceptionOccurs(Type exceptionType, int returnStatus, string code, string message)
        {
            var context = new DefaultHttpContext();
            var loggerMock = new Mock<IOptimalLogger>();

            RequestDelegate next = exceptionType switch
            {
                var type when type == typeof(ValidationException) => _ => throw new ValidationException(message),
                var type when type == typeof(NotFoundException) => _ => throw new NotFoundException(message),
                var type when type == typeof(UnauthorizedException) => _ => throw new UnauthorizedException(message)
                {
                    Code = code,
                },
                
                _ => _ => throw new Exception(message),

            };

            var middleware = new ExceptionHandlingMiddleware(next);

            context.Response.Body = new MemoryStream();

            await middleware.InvokeAsync(context, loggerMock.Object);

            context.Response.Body.Position = 0;

            using var reader = new StreamReader(context.Response.Body);
            var response = await reader.ReadToEndAsync();

            var errorResponse = new ErrorResponse
            {
                Status = returnStatus,
                Success = false,
                Code = code,
                Message = message
            };

            var errorResponseJson = JsonSerializer.Serialize(
                        errorResponse,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        });

            Assert.That(response, Is.EqualTo(errorResponseJson));

        }
    }

}
