using AdvertisingPlatforms.Core.Abstractions;
using Xunit.Abstractions;

namespace AdvertisingPlatforms.Tests.Unit_Tests.ValidationTest.DTO
{
    /// <summary>
    /// Класс передачи параметров в метод для тестирования локаций
    /// </summary>
    public class DTO_LocationValidation_Params : IAdvertisingPlatformValidationParameters
    {
        public DTO_LocationValidation_Params(int id_test) 
        {
            ID_test = id_test;
        }
        public DTO_LocationValidation_Params(int id_test,
                          bool allowingTheUseOfCapitalLetters,
                          bool capitaLetterSensitivity,
                          bool repeatingSubLocations,
                          bool locationsWithTheSameName)
        {
            AllowingTheUseOfCapitalLetters = allowingTheUseOfCapitalLetters;
            CapitaLetterSensitivity = capitaLetterSensitivity;
            RepeatingSubLocations = repeatingSubLocations;
            LocationsWithTheSameName = locationsWithTheSameName;
            ID_test = id_test;
        }

        #region Реализация интерфейса IAdvertisingPlatformValidationParameters
        public bool AllowingTheUseOfCapitalLetters { get; init; } = false;
        public bool CapitaLetterSensitivity { get; init; } = false;
        public bool RepeatingSubLocations { get; init; } = false;
        public bool LocationsWithTheSameName { get; init; } = false;
        public string StringValidationPattern { 
            get {
                return AllowingTheUseOfCapitalLetters
                       ? @"^[a-zA-Z/]+$" // Тут верхний регист разрешён
                       : @"^[a-z/]+$";   // Тут верхний регист вызывает ошибки
                 }
        }
        #endregion

        /// <summary>
        /// Номер теста
        /// <para>
        /// По умолчанию: <b>0</b>
        /// </para>
        /// </summary>
        public int ID_test { get; init; }


        /// <summary>
        /// Массив правильных локаций, которые пройдут проверку для добавления возможных повторений подлокаций
        /// <para>
        /// По умолчанию: <b>[]</b>
        /// </para>
        /// </summary>
        public string[] Locations { get; init; } = [];

        /// <summary>
        /// Тестируемая строка
        /// <para>
        /// По умолчанию: <b>String.Empty</b>
        /// </para>
        /// </summary>
        public string TestedLocation { get; init; } = String.Empty;

        /// <summary>
        /// Ожидаемый результат валидации и десериализации <see cref="TestedLocation">TestedLocation</see> 
        /// <para>
        /// По умолчанию: <b>true</b>
        /// </para>
        /// </summary>
        public bool CorrectResult { get; init; } = true;

        /// <summary>
        /// Если ожидаемый результат <see cref="CorrectResult">CorrectResult</see> = <b>true</b><br/>
        /// то мы можем свериться с последним десерилизованный элементом<br/>
        /// Локация: /ru/xr/fhg -> десериализация -> ["ru","xr","fhg"]
        /// <para>
        /// По умолчанию: <b>null</b>
        /// </para>
        /// </summary>
        public string[]? CorrectValue { get; init; } = null;

        public override string ToString()
        {
            return $"ID:{ID_test}, {AllowingTheUseOfCapitalLetters}, {CapitaLetterSensitivity}, {RepeatingSubLocations}, " +
                $"{LocationsWithTheSameName},TestedLocation: ${TestedLocation} Locations:[ \"{string.Join("\", \"", Locations)}\" ], CorrectResult: {CorrectResult}, " +
                $"CorrectValue: {(CorrectValue is null? "null" : "[ \""+string.Join("\", \"", CorrectValue)+"\" ]")}"
            ;
        }
    }

}
