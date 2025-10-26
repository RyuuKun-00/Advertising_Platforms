using AdvertisingPlatforms.Core.Abstractions;
using AdvertisingPlatforms.Core.Entities;
using AdvertisingPlatforms.Core.Models;

namespace AdvertisingPlatforms.Application.Services
{
    public class StorageBuilder : IStorageBuilder
    {
        private readonly IAdvertisingPlatformValidationParameters _validationParameters;
        private readonly IStorage _storage;

        public StorageBuilder(IAdvertisingPlatformValidationParameters validationParameters, IStorage storage)
        {
            _validationParameters = validationParameters;
            _storage = storage;
        }

        public async Task<IStorage> CreateStorage(List<AdvertisingPlatformDTO> advertisingPlatformDTOs)
        {
            return await Task.Run(() =>
            {
                // Создаём хранилище
                Dictionary<string, AdvertisingPlatformEntity> storage = new();

                // Добавляни платформы в хранилище
                AddAdvertisingPlatformsInStorage(storage, advertisingPlatformDTOs);

                // Удаление пустых подлокаций, связывание зависимых подлокаций
                // и сортирка списков рекламных площадок
                StorageNormalization(storage);

                _storage.StorageAP = storage;

                return _storage;
            });
        }

        /// <summary>
        /// Метод добавления рекламных площадок в хранилище
        /// </summary>
        /// <param name="storage">Хранилище куда будут добавляться площадки</param>
        /// <param name="advertisingPlatformDTOs">Список рекламных площадок, для добавления</param>
        /// <returns>Хранилище с добавленными площадками</returns>
        private Dictionary<string, AdvertisingPlatformEntity> AddAdvertisingPlatformsInStorage
            (Dictionary<string, AdvertisingPlatformEntity> storage,
             List<AdvertisingPlatformDTO> advertisingPlatformDTOs)
        {

            // Перебираем рекламные площадки
            foreach (AdvertisingPlatformDTO entity in advertisingPlatformDTOs)
            {
                // Добавляем площадку во все локации
                foreach (string[] location in entity.Locations)
                {
                    AdvertisingPlatformEntity? lastLSubLocation = null;
                    string fullLocation = "";
                    // Смотрим наличие или добаляем все подлокации в хранилище
                    foreach (string subLocation in location)
                    {
                        fullLocation += $"/{subLocation}";

                        string searchKey = String.Empty;
                        // Если локации могут имень одинаковые конечные названия
                        if (_validationParameters.LocationsWithTheSameName)
                        {
                            searchKey = fullLocation;
                        }
                        else
                        {
                            searchKey = $"/{subLocation}"; ;
                        }
                        // Проверка на наличие элемента в хранилище
                        bool isTry = storage.TryGetValue(searchKey, out AdvertisingPlatformEntity? elementLocation);

                        // При отсутсвии элемента содаём новый и сохраняем его
                        if (!isTry)
                        {
                            elementLocation = new() { NameLocation = fullLocation };
                            storage.Add(searchKey, elementLocation);
                        }

                        // Установка ссылки предыдущего элемента
                        elementLocation!.Previous = lastLSubLocation;

                        lastLSubLocation = elementLocation;
                    }

                    lastLSubLocation!.NamesAdvertisingPlatforms.Add(entity.Name);
                }
            }

            return storage;
        }

        /// <summary>
        /// Метод удаление пустых подлокаций, связывание зависимых подлокаций
        /// и сортирки списков рекламных площадок
        /// </summary>
        /// <param name="storage">Хранилище для нормализации</param>
        /// <returns>Нормализованное хранилище</returns>
        private Dictionary<string, AdvertisingPlatformEntity> StorageNormalization
            (Dictionary<string, AdvertisingPlatformEntity> storage)
        {
            List<string> listKeysToDelete = new();
            // Перебираем все локации
            foreach (var location in storage)
            {
                AdvertisingPlatformEntity entity = location.Value;

                var listAdvertisingPlatform = entity.NamesAdvertisingPlatforms;

                // Если список рекламных площадок пуст, то нам не нужна запись данной локации
                if (listAdvertisingPlatform.Count == 0)
                {
                    listKeysToDelete.Add(location.Key);
                    continue;
                }

                // Сортировка списка рекламных площадок
                listAdvertisingPlatform.Sort();

                // Переносим ссылку на предыдущий элемент по порядку пока не дойдём до конца или действующий элемент
                AdvertisingPlatformEntity? temp = entity.Previous;

                while (temp is not null && temp!.NamesAdvertisingPlatforms.Count == 0)
                {
                    temp = temp!.Previous;
                }

                entity.Previous = temp;
            }

            // Очищение пустых значений в хранилище
            foreach (string key in listKeysToDelete)
            {
                storage.Remove(key);
            }

            return storage;
        }
    }
}
