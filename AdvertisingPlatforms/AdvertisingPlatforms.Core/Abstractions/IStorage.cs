using AdvertisingPlatforms.Core.Entities;

namespace AdvertisingPlatforms.Core.Abstractions
{
    /// <summary>
    /// Шаблон хранилища рекламных платформ
    /// </summary>
    public interface IStorage
    {
        /// <summary>
        /// Свейство для хранения словаря рекламных площадок по локации
        /// </summary>
        Dictionary<string, AdvertisingPlatformEntity> StorageAP { get; set; }
    }
}