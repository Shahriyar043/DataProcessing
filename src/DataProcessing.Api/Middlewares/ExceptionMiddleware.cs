using DataProcessing.Api.Models;
using RabbitMQ.Client.Exceptions;
using System.Net;
using System.Text.Json;

namespace DataProcessing.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionMiddleware> logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (BrokerUnreachableException bex)
            {
                await LogAndHandleException(context, bex, HttpStatusCode.InternalServerError, "Broker is unreachable");
            }
            catch (FileNotFoundException fex)
            {
                await LogAndHandleException(context, fex, HttpStatusCode.NotFound, "File not found");
            }
            catch (ArgumentNullException anex)
            {
                await LogAndHandleException(context, anex, HttpStatusCode.BadRequest, "Argument is null");
            }
            catch (JsonException jex)
            {
                await LogAndHandleException(context, jex, HttpStatusCode.BadRequest, "Invalid JSON format");
            }
            catch (Exception ex)
            {
                await LogAndHandleException(context, ex, HttpStatusCode.InternalServerError, "An unexpected error occurred");
            }
        }

        private async Task LogAndHandleException(HttpContext context, Exception ex, HttpStatusCode statusCode, string message)
        {
            logger.LogError($"{message}: {ex}");
            await HandleExceptionAsync(context, statusCode, message);
        }

        private async Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var errorDetails = new ErrorDetails
            {
                StatusCode = context.Response.StatusCode,
                Message = message
            };

            var jsonError = JsonSerializer.Serialize(errorDetails);
            await context.Response.WriteAsync(jsonError);
        }
    }
}
