using Xunit;

namespace AdvertisingPlatforms.Tests.Integration_Tests.CustomWebApplication
{
    /// <summary>
    /// Класс для создания общего контекста, а точнее одного веб приложения
    /// </summary>
    [CollectionDefinition("CommonClientWebApplication")]
    public class CommonClientWebApplication
        :ICollectionFixture<CustomApiFixture>
    {
    }
}
