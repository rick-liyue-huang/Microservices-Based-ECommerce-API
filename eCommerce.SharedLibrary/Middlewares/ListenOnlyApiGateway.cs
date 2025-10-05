
using Microsoft.AspNetCore.Http;

namespace eCommerce.SharedLibrary.Middlewares;

public class ListenOnlyApiGateway(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Extract specific header from the request
        var signedHeader = context.Request.Headers["Api-Gateway"];

        // NULL means the request is not coming from the API Gateway / 503 service unavailable
        if (signedHeader.FirstOrDefault() is null)
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsync("sorry, service is not available at the moment");
            return;
        }
        else
        {
            await next(context);
        }
    }
}
