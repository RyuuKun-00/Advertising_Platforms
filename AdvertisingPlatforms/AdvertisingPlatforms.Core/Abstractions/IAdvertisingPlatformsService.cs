using AdvertisingPlatforms.Core.Models;

namespace AdvertisingPlatforms.Core.Abstractions
{
    /// <summary>
    /// Шаблон сервиса для манипулирования рекламными площадками
    /// </summary>
    public interface IAdvertisingPlatformsService
    {
        /// <summary>
        /// Поиск рекламных площадок по локации
        /// </summary>
        /// <param name="nameLocation">Название локации</param>
        /// <returns>Список рекламных площадок соответствующих заданной локации</returns>
        Task<(List<AdvertisingPlatform>?, string?)> Search(string nameLocation);
    }
}