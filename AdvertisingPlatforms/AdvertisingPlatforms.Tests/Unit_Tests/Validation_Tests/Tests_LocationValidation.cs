using AdvertisingPlatforms.Application.Validators;
using AdvertisingPlatforms.ApplicationSettings;
using AdvertisingPlatforms.Core.Abstractions;
using AdvertisingPlatforms.Tests.Unit_Tests.ValidationTest.DTO;
using Xunit;

namespace AdvertisingPlatforms.Tests.Unit_Tests.ValidationTest
{
    public class Tests_LocationValidation
    {
        [Theory]
        [MemberData(nameof(GetParams))]
        public void Test_IsValidAndDeserialization(DTO_LocationValidation_Params testParams)
        {
            /////////////
            // Arrange //
            /////////////
            
            // Создаём класс параметров валидации
            IAdvertisingPlatformValidationParameters validationParameters = new AppParametersSingleton()
            {
                AllowingTheUseOfCapitalLetters = testParams.AllowingTheUseOfCapitalLetters,
                CapitaLetterSensitivity = testParams.CapitaLetterSensitivity,
                RepeatingSubLocations = testParams.RepeatingSubLocations,
                LocationsWithTheSameName = testParams.LocationsWithTheSameName
            };
            // Создаём класс валидации рекламных площадок
            IAdvertisingPlatformValidation validation = new AdvertisingPlatformValidation(validationParameters);
            // Получаем строку для проверки
            string testedlLocation = testParams.TestedLocation;

            // Валидируем дополнительные тсроки для выявления повторений
            // При валидации они добавляются во внутренне хранилище
            foreach (string location in testParams.Locations)
            {
                bool res = validation.IsLocation(location, out string[]? subLoc);
                // Проверка результата валидации, так как строки должны быть валидны
                if (!res)
                {
                    throw new Exception("Добавляемы строки перед тестом должны быть валидны!");
                }
            }

            /////////
            // Act //
            /////////

            // Валедируем и десериализуем входную строку
            bool result = validation.IsLocation(testedlLocation, out string[]? subLocations);

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
            if (testParams.CorrectResult && testParams.CorrectValue is not null)
            {
                Assert.Equal(testParams.CorrectValue, subLocations);
            }

        }

        /// <summary>
        /// Тестовые данные
        /// </summary>
        public static IEnumerable<object[]> GetParams()
        {
            // Тест на правильный ответ
            // Строгие тербования только нижний регистр и без повторений локаций
            yield return [new DTO_LocationValidation_Params(1) {
                AllowingTheUseOfCapitalLetters = false,
                CapitaLetterSensitivity = false,
                RepeatingSubLocations = false,
                LocationsWithTheSameName = false,
                Locations = [],
                TestedLocation = "/pz/kz",
                CorrectResult = true,
                CorrectValue = ["pz","kz"]
            }];

            // Тест на правильный ответ
            // Допустимо использовать в локациях верхний регистр, но система будет его преобразовывать в нижний,
            // без повторений локаций
            yield return [new DTO_LocationValidation_Params(2) {
                AllowingTheUseOfCapitalLetters = true,
                CapitaLetterSensitivity = false,
                RepeatingSubLocations = false,
                LocationsWithTheSameName = false,
                Locations = [],
                TestedLocation = "/Pz/Kz",
                CorrectResult = true,
                CorrectValue = ["pz","kz"]
            }];

            // Тест на правильный ответ
            // Допустимо использовать в локациях верхний регистр, система чуствительна к верхнему регистру,
            // без повторений локаций
            yield return [new DTO_LocationValidation_Params(3) {
                AllowingTheUseOfCapitalLetters = true,
                CapitaLetterSensitivity = true,
                RepeatingSubLocations = false,
                LocationsWithTheSameName = false,
                Locations = [],
                TestedLocation = "/Pz/Kz",
                CorrectResult = true,
                CorrectValue = ["Pz", "Kz"]
            }];

            // Тест на ошибочный ответ
            // Допустимо использовать в локациях верхний регистр, но система будет его преобразовывать в нижний,
            // без повторений локаций
            yield return [new DTO_LocationValidation_Params(4) {
                AllowingTheUseOfCapitalLetters = true,
                CapitaLetterSensitivity = false,
                RepeatingSubLocations = false,
                LocationsWithTheSameName = false,
                // не могут юыть локации с одинаковыми подлокациями и разными полными локациями
                // /ru и /Pz/ru -> Одинаково заканчиваются, но разный префикс
                Locations = ["/Kz"],
                TestedLocation = "/Pz/Kz",
                CorrectResult = false,
                CorrectValue = null
            }];

            // Тест на правильный ответ
            // Допустимо использовать в локациях верхний регистр, но система будет его преобразовывать в нижний,
            // Разрешено повторение конечных локаций в разных полных локациях
            yield return [new DTO_LocationValidation_Params(5) {
                AllowingTheUseOfCapitalLetters = true,
                CapitaLetterSensitivity = false,
                RepeatingSubLocations = false,
                LocationsWithTheSameName = true,
                Locations = ["/ru"],
                TestedLocation = "/Pz/Ru",
                CorrectResult = true,
                CorrectValue = ["pz", "ru"]
            }];

            // Тест на ошибочный ответ
            // Допустимо использовать в локациях верхний регистр, но система будет его преобразовывать в нижний,
            // Разрешено повторение конечных локаций в разных полных локациях
            yield return [new DTO_LocationValidation_Params(6) {
                AllowingTheUseOfCapitalLetters = true,
                CapitaLetterSensitivity = false,
                RepeatingSubLocations = false,
                LocationsWithTheSameName = true,
                // В одной полной локации запрещены одинаковые подлокации
                // /ru/ru -> ошибка
                Locations = ["/ru"],
                TestedLocation = "/ru/Ru",
                CorrectResult = false,
                CorrectValue = null
            }];

            // Тест на правильный ответ
            // Допустимо использовать в локациях верхний регистр, но система будет его преобразовывать в нижний,
            // Разрешено повторение конечных локаций и в полных локациях
            yield return [new DTO_LocationValidation_Params(7) {
                AllowingTheUseOfCapitalLetters = true,
                CapitaLetterSensitivity = false,
                RepeatingSubLocations = true,
                LocationsWithTheSameName = true,
                Locations = ["/ru"],
                TestedLocation = "/ru/Ru",
                CorrectResult = true,
                CorrectValue = ["ru", "ru"]
            }];

            // Тест на правильный ответ
            // Допустимо использовать в локациях верхний регистр, но система будет его преобразовывать в нижний,
            // Разрешено повторение конечных локаций и в полных локациях
            yield return [new DTO_LocationValidation_Params(8) {
                AllowingTheUseOfCapitalLetters = true,
                CapitaLetterSensitivity = true,
                RepeatingSubLocations = true,
                LocationsWithTheSameName = true,
                Locations = ["/ru"],
                TestedLocation = "/RU/RU",
                CorrectResult = true,
                CorrectValue = ["RU", "RU"]
            }];
        }

    }
}
