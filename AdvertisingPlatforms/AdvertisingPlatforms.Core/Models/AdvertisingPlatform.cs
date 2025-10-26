

namespace AdvertisingPlatforms.Core.Models
{
    /// <summary>
    /// Класс списка рекламных площадок на определённой локации
    /// </summary>
    /// <param name="NameLocation">Название локации</param>
    /// <param name="AdvertisingPlatforms">Список рекламных площадок</param>
    public record class AdvertisingPlatform(string NameLocation,List<string> AdvertisingPlatforms);
}
