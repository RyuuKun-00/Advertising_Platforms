using AdvertisingPlatforms.Core.Models;

namespace AdvertisingPlatforms.Core.Abstractions
{
    /// <summary>
    /// Шаблон сборщика хранилища рекламных площадок
    /// </summary>
    public interface IStorageBuilder
    {
        /// <summary>
        /// Метод создания хранилища
        /// </summary>
        /// <param name="advertisingPlatformDTOs"></param>
        Task<IStorage> CreateStorage(List<AdvertisingPlatformDTO> advertisingPlatformDTOs);
    }
}