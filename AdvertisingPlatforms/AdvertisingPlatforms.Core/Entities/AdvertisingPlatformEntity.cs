

namespace AdvertisingPlatforms.Core.Entities
{ 
    /// <summary>
    /// Сущность для хранения рекламных площадок в текущей локации
    /// </summary>
    public class AdvertisingPlatformEntity
    {
        /// <summary>
        /// Название локации
        /// </summary>
        public string NameLocation = String.Empty;
        /// <summary>
        /// Список рекламных площадок, работающих в локации: <see cref="NameLocation"/> 
        /// </summary>
        public List<string> NamesAdvertisingPlatforms = new List<string>();
        /// <summary>
        /// Сущность предыдущей локации
        /// </summary>
        public AdvertisingPlatformEntity? Previous = null;
    }
}
