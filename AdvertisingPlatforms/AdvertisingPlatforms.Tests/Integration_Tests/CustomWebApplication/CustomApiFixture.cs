using AdvertisingPlatforms.Core.Abstractions;
using AdvertisingPlatforms.Tests.TestResources.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AdvertisingPlatforms.Tests.Integration_Tests.CustomWebApplication
{
    /// <summary>
    /// Пользовательский класс для создания клиента
    /// </summary>
    public class CustomApiFixture:IDisposable
    {
        public HttpClient Client;
        public CustomWebApplicationFactory<Program> Factory;
        public AppParameters_Test Parameters;
        public CustomApiFixture()
        {
            Factory = new CustomWebApplicationFactory<Program>();
            Parameters = (AppParameters_Test)(Factory.Services.GetService(typeof(IAppParameters))
                                               ?? AppParameters_Test.GetInstance);
            Client = Factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        public void Dispose()
        {
            Client.Dispose();
        }
    }
}
