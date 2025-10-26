using AdvertisingPlatforms.Core.Models;

namespace AdvertisingPlatforms.Core.Abstractions
{
    /// <summary>
    /// Шаблон валидации рекламных площадок
    /// </summary>
    public interface IAdvertisingPlatformValidation
    {


        /// <summary>
        /// Проверка строки локации, на повторение подлокаций и наличие запрещённых символов
        /// </summary>
        /// <param name="location">Строка локиции</param>
        /// <param name="subLocations">Массив подлокаций</param>
        /// <returns>Результат проверки валидации. true - если прошла</returns>
        bool IsLocation(string location, out string[]? subLocations);
        /// <summary>
        /// Метод валидации и десериализации строки в объект передачи данных: <see cref="AdvertisingPlatformDTO"/>
        /// </summary>
        /// <param name="line">Строка для десериализации рекламной площадки</param>
        /// <param name="advertisingPlatformDTO">Десериализованный объект</param>
        /// <returns><b>true</b> - если файл прошёл проверку, иначе: <b>false</b></returns>
        bool IsValid(string line, out AdvertisingPlatformDTO? advertisingPlatformDTO);
    }
}