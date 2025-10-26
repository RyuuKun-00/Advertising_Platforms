using AdvertisingPlatforms.Core.Models;

namespace AdvertisingPlatforms.Core.Abstractions
{
    /// <summary>
    /// Шаблон репозитория для манипулирования рекламными площадками в хранилище
    /// </summary>
    public interface IAdvertisingPlatformsRepository
    {
        /// <summary>
        /// Поиск рекламных площадок в хранилище по локации
        /// </summary>
        /// <param name="nameLocation">Название локации</param>
        /// <returns>Список рекламных площадок соответствующих заданной локации</returns>
        Task<List<AdvertisingPlatform>> Search(string nameLocation);
    }
}