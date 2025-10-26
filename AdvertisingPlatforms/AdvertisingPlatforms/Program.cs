using AdvertisingPlatforms.Configurations.Builder;
using AdvertisingPlatforms.Configurations.WebApp;

namespace AdvertisingPlatforms
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Добавление всех необходимых сервисов
            builder.AddServices();

            var app = builder.Build();

            // Добавление необходимой функциональности
            app.SetupWebApplication();

            app.Run();

        }
    }
}
