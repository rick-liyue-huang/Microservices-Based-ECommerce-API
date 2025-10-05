
using System.Net;
using System.Text.Json;
using eCommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.SharedLibrary.Middlewares;

public class GlobalException(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // declare variables
        string message = "sorry, internal server error occurred, kindly contact system administrator";
        int statusCode = (int)HttpStatusCode.InternalServerError;
        string title = "Error";

        try 
        {
            await next(context);

            // check if Exception is Too many Requests // 429
            if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
            {
                title = "Warning";
                message = "too many requests, please try again later";
                statusCode = (int)HttpStatusCode.TooManyRequests;
                await ModifyHeader(context, title, message, statusCode);
            }

            // if is Unauthorized // 401
            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                title = "Alert";
                message = "you are not authorized to access this resource";
                statusCode = (int)HttpStatusCode.Unauthorized;
                await ModifyHeader(context, title, message, statusCode);
            }

            // if is Forbidden // 403
            if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                title = "Out of Service";
                message = "You are not allowed/required to access this resource";
                statusCode = StatusCodes.Status403Forbidden; // (int)HttpStatusCode.Forbidden;
                await ModifyHeader(context, title, message, statusCode);
            }
        }
        catch (Exception ex)
        { 
            // Log Original Exceptions / Console Log, File Log, Debugger Log
            LogException.LogExceptions(ex);

            // check if Exception is Time out
            if (ex is TaskCanceledException || ex is TimeoutException)
            {
                title = "Out of Service";
                message = "request timed out, please try again later";
                statusCode = StatusCodes.Status408RequestTimeout; //(int)HttpStatusCode.GatewayTimeout;
            }
            // if none of the above, do the default
            // or if Exception is caught, display the default message
            await ModifyHeader(context, title, message, statusCode);
        }
    }

    private async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
    {
        // display scary-free message to client
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails() 
        {
            Detail = message,
            Status = statusCode,
            Title = title
        }), CancellationToken.None);
        return;
    }
}
