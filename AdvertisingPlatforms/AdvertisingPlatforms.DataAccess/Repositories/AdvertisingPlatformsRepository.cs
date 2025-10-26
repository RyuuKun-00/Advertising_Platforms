
using AdvertisingPlatforms.Core.Abstractions;
using AdvertisingPlatforms.Core.Entities;
using AdvertisingPlatforms.Core.Models;

namespace AdvertisingPlatforms.DataAccess.Repositories
{
    /// <summary>
    /// Реализация шаблона репозитория
    /// </summary>
    public class AdvertisingPlatformsRepository : IAdvertisingPlatformsRepository
    {
        private readonly IStorage _storage;

        public AdvertisingPlatformsRepository(IStorage storage)
        {
            _storage = storage;
        }
        public async Task<List<AdvertisingPlatform>> Search(string nameLocation)
        {
            var result = await Task.Run(() =>
            {
                // Проверяем в хранилище наличие рекламных площадок соответствующей локации
                bool isTry = _storage.StorageAP.TryGetValue(nameLocation, out AdvertisingPlatformEntity? entity);

                List<AdvertisingPlatform> platforms = new();

                // если площадки есть
                if (isTry)
                {
                    var temp = entity;
                    // получем все возможные площадки увеличивая область работы рекламного агенства
                    while (temp is not null)
                    {
                        platforms.Add(new AdvertisingPlatform(temp.NameLocation, temp.NamesAdvertisingPlatforms));

                        temp = temp.Previous;
                    }
                }

                return platforms;
            });

            return result;
        }
    }
}
