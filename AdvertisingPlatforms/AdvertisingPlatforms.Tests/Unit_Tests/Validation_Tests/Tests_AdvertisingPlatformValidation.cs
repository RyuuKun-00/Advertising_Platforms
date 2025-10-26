using AdvertisingPlatforms.Application.Validators;
using AdvertisingPlatforms.Core.Abstractions;
using AdvertisingPlatforms.Core.Models;
using AdvertisingPlatforms.Tests.TestResources.Models;
using AdvertisingPlatforms.Tests.Unit_Tests.ValidationTest.DTO;
using Xunit;

namespace AdvertisingPlatforms.Tests.Unit_Tests.ValidationTest
{
    public class Tests_AdvertisingPlatformValidation
    {
        [Theory]
        [MemberData(nameof(GetParams))]
        public void Test_IsValidAndDeserialization(DTO_AdvertisingPlatformValidation_Params testParams)
        {
            /////////////
            // Arrange //
            /////////////
            
            // Создаём класс параметров валидации
            IAdvertisingPlatformValidationParameters validationParameters = new AppParameters_Test() {
                                           AllowingTheUseOfCapitalLetters = testParams.AllowingTheUseOfCapitalLetters,
                                           CapitaLetterSensitivity = testParams.CapitaLetterSensitivity,
                                           RepeatingSubLocations = testParams.RepeatingSubLocations,
                                           LocationsWithTheSameName = testParams.LocationsWithTheSameName
            };
            // Создаём класс валидации рекламмных площадок
            IAdvertisingPlatformValidation validation = new AdvertisingPlatformValidation(validationParameters);
            bool result = false;
            AdvertisingPlatformDTO? platformDTO=null;

            /////////
            // Act //
            /////////
            
            // Валедируем и десериализуем входную строку
            foreach (string line in testParams.Lines)
            {
                result = validation.IsValid(line, out platformDTO);
            }

            ////////////
            // Assert //
            ////////////
            
            // Проверка на результат валидации
            if (testParams.CorrectResult)
            {
                Assert.True(result);
            }
            else
            {
                Assert.False(result);
            }
            // Проверка на результат десериализации и при успешной валидации
            if(testParams.CorrectResult && testParams.CorrectValue is not null)
            {
                Assert.Equal(testParams.CorrectValue, platformDTO!.Locations.Last());
            }
                
        }

        /// <summary>
        /// Тестовые данные
        /// </summary>
        public static IEnumerable<object[]> GetParams()
        {
            // Тест на правильный ответ
            // Строгие тербования только нижний регистр и без повторений локаций
            yield return [new DTO_AdvertisingPlatformValidation_Params(1) {
                // Параметры валидации данных файла
                AllowingTheUseOfCapitalLetters = false,
                CapitaLetterSensitivity = false,
                RepeatingSubLocations = false,
                LocationsWithTheSameName = false,
                Lines =["Item#0: /ru, /pz/kz"],
                CorrectResult = true,
                CorrectValue = ["pz","kz"]
            }];

            // Тест на правильный ответ
            // Допустимо использовать в локациях верхний регистр, но система будет его преобразовывать в нижний,
            // без повторений локаций
            yield return [new DTO_AdvertisingPlatformValidation_Params(2) {
                // Параметры валидации данных файла
                AllowingTheUseOfCapitalLetters = true,
                CapitaLetterSensitivity = false,
                RepeatingSubLocations = false,
                LocationsWithTheSameName = false,
                Lines =["Item#0: /ru, /Pz/Kz"],
                CorrectResult = true,
                CorrectValue = ["pz","kz"]
            }];

            // Тест на правильный ответ
            // Допустимо использовать в локациях верхний регистр, система чуствительна к верхнему регистру,
            // без повторений локаций
            yield return [new DTO_AdvertisingPlatformValidation_Params(3) {
                // Параметры валидации данных файла
                AllowingTheUseOfCapitalLetters = true,
                CapitaLetterSensitivity = true,
                RepeatingSubLocations = false,
                LocationsWithTheSameName = false,
                Lines =["Item#0: /ru, /Pz/Kz"],
                CorrectResult = true,
                CorrectValue = ["Pz", "Kz"]
            }];

            // Тест на ошибочный ответ
            // Допустимо использовать в локациях верхний регистр, но система будет его преобразовывать в нижний,
            // без повторений локаций
            yield return [new DTO_AdvertisingPlatformValidation_Params(4) {
                // Параметры валидации данных файла
                AllowingTheUseOfCapitalLetters = true,
                CapitaLetterSensitivity = false,
                RepeatingSubLocations = false,
                LocationsWithTheSameName = false,
                // не могут юыть локации с одинаковыми подлокациями и разными полными локациями
                // /ru и /Pz/ru -> Одинаково заканчиваются, но разный префикс
                Lines =["Item#0: /ru, /Pz/Kz","Item#1: /ru, /Pz/ru"],
                CorrectResult = false,
                CorrectValue = null
            }];

            // Тест на правильный ответ
            // Допустимо использовать в локациях верхний регистр, но система будет его преобразовывать в нижний,
            // Разрешено повторение конечных локаций в разных полных локациях
            yield return [new DTO_AdvertisingPlatformValidation_Params(5) {
                // Параметры валидации данных файла
                AllowingTheUseOfCapitalLetters = true,
                CapitaLetterSensitivity = false,
                RepeatingSubLocations = false,
                LocationsWithTheSameName = true,
                Lines =["Item#0: /ru, /Pz/Kz","Item#1: /ru, /Pz/ru"],
                CorrectResult = true,
                CorrectValue = ["pz", "ru"]
            }];

            // Тест на ошибочный ответ
            // Допустимо использовать в локациях верхний регистр, но система будет его преобразовывать в нижний,
            // Разрешено повторение конечных локаций в разных полных локациях
            yield return [new DTO_AdvertisingPlatformValidation_Params(6) {
                // Параметры валидации данных файла
                AllowingTheUseOfCapitalLetters = true,
                CapitaLetterSensitivity = false,
                RepeatingSubLocations = false,
                LocationsWithTheSameName = true,
                // В одной полной локации запрещены одинаковые подлокации
                // /ru/ru -> ошибка
                Lines =["Item#0: /ru, /Pz/Kz","Item#1: /ru, /ru/ru"],
                CorrectResult = false,
                CorrectValue = null
            }];

            // Тест на правильный ответ
            // Допустимо использовать в локациях верхний регистр, но система будет его преобразовывать в нижний,
            // Разрешено повторение конечных локаций и в полных локациях
            yield return [new DTO_AdvertisingPlatformValidation_Params(7) {
                // Параметры валидации данных файла
                AllowingTheUseOfCapitalLetters = true,
                CapitaLetterSensitivity = false,
                RepeatingSubLocations = true,
                LocationsWithTheSameName = true,
                Lines =["Item#0: /ru, /Pz/Kz","Item#1: /ru, /ru/ru"],
                CorrectResult = true,
                CorrectValue = ["ru", "ru"]
            }];

            // Тест на правильный ответ
            // Допустимо использовать в локациях верхний регистр, но система будет его преобразовывать в нижний,
            // Разрешено повторение конечных локаций и в полных локациях
            yield return [new DTO_AdvertisingPlatformValidation_Params(8) {
                // Параметры валидации данных файла
                AllowingTheUseOfCapitalLetters = true,
                CapitaLetterSensitivity = true,
                RepeatingSubLocations = true,
                LocationsWithTheSameName = true,
                Lines =["Item#0: /ru, /Pz/Kz","Item#1: /RU, /RU/RU"],
                CorrectResult = true,
                CorrectValue = ["RU", "RU"]
            }];
        }

    }

}
