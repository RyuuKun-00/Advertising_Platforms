using AdvertisingPlatforms.Core.Abstractions;
using AdvertisingPlatforms.Core.Models;

namespace AdvertisingPlatforms.Application.Services
{
    /// <summary>
    /// Реализация шаблона для манипулирования рекламными площадками
    /// </summary>
    public class AdvertisingPlatformsService : IAdvertisingPlatformsService
    {
        private readonly IAdvertisingPlatformsRepository _repository;
        private readonly IAdvertisingPlatformValidation _validation;
        private readonly IResponseTemplates _templates;
        private readonly IAdvertisingPlatformValidationParameters _parameters;

        public AdvertisingPlatformsService(IAdvertisingPlatformsRepository repository,
                                           IAdvertisingPlatformValidation validation,
                                           IResponseTemplates templates,
                                           IAdvertisingPlatformValidationParameters parameters)
        {
            _repository = repository;
            _validation = validation;
            _templates = templates;
            _parameters = parameters;
        }

        public async Task<(List<AdvertisingPlatform>?,string?)> Search(string nameLocation)
        {
            // Проверяем строку поиска на соответствие локации
            bool isTry = _validation.IsLocation(nameLocation, out string[]? subLocations);

            // Если локация не валидна, то ошибка
            if (!isTry)
            {
                return (null,$"Локация для поиска задана не верно.\r\n{_templates.LocationTemplateDescription}");
            }

            
            string strSearch;
            int tempSubLoc = subLocations!.Length-1;
            List<AdvertisingPlatform> result;
            do
            {
                // Если запрещено ипользовать одинаковые конечные подлокации,
                // то можно определить по конечной локации.
                // Этот код нужен чтобы вернуть значение подлокаци,
                // если нет значения текущей локации.
                // Например:
                // ищем /ru/pz -> такого нет
                // но есть /ru -> возвращаем его рекламные площадки,
                // так как они тоже охватывают область /ru/pz
                if (_parameters.LocationsWithTheSameName)
                {
                    strSearch = "";
                    for(int i = 0; i <= tempSubLoc; i++)
                    {
                        strSearch += $"/{subLocations![tempSubLoc]}";
                    }
                }
                else
                {
                    strSearch = $"/{subLocations![tempSubLoc]}";
                }
                

                // Получение списка рекламных площадок
                result = await _repository.Search(strSearch);

                // Если ответ получен, то выходим из поиска
                if(result.Count > 0)
                {
                    break;
                }

                tempSubLoc--;
                // Если указатель меньше нуля, то локаций нет
                if (tempSubLoc < 0)
                {
                    break;
                }

            } while (true);

            return (result, null);
        }
    }
}
