using AdvertisingPlatforms.Core.Abstractions;
using AdvertisingPlatforms.Tests.TestResources.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AdvertisingPlatforms.Tests.Integration_Tests.CustomWebApplication
{
    public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Метод удоления сервисов из сборщика
                var DeleteService = (Type NameService) =>
                {
                    var iAppParameters = services.SingleOrDefault(
                        d => d.ServiceType ==NameService);

                    if(iAppParameters is null)
                    {
                        throw new Exception($"Не удалось удалить сервис: {NameService.FullName}");
                    }
                    services.Remove(iAppParameters);
                };

                // Удаление сервисов параметров
                DeleteService(typeof(IAppParameters));
                DeleteService(typeof(IAdvertisingPlatformValidationParameters));
                DeleteService(typeof(IFileValidationParameters));
                DeleteService(typeof(IResponseTemplates));

                // Добавление новых сервисов параметров, основанных на тестовом классе
                services.AddSingleton<IAdvertisingPlatformValidationParameters, AppParameters_Test>(_ => AppParameters_Test.GetInstance);
                services.AddSingleton<IFileValidationParameters, AppParameters_Test>(_ => AppParameters_Test.GetInstance);
                services.AddSingleton<IResponseTemplates, AppParameters_Test>(_ => AppParameters_Test.GetInstance);
                services.AddSingleton<IAppParameters, AppParameters_Test>(_ => AppParameters_Test.GetInstance);
            });

            builder.UseEnvironment("Development");
        }
    }
}
