

namespace AdvertisingPlatforms.Core.Abstractions
{
    /// <summary>
    /// Интерфейс группирующий основные параметры приложения:<br/>
    /// <see cref="IAdvertisingPlatformValidationParameters">IAdvertisingPlatformValidationParameters</see>,
    /// <see cref="IFileValidationParameters">IFileValidationParameters</see>,
    /// <see cref="IResponseTemplates">IResponseTemplates</see>
    /// </summary>
    public interface IAppParameters: IAdvertisingPlatformValidationParameters,
                                      IFileValidationParameters,
                                      IResponseTemplates
    {
    }
}
