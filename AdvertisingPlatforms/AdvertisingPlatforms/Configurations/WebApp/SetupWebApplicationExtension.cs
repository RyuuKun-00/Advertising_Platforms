using AdvertisingPlatforms.Middleware;

namespace AdvertisingPlatforms.Configurations.WebApp
{
    public static class SetupWebApplicationExtension
    {
        /// <summary>
        /// Расширение для <see cref="WebApplication"/><br/>
        /// Настройка функциональности веб-приложения
        /// </summary>
        public static WebApplication SetupWebApplication(this WebApplication app)
        {
            // Добавление функциональности для Swagger и для разработки, если проект в режиме разработки
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerUI();
                app.UseSwagger();
                
            }

            // Для доступа к API из вне, только для разработки
            //app.UseCors("AllowAll");


            app.UseHttpsRedirection();

            // Доступ к публичной части сайта
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Глобальная обработка ошибок
            app.UseMiddleware<GlobalErrorHandlerMiddleware>();

            app.MapControllers();

            // Вывод в логгер пользовательских настроек приложения
            app.PrintApplicationSettingsInLog();

            return app;
        }
    }
}
