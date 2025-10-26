using System.Text.Json;

namespace AdvertisingPlatforms.Middleware
{
    /// <summary>
    /// Класс для глобальной обработки ошибок
    /// </summary>
    public class GlobalErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalErrorHandlerMiddleware> _logger;

        public GlobalErrorHandlerMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                
                await _next(context);
            }
            catch (Exception ex)
            {
                string error = "Произошла непредвиденная ошибка на стороне сервера.";

                _logger.LogError(ex,error);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync(JsonSerializer.Serialize(error));
            }
        }
    }
}
