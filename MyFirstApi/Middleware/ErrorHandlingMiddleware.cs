using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;

namespace MyFirstApi.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var error = new { message = "An unexpected error occured.", detail = ex.Message };

                var json = JsonSerializer.Serialize(error);
                await context.Response.WriteAsync(json);
            }
        }
    }

    public static class ErrorHandlingExtenstions
    {
        public static IApplicationBuilder UseGlobalErrorHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}
