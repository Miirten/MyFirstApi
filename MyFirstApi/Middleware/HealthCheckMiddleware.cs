namespace MyFirstApi.Middleware
{
    public class HealthCheckMiddleware
    {
        private readonly RequestDelegate _next;

        public HealthCheckMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/health"))
            {
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync("Healthy");
                return;
            }
            await _next(context);
        }
    }

    public static class HealthCheckMiddlewareExtensions
    {
        public static IApplicationBuilder UseHealthCheck(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HealthCheckMiddleware>();
        }
    }
}
