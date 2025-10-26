

namespace AdvertisingPlatforms.Core.Models
{
    /// <summary>
    /// Модель для передачи данных от валидации к билдеру для создания словаря данных
    /// </summary>
    public record class AdvertisingPlatformDTO(string Name, List<string[]> Locations);
}
