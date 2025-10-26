

namespace AdvertisingPlatforms.Tests.TestResources.Models
{
    /// <summary>
    /// Тестовый класс хранения рекламных площадок по локации
    /// </summary>
    public class AdvertisingPlatforms_Test
    {
        /// <summary>
        /// Название локации
        /// <para>
        /// По умолчанию: <b>String.Empty</b>
        /// </para>
        /// </summary>
        public string NameLocation { get; set; } = String.Empty;
        /// <summary>
        /// Список рекламных площадок
        /// <para>
        /// По умолчанию: <b>[]</b>
        /// </para>
        /// </summary>
        public List<string> AdvertisingPlatforms { get; set; } = new();

        public override string ToString()
        {
            return $"AdvertisingPlatforms_Test {{ NameLocation = {NameLocation}, AdvertisingPlatforms = [ {String.Join(", ", AdvertisingPlatforms)} ] }}";
        }
    }
}
