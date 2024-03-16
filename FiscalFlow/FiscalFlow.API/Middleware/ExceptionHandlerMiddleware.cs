using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FiscalFlow.API.Middleware;

internal class ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
{
    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An exception occured: {Message}", e.Message);

            await RespondToExceptionAsync(httpContext, HttpStatusCode.InternalServerError,
                e.Message + ' ' + e.InnerException?.Message, e);
        }
    }

    private static Task RespondToExceptionAsync(HttpContext context, HttpStatusCode failureStatusCode, string message,
        Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)failureStatusCode;

        var response = new
        {
            Message = message,
            Error = exception.GetType().Name,
            Timestamp = DateTime.UtcNow
        };
        return context.Response.WriteAsync(JsonConvert.SerializeObject(response,
            new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
    }
}