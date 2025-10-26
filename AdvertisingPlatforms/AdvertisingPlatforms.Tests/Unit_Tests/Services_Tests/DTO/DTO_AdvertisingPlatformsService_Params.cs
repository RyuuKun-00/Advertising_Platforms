using AdvertisingPlatforms.Core.Abstractions;
using AdvertisingPlatforms.Core.Entities;
using AdvertisingPlatforms.Tests.TestResources.Models;

namespace AdvertisingPlatforms.Tests.Unit_Tests.Services_Tests.DTO
{
    /// <summary>
    /// Класс передачи параметров в тест для поиска рекламмных площадок по заданной локации
    /// </summary>
    public class DTO_AdvertisingPlatformsService_Params
        : IAdvertisingPlatformValidationParameters
    {
        #region Реализация интерфейса IAdvertisingPlatformValidationParameters
        public bool AllowingTheUseOfCapitalLetters { get; init; } = false;

        public bool CapitaLetterSensitivity { get; init; } = false;

        public bool LocationsWithTheSameName { get; init; } = false;

        public bool RepeatingSubLocations { get; init; } = false;

        public string StringValidationPattern { get; } = string.Empty;

        #endregion

        /// <summary>
        /// Номер теста
        /// <para>
        /// По умолчанию: <b>0</b>
        /// </para>
        /// </summary>
        public int ID_test { get; init; }

        /// <summary>
        /// Храниище рекламных площадок по локациям
        /// <para>
        /// По умолчанию: <b>String.Empty</b>
        /// </para>
        /// </summary>
        public string Content { get; set; } = String.Empty;

        /// <summary>
        /// Строка для поиска рекламных площадок
        /// <para>
        /// По умолчанию: <b>/ru</b>
        /// </para>
        /// </summary>
        public string SearchLocation { get; set; } = "/ru";

        /// <summary>
        /// Ожидаемый результат валидации
        /// <para>
        /// По умолчанию: <b>true</b>
        /// </para>
        /// </summary>
        public bool ValidationResult { get; set; } = true;

        /// <summary>
        /// Ожидаемый результат, если строка прошла валидацию
        /// <para>
        /// По умолчанию: <b>null</b>
        /// </para>
        /// </summary>
        public List<AdvertisingPlatforms_Test>? Result { get; set; } = null;

        public DTO_AdvertisingPlatformsService_Params(int id_test = 0)
        {
            ID_test = id_test;
        }



        
    }
}
