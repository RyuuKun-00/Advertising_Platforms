

using AdvertisingPlatforms.Core.Abstractions;
using AdvertisingPlatforms.Core.Models;
using System.Text.RegularExpressions;

namespace AdvertisingPlatforms.Application.Validators
{
    /// <summary>
    /// Класс валидации рекламной площадки
    /// </summary>
    public class AdvertisingPlatformValidation : IAdvertisingPlatformValidation
    {
        private readonly IAdvertisingPlatformValidationParameters _validationParameters;

        private Dictionary<string,string> _uniqueSubLocations = new(); 
        public AdvertisingPlatformValidation(IAdvertisingPlatformValidationParameters validationParameters)
        {
            _validationParameters = validationParameters;
        }
        /// <summary>
        /// Метод валидации и десериализации строки в объект передачи данных: <see cref="AdvertisingPlatformDTO"/>
        /// </summary>
        /// <param name="line">Строка для десериализации рекламной площадки</param>
        /// <param name="advertisingPlatformDTO">Десериализованный объект</param>
        /// <returns><b>true</b> - если файл прошёл проверку, иначе: <b>false</b></returns>
        public bool IsValid(string line, out AdvertisingPlatformDTO? advertisingPlatformDTO)
        {
            advertisingPlatformDTO = null;

            // Проверка на паттерн
            // <название> : <локации>
            bool isNameAndLocations = IsNameAndLocations(line, out string? namePlatform, out string[]? locations);

            if (!isNameAndLocations)
            {
                return false;
            }

            // Проверка на наличие запрещённых символов, повторений и паттерна:
            // /<локация>/<локация>/.../<локация>
            bool isValidLocations = IsLocations(locations!, out List<string[]>? listSubLocations);

            if (!isValidLocations)
            {
                return false;
            }

            advertisingPlatformDTO = new AdvertisingPlatformDTO(namePlatform!, listSubLocations!);

            return true;
        }

        /// <summary>
        /// Проверка на соответствие входящей строки паттерну:
        /// <para>[название] : [локации]</para>
        /// </summary>
        /// <param name="line">Входящая строка для валидации</param>
        /// <param name="namePlatform"></param>
        /// <param name="locations"></param>
        /// <returns>(название , локации) если IsValid=true</returns>
        private bool IsNameAndLocations(string line, out string? namePlatform, out string[]? locations)
        {
            namePlatform = null;
            locations = null;

            string[] data = line.Split(':');

            // Если символ ':' встречается больше одного раза, то не сответствие шаблону -> [название] : [локации]
            if (data.Length != 2)
            {
                return false;
            }

            namePlatform = data[0].Trim();
            locations = data[1].Split(",", StringSplitOptions.RemoveEmptyEntries);

            return true;
        }

        /// <summary>
        /// Проверка на наличие запрещённых символов, повторений и паттерна:
        /// <para>/[локация]/[локация].../[локация]</para>
        /// </summary>
        /// <param name="locations">Строки локаций</param>
        /// <param name="listSubLocations">Список массивов подлокаций</param>
        /// <returns>Результат проверки валидации. true - если прошла</returns>
        private bool IsLocations(string[] locations, out List<string[]>? listSubLocations)
        {
            listSubLocations = null;

            List<string[]> temp_ValidLocations = new();

            // Перебираем  список всех локаций
            foreach (string location in locations)
            {
                // Проверка на валидность локации
                bool isValidLocation = IsLocation(location, out string[]? subLocations);

                if (isValidLocation)
                {
                    temp_ValidLocations.Add(subLocations!);
                }
                else
                {
                    return false;
                }
            }

            listSubLocations = temp_ValidLocations;

            return true;
        }
        /// <summary>
        /// Проверка строки локации, на повторение подлокаций и наличие запрещённых символов
        /// </summary>
        /// <param name="location">Строка локиции</param>
        /// <param name="validLocation">Действительная локация</param>
        /// <returns>Результат проверки валидации. true - если прошла</returns>
        public bool IsLocation(string location, out string[]? subLocations)
        {
            subLocations = null;

            string locTrim = location.Trim();

            if (locTrim.Length == 0)
            {
                return false;
            }

            // Проверка на разрешённые символы по патерну
            bool isValidPatternCheck = IsValidPatternCheck(locTrim);
            if (!isValidPatternCheck)
            {
                return false;
            }

            // Преобразование локации к нижнему регистру, если они не чувствительны к регистру
            UppercaseResolution(ref locTrim);

            // Проверка на пустые подлокации и повторения подлокаций
            bool isValidWhiteSpaceOrRepeat = IsValidWhiteSpaceOrRepeatSubLocations(locTrim, out string[]? result_subLocations);

            if (!isValidWhiteSpaceOrRepeat)
            {
                return false;
            }

            subLocations = result_subLocations;

            return true;
        }

        /// <summary>
        /// Проверка лакации на наличие посторонних символов
        /// </summary>
        private bool IsValidPatternCheck(string location)
        {
            // Проверка на разрешённые символы по патерну
            return Regex.IsMatch(location, _validationParameters.StringValidationPattern);
        }

        /// <summary>
        /// Если параметр CapitaLetterSensitivity = true,<br/>
        /// то следующие локации разные: /ru /Ru /rU /RU
        /// </summary>
        private void UppercaseResolution(ref string location)
        {
            if (_validationParameters.CapitaLetterSensitivity) return;

            location = location.ToLowerInvariant();
        }

        /// <summary>
        /// Проверяем вложенные локации на пустые и повторяющиеся
        /// </summary>
        /// <returns>Массив подлокаций</returns>
        private bool IsValidWhiteSpaceOrRepeatSubLocations(string strLocation, out string[]? subLocations)
        {
            subLocations = null;

            // Локация должна начинаться с символа '/'
            if (strLocation[0] != '/')
            {
                return false;
            }

            string[] locations = strLocation.Substring(1).Split('/');
            List<string> listSubLocations = new();

            foreach (string location in locations)
            {
                // Проверка на пустую вложенную локацию
                if (String.IsNullOrWhiteSpace(location))
                {
                    return false;
                }

                // Проверка на разрешение повторений подлокаций в одной локации
                if (!_validationParameters.RepeatingSubLocations)
                {   // Поиск повторения
                    if (listSubLocations.IndexOf(location) > -1)
                    {
                        return false;
                    }
                }

                listSubLocations.Add(location);
                continue;
            }

            // Проверка на использование одинаковых конечных подлокаций
            if (!_validationParameters.LocationsWithTheSameName)
            {
                string key = locations.Last();
                bool isTry = _uniqueSubLocations.TryGetValue(key,out string? fullLocal);
                // Если такая локация уже сохранена
                if (isTry)
                {
                    // То проверяем совпадает ли она с текущей, если нет то ошибка
                    if (strLocation != fullLocal!)
                    {
                        return false;
                    }
                }
                else
                {
                    _uniqueSubLocations.Add(key, strLocation);
                }
            }

            subLocations = listSubLocations.ToArray();

            return true;
        }
    }
}
