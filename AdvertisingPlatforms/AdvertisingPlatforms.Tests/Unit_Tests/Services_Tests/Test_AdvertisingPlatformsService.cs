using AdvertisingPlatforms.Application.Services;
using AdvertisingPlatforms.Application.Validators;
using AdvertisingPlatforms.Core.Abstractions;
using AdvertisingPlatforms.Core.Models;
using AdvertisingPlatforms.DataAccess.Repositories;
using AdvertisingPlatforms.DataAccess.Storage;
using AdvertisingPlatforms.Tests.Integration_Tests.Controllers_Tests.AdvertisingPlatformsController.DTO;
using AdvertisingPlatforms.Tests.TestResources.Models;
using AdvertisingPlatforms.Tests.Unit_Tests.Services_Tests.DTO;
using System.Net;
using Xunit;

namespace AdvertisingPlatforms.Tests.Unit_Tests.Services_Tests
{
    public class Test_AdvertisingPlatformsService
    {
        [Theory]
        [MemberData(nameof(GetParams))]
        public async Task Test_SearchAdvertisingPlatforms(DTO_AdvertisingPlatformsService_Params parameters)
        {

            /////////////
            // Arrange //
            /////////////

            // Инициализация параметров приложения
            var appParameters = new AppParameters_Test()
            {
                AllowingTheUseOfCapitalLetters = parameters.AllowingTheUseOfCapitalLetters,
                CapitaLetterSensitivity = parameters.CapitaLetterSensitivity,
                RepeatingSubLocations = parameters.RepeatingSubLocations,
                LocationsWithTheSameName = parameters.LocationsWithTheSameName
            };
            // Создание шаблонов ответов
            IResponseTemplates templatesResponse = appParameters;
            // Создание параметров валидации рекламных площадок
            IAdvertisingPlatformValidationParameters validationParameters = appParameters;
            // Создание валидатора на основе параметров валидации
            IAdvertisingPlatformValidation validator = new AdvertisingPlatformValidation(validationParameters); ;

            // Создание хранилища для поиска рекламных площадок
            List<AdvertisingPlatformDTO> listAP = new ();
            // Разделяем строку аострочно для обработки
            var linesAP = parameters.Content.Split("\n",StringSplitOptions.RemoveEmptyEntries);
            foreach(var line in linesAP)
            {
                // Валидируем данные
                var validResult = validator.IsValid(line, out AdvertisingPlatformDTO? advertisingPlatformDTO);
                if (validResult)
                {
                    // Добавляем спарсенные данные в список
                    listAP.Add(advertisingPlatformDTO!);
                }
                else
                {
                    Assert.Fail("Ошибка при создании хранилища, данные не прошли валидацию.");
                }
            }
            // Создаём сборщие хранилища
            IStorageBuilder storageBuilder = new StorageBuilder(validationParameters, new StorageSingleton());
            // Собираем хранилище
            IStorage storage = await storageBuilder.CreateStorage(listAP);

            // Создание репозитория(доступа) к хранилищу
            IAdvertisingPlatformsRepository repository = new AdvertisingPlatformsRepository(storage);

            // Создание сервиса поиска рекламных площадок
            IAdvertisingPlatformsService service = new AdvertisingPlatformsService(
                repository, 
                validator,
                templatesResponse, 
                validationParameters);

            // Строка для поиска
            var searchLoc = parameters.SearchLocation;

            /////////
            // Act //
            /////////

            // Поиск рекламных площадок
            (var listAdvertisingPlatforms,var error) = await service.Search(searchLoc);

            ////////////
            // Assert //
            ////////////
            
            // Если данные не прошли валидацию
            if (!parameters.ValidationResult)
            {
                Assert.Null(listAdvertisingPlatforms);
                Assert.NotNull(error);
                return;
            }

            // Если данные прошли валидацию и мы получили ответ
            
            // Проверка на наличие ожидаемого результата
            if(parameters.Result is null)
            {
                Assert.Fail("Данные для проверки не могут быть пустыми. Тест:" + parameters.ID_test);
            }

            // Если полученный результат отсутствует
            if(listAdvertisingPlatforms is null)
            {
                Assert.Fail(
                    $"""
                    Полученный результат не может быть пустым. 
                    Тест: {parameters.ID_test}
                    Ожидалось: {parameters.Result}
                    """);
            }

            var result = parameters.Result;
            var count_res = result.Count;
            var count_get = listAdvertisingPlatforms.Count;

            // Проверка на кол-во вернувшихся локаций
            if (count_res != count_get)
            {
                Assert.Fail(
                    $"""
                        Кол-во локаций не совпадает с ожидаемым кол-вом.  
                        Ожидалось: {count_res} 
                        Получено: {count_get}
                        """);
            }

            // пеебираем локации и проверяем рекламные площадки
            for (int i = 0; i < count_res; i++)
            {
                // Возвращаемые локации отсортированы по уменьшению кол-ва подлокаций
                var ap_res = result[i];
                var ap_get = listAdvertisingPlatforms[i];

                var loc_res = ap_res.NameLocation.Trim();
                var loc_get = ap_get.NameLocation.Trim();


                // Сравнение названий локаций
                if (loc_res != loc_get)
                {
                    Assert.Fail(
                        $"""
                            Названия локаций не совпадают. 
                            Ожидалось: {loc_res} 
                            Получено: {loc_get}
                            Ошибка в {i + 1} группе рекламных площадок.
                            Ожидаемый объект: {ap_res}
                            Полученный объект: {ap_get}
                            """);
                }

                var list_ap_res = ap_res.AdvertisingPlatforms;
                var list_ap_get = ap_get.AdvertisingPlatforms;

                // Сравнение кол-ва рекланых площадок
                if (list_ap_res.Count != list_ap_get.Count)
                {
                    Assert.Fail(
                        $"""
                            Кол-во рекламных площадок не совпадает с ожидаемым кол-вом. 
                            Ожидалось: {list_ap_res.Count} 
                            Получено: {list_ap_get.Count}
                            Ошибка в {i + 1} группе рекламных площадок.
                            Ожидаемый объект: {ap_res}
                            Полученный объект: {ap_get}
                            """);
                }

                // Сравнение списка рекламных площадок
                for (int j = 0; j < list_ap_res.Count; j++)
                {
                    var name_ap_res = list_ap_res[j].Trim();
                    var name_ap_get = list_ap_get[j].Trim();
                    // Если хоть одна не совпала это ошибка
                    if (name_ap_res != name_ap_get)
                    {
                        Assert.Fail(
                            $"""
                                Список рекламных площадок не совпадает. 
                                Ожидалось: {name_ap_res} 
                                Получено: {name_ap_get}
                                Ошибка в {i + 1} группе рекламных площадок.
                                Ожидаемый объект: {ap_res}
                                Полученный объект: {ap_get}
                                """);
                    }
                }
            }

            // Тест прошёл успешно
            Assert.True(true);
        }


        /// <summary>
        /// Тестовые данные
        /// </summary>
        public static IEnumerable<object[]> GetParams()
        {
            // Тест на правильный ответ
            // Строгие тербования только нижний регистр и без повторений локаций
            yield return [new DTO_AdvertisingPlatformsService_Params(1) {
                // Параметры валидации данных файла
                AllowingTheUseOfCapitalLetters = false,
                CapitaLetterSensitivity = false,
                LocationsWithTheSameName = false,
                RepeatingSubLocations = false,
                // Хранилище для поиска
                Content = "Item#0: /ru",
                // Строка для поиска
                SearchLocation = "/ru",
                // Ожидаемый результат валидации строки
                ValidationResult = true,
                // Ожидаемый результат поиска
                Result = new List<AdvertisingPlatforms_Test>() {
                    new AdvertisingPlatforms_Test(){
                        NameLocation = "/ru",
                        AdvertisingPlatforms = new List<string>(){
                            "Item#0"
                        }
                    }
                }
            }];

            // Тест на правильный ответ
            // Базовый запрос с параметрами по умолчанию.
            // С локацией для поиска, которой нет в хранилище,
            // но с подлокацией, которая есть в хранилище
            yield return
                [
                    new DTO_AdvertisingPlatformsService_Params(2){
                        // Параметры валидации данных файла
                        AllowingTheUseOfCapitalLetters = false,
                        CapitaLetterSensitivity = false,
                        LocationsWithTheSameName = false,
                        RepeatingSubLocations = false,
                        // Хранилище для поиска
                        Content = "Item#0: /ru",
                        // Строка поиска
                        SearchLocation = "/ru/pz",
                        // Ожидаемый ответ поиска по локации
                        ValidationResult = true,
                        // Ожидаемый результат запроса
                        Result = new List<AdvertisingPlatforms_Test>() {
                            new AdvertisingPlatforms_Test(){
                                NameLocation = "/ru",
                                AdvertisingPlatforms = new List<string>(){
                                    "Item#0"
                                }
                            }
                        }
                    }
                ];

            // Тест на правильный ответ
            // Базовый запрос с параметрами по умолчанию
            // Пустой ответ
            yield return
                [
                    new DTO_AdvertisingPlatformsService_Params(3){
                        // Параметры валидации данных файла
                        AllowingTheUseOfCapitalLetters = false,
                        CapitaLetterSensitivity = false,
                        LocationsWithTheSameName = false,
                        RepeatingSubLocations = false,
                        // Хранилище для поиска
                        Content = "Item#0: /ru",
                        // Строка поиска
                        SearchLocation = "/pz",
                        // Ожидаемый ответ поиска по локации
                        ValidationResult = true,
                        // Ожидаемый результат запроса
                        Result = new List<AdvertisingPlatforms_Test>()
                    }
                ];

            // Тест на не правильный ответ
            // Базовый запрос с параметрами по умолчанию
            // Ошибка в валидации локации для поиска
            // Не правильно записана строка для поиска
            yield return
                [
                    new DTO_AdvertisingPlatformsService_Params(4){
                        // Параметры валидации данных файла
                        AllowingTheUseOfCapitalLetters = false,
                        CapitaLetterSensitivity = false,
                        LocationsWithTheSameName = false,
                        RepeatingSubLocations = false,
                        // Хранилище для поиска
                        Content = "Item#0: /ru, /ru/kz",
                        // Строка поиска
                        SearchLocation = "pz", // ошибка
                        // Ожидаемый ответ поиска по локации
                        ValidationResult = false,
                        // Ожидаемый результат запроса
                        Result = null
                    }
                ];

            // Тест на не правильный ответ
            // Базовый запрос с параметрами по умолчанию
            // Ошибка в валидации локации для поиска
            // Не правильно записана строка для поиска, пробел не нужен
            yield return
                [
                    new DTO_AdvertisingPlatformsService_Params(5){
                        // Параметры валидации данных файла
                        AllowingTheUseOfCapitalLetters = false,
                        CapitaLetterSensitivity = false,
                        LocationsWithTheSameName = false,
                        RepeatingSubLocations = false,
                        // Хранилище для поиска
                        Content = "Item#0: /ru, /ru/kz",
                        // Строка поиска
                        SearchLocation = "/ru /kz", // ошибка
                        // Ожидаемый ответ поиска по локации
                        ValidationResult = false,
                        // Ожидаемый результат запроса
                        Result = null
                    }
                ];

            // Тест на не правильный ответ
            // Базовый запрос с параметрами по умолчанию
            // Ошибка в валидации локации для поиска
            // Исползован верхний регистр, а он запрещён
            yield return
                [
                    new DTO_AdvertisingPlatformsService_Params(6){
                        // Параметры валидации данных файла
                        AllowingTheUseOfCapitalLetters = false,
                        CapitaLetterSensitivity = false,
                        LocationsWithTheSameName = false,
                        RepeatingSubLocations = false,
                        // Хранилище для поиска
                        Content = "Item#0: /ru",
                        // Строка поиска
                        SearchLocation = "/RU", // ошибка
                        // Ожидаемый ответ поиска по локации
                        ValidationResult = false,
                        // Ожидаемый результат запроса
                        Result = null
                    }
                ];

            // Тест на не правильный ответ
            // Базовый запрос с параметрами по умолчанию
            // Ошибка в валидации локации для поиска
            // Дублирование подлокаций в полной локации
            yield return
                [
                    new DTO_AdvertisingPlatformsService_Params(7){
                        // Параметры валидации данных файла
                        AllowingTheUseOfCapitalLetters = false,
                        CapitaLetterSensitivity = false,
                        LocationsWithTheSameName = false,
                        RepeatingSubLocations = false,
                        // Хранилище для поиска
                        Content = "Item#0: /ru",
                        // Строка поиска
                        SearchLocation = "/ru/ru", // ошибка
                        // Ожидаемый ответ поиска по локации
                        ValidationResult = false,
                        // Ожидаемый результат запроса
                        Result = null
                    }
                ];

            //=================================================================//
            // - Разрешено использование верхнего регистра,
            // но система не чуствительна к нему
            //=================================================================//

            // Тест на правильный ответ
            // Базовый запрос. Разрешён верхний регистр, но он не учитывается
            yield return
                [
                    new DTO_AdvertisingPlatformsService_Params(8){
                        // Параметры валидации данных файла
                        AllowingTheUseOfCapitalLetters = true,
                        CapitaLetterSensitivity = false,
                        LocationsWithTheSameName = false,
                        RepeatingSubLocations = false,
                        // Хранилище для поиска
                        Content = "Item#0: /Ru",
                        // Строка поиска
                        SearchLocation = "/rU",
                        // Ожидаемый ответ поиска по локации
                        ValidationResult = true,
                        // Ожидаемый результат запроса
                        Result = new List<AdvertisingPlatforms_Test>() {
                            new AdvertisingPlatforms_Test(){
                                NameLocation = "/ru",
                                AdvertisingPlatforms = new List<string>(){
                                    "Item#0"
                                }
                            }
                        }
                    }
                ];

            // Тест на правильный ответ
            // Базовый запрос. Разрешён верхний регистр, но он не учитывается
            // С локацией для поиска, которой нет в хранилище,
            // но с подлокацией, которая есть в хранилище
            yield return
                [
                    new DTO_AdvertisingPlatformsService_Params(9){
                        // Параметры валидации данных файла
                        AllowingTheUseOfCapitalLetters = true,
                        CapitaLetterSensitivity = false,
                        LocationsWithTheSameName = false,
                        RepeatingSubLocations = false,
                        // Хранилище для поиска
                        Content = "Item#0: /rU",
                        // Строка поиска
                        SearchLocation = "/ru/PZ",
                        // Ожидаемый ответ поиска по локации
                        ValidationResult = true,
                        // Ожидаемый результат запроса
                        Result = new List<AdvertisingPlatforms_Test>() {
                            new AdvertisingPlatforms_Test(){
                                NameLocation = "/ru",
                                AdvertisingPlatforms = new List<string>(){
                                    "Item#0"
                                }
                            }
                        }
                    }
                ];

            // Тест на правильный ответ
            // Базовый запрос. Разрешён верхний регистр, но он не учитывается
            // Пустой ответ
            yield return
                [
                    new DTO_AdvertisingPlatformsService_Params(10){
                        // Параметры валидации данных файла
                        AllowingTheUseOfCapitalLetters = true,
                        CapitaLetterSensitivity = false,
                        LocationsWithTheSameName = false,
                        RepeatingSubLocations = false,
                        // Хранилище для поиска
                        Content = "Item#0: /Ru",
                        // Строка поиска
                        SearchLocation = "/pZ",
                        // Ожидаемый ответ поиска по локации
                        ValidationResult = true,
                        // Ожидаемый результат запроса
                        Result = new List<AdvertisingPlatforms_Test>()
                    }
                ];

            // Тест на не правильный ответ
            // Базовый запрос. Разрешён верхний регистр, но он не учитывается
            // Ошибка в валидации локации для поиска
            // Не правильно записана строка для поиска
            yield return
                [
                    new DTO_AdvertisingPlatformsService_Params(11){
                        // Параметры валидации данных файла
                        AllowingTheUseOfCapitalLetters = true,
                        CapitaLetterSensitivity = false,
                        LocationsWithTheSameName = false,
                        RepeatingSubLocations = false,
                        // Хранилище для поиска
                        Content = "Item#0: /ru",
                        // Строка поиска
                        SearchLocation = "PZ", // ошибка
                        // Ожидаемый ответ поиска по локации
                        ValidationResult = false,
                        // Ожидаемый результат запроса
                        Result = null
                    }
                ];

            // Тест на не правильный ответ
            // Базовый запрос. Разрешён верхний регистр, но он не учитывается
            // Ошибка в валидации локации для поиска
            // Не правильно записана строка для поиска, пробел не нужен
            yield return
                [
                    new DTO_AdvertisingPlatformsService_Params(12){
                        // Параметры валидации данных файла
                        AllowingTheUseOfCapitalLetters = true,
                        CapitaLetterSensitivity = false,
                        LocationsWithTheSameName = false,
                        RepeatingSubLocations = false,
                        // Хранилище для поиска
                        Content = "Item#0: /ru, /ru/kz",
                        // Строка поиска
                        SearchLocation = "/rU /Kz", // ошибка
                        // Ожидаемый ответ поиска по локации
                        ValidationResult = false,
                        // Ожидаемый результат запроса
                        Result = null
                    }
                ];

            // Тест на не правильный ответ
            // Базовый запрос. Разрешён верхний регистр, но он учитывается
            // Ошибка в валидации локации для поиска
            // Дублирование подлокаций в полной локации
            yield return
                [
                    new DTO_AdvertisingPlatformsService_Params(13){
                        // Параметры валидации данных файла
                        AllowingTheUseOfCapitalLetters = true,
                        CapitaLetterSensitivity = false,
                        LocationsWithTheSameName = false,
                        RepeatingSubLocations = false,
                        // Хранилище для поиска
                        Content = "Item#0: /rU",
                        // Строка поиска
                        SearchLocation = "/rU/Ru", // ошибка
                        // Ожидаемый ответ поиска по локации
                        ValidationResult = false,
                        // Ожидаемый результат запроса
                        Result = null
                    }
                ];

            //=================================================================//
            // - Разрешено использование верхнего регистра,
            // и система чуствительна к нему
            //=================================================================//

            // Тест на правильный ответ
            // Базовый запрос. Разрешён верхний регистр, но он учитывается
            yield return
                [
                    new DTO_AdvertisingPlatformsService_Params(14){
                        // Параметры валидации данных файла
                        AllowingTheUseOfCapitalLetters = true,
                        CapitaLetterSensitivity = true,
                        LocationsWithTheSameName = false,
                        RepeatingSubLocations = false,
                        // Хранилище для поиска
                        Content = "Item#0: /Ru",
                        // Строка поиска
                        SearchLocation = "/Ru",
                        // Ожидаемый ответ поиска по локации
                        ValidationResult = true,
                        // Ожидаемый результат запроса
                        Result = new List<AdvertisingPlatforms_Test>() {
                            new AdvertisingPlatforms_Test(){
                                NameLocation = "/Ru",
                                AdvertisingPlatforms = new List<string>(){
                                    "Item#0"
                                }
                            }
                        }
                    }
                ];

            // Тест на правильный ответ
            // Базовый запрос. Разрешён верхний регистр, но он учитывается
            // С локацией для поиска, которой нет в хранилище,
            // но с подлокацией, которая есть в хранилище
            yield return
                [
                    new DTO_AdvertisingPlatformsService_Params(15){
                        // Параметры валидации данных файла
                        AllowingTheUseOfCapitalLetters = true,
                        CapitaLetterSensitivity = true,
                        LocationsWithTheSameName = false,
                        RepeatingSubLocations = false,
                        // Хранилище для поиска
                        Content = "Item#0: /rU",
                        // Строка поиска
                        SearchLocation = "/rU/PZ",
                        // Ожидаемый ответ поиска по локации
                        ValidationResult = true,
                        // Ожидаемый результат запроса
                        Result = new List<AdvertisingPlatforms_Test>() {
                            new AdvertisingPlatforms_Test(){
                                NameLocation = "/rU",
                                AdvertisingPlatforms = new List<string>(){
                                    "Item#0"
                                }
                            }
                        }
                    }
                ];

            // Тест на правильный ответ
            // Базовый запрос. Разрешён верхний регистр, но он учитывается
            // С локацией для поиска, которой нет в хранилище,
            // но с подлокацией, которая есть в хранилище
            // Так же тут повторение, но в разном регистре поэтому это уже не повторение
            yield return
                [
                    new DTO_AdvertisingPlatformsService_Params(16){
                        // Параметры валидации данных файла
                        AllowingTheUseOfCapitalLetters = true,
                        CapitaLetterSensitivity = true,
                        LocationsWithTheSameName = false,
                        RepeatingSubLocations = false,
                        // Хранилище для поиска
                        Content = "Item#0: /rU",
                        // Строка поиска
                        SearchLocation = "/rU/Ru",
                        // Ожидаемый ответ поиска по локации
                        ValidationResult = true,
                        // Ожидаемый результат запроса
                        Result = new List<AdvertisingPlatforms_Test>() {
                            new AdvertisingPlatforms_Test(){
                                NameLocation = "/rU",
                                AdvertisingPlatforms = new List<string>(){
                                    "Item#0"
                                }
                            }
                        }
                    }
                ];

            // Тест на правильный ответ
            // Базовый запрос. Разрешён верхний регистр, но он учитывается
            // Пустой ответ
            yield return
                [
                    new DTO_AdvertisingPlatformsService_Params(17){
                        // Параметры валидации данных файла
                        AllowingTheUseOfCapitalLetters = true,
                        CapitaLetterSensitivity = true,
                        LocationsWithTheSameName = false,
                        RepeatingSubLocations = false,
                        // Хранилище для поиска
                        Content = "Item#0: /ru",
                        // Строка поиска
                        SearchLocation = "/RU",
                        // Ожидаемый ответ поиска по локации
                        ValidationResult = true,
                        // Ожидаемый результат запроса
                        Result = new List<AdvertisingPlatforms_Test>()
                    }
                ];

            // Тест на не правильный ответ
            // Базовый запрос. Разрешён верхний регистр, но он учитывается
            // Ошибка в валидации локации для поиска
            // Не правильно записана строка для поиска
            yield return
                [
                    new DTO_AdvertisingPlatformsService_Params(18){
                        // Параметры валидации данных файла
                        AllowingTheUseOfCapitalLetters = true,
                        CapitaLetterSensitivity = true,
                        LocationsWithTheSameName = false,
                        RepeatingSubLocations = false,
                        // Хранилище для поиска
                        Content = "Item#0: /ru",
                        // Строка поиска
                        SearchLocation = "PZ", // ошибка
                        // Ожидаемый ответ поиска по локации
                        ValidationResult = false,
                        // Ожидаемый результат запроса
                        Result = null
                    }
                ];

            // Тест на не правильный ответ
            // Базовый запрос. Разрешён верхний регистр, но он учитывается
            // Ошибка в валидации локации для поиска
            // Не правильно записана строка для поиска, пробел не нужен
            yield return
                [
                    new DTO_AdvertisingPlatformsService_Params(19){
                        // Параметры валидации данных файла
                        AllowingTheUseOfCapitalLetters = true,
                        CapitaLetterSensitivity = true,
                        LocationsWithTheSameName = false,
                        RepeatingSubLocations = false,
                        // Хранилище для поиска
                        Content = "Item#0: /ru, /RU/kz",
                        // Строка поиска
                        SearchLocation = "/RU /kz", // ошибка
                        // Ожидаемый ответ поиска по локации
                        ValidationResult = false,
                        // Ожидаемый результат запроса
                        Result = null
                    }
                ];

            // Тест на не правильный ответ
            // Базовый запрос. Разрешён верхний регистр, но он учитывается
            // Ошибка в валидации локации для поиска
            // Дублирование подлокаций в полной локации
            yield return
                [
                    new DTO_AdvertisingPlatformsService_Params(20){
                        // Параметры валидации данных файла
                        AllowingTheUseOfCapitalLetters = true,
                        CapitaLetterSensitivity = true,
                        LocationsWithTheSameName = false,
                        RepeatingSubLocations = false,
                        // Хранилище для поиска
                        Content = "Item#0: /rU",
                        // Строка поиска
                        SearchLocation = "/rU/rU", // ошибка
                        // Ожидаемый ответ поиска по локации
                        ValidationResult = false,
                        // Ожидаемый результат запроса
                        Result = null
                    }
                ];


            //=================================================================//
            // - Разрешено использование верхнего регистра,
            // но система не чуствительна к нему
            // - Разрешены использование подлокаций с одинаковыми названиями
            // в конце, например: /ru/fp и /kz/fp - но без повторений
            // В одной локации /ru/ru -> ошибка
            //=================================================================//

            // Тест на правильный ответ
            // Базовый запрос. Разрешён верхний регистр, но он не учитывается
            // Разрешены повторения в разных локациях
            yield return
                [
                    new DTO_AdvertisingPlatformsService_Params(21){
                        // Параметры валидации данных файла
                        AllowingTheUseOfCapitalLetters = true,
                        CapitaLetterSensitivity = false,
                        LocationsWithTheSameName = true,
                        RepeatingSubLocations = false,
                        // Хранилище для поиска
                        Content =
                            """
                            Item#0: /ru
                            Item#1: /ru, /kz/ru
                            """,
                        // Строка поиска
                        SearchLocation = "/ru",
                        // Ожидаемый ответ поиска по локации
                        ValidationResult = true,
                        // Ожидаемый результат запроса
                        Result = new List<AdvertisingPlatforms_Test>() {
                            new AdvertisingPlatforms_Test(){
                                NameLocation = "/ru",
                                AdvertisingPlatforms = new List<string>(){
                                    "Item#0",
                                    "Item#1"
                                }
                            }
                        }
                    }
                ];

            // Тест на правильный ответ
            // Базовый запрос. Разрешён верхний регистр, но он не учитывается
            // Разрешены повторения в разных локациях
            // С локацией для поиска, которой нет в хранилище,
            // но с подлокацией, которая есть в хранилище
            yield return
                [
                    new DTO_AdvertisingPlatformsService_Params(22){
                        // Параметры валидации данных файла
                        AllowingTheUseOfCapitalLetters = true,
                        CapitaLetterSensitivity = false,
                        LocationsWithTheSameName = true,
                        RepeatingSubLocations = false,
                        // Хранилище для поиска
                        Content =
                            """
                            Item#0: /rU
                            Item#1: /Ru, /kz/ru
                            """,
                        // Строка поиска
                        SearchLocation = "/rU/pz",
                        // Ожидаемый ответ поиска по локации
                        ValidationResult = true,
                        // Ожидаемый результат запроса
                        Result = new List<AdvertisingPlatforms_Test>() {
                            new AdvertisingPlatforms_Test(){
                                NameLocation = "/ru",
                                AdvertisingPlatforms = new List<string>(){
                                    "Item#0",
                                    "Item#1"
                                }
                            }
                        }
                    }
                ];

            // Тест на правильный ответ
            // Базовый запрос. Разрешён верхний регистр, но он не учитывается
            // Разрешены повторения в разных локациях
            // Пустой ответ
            yield return
                [
                    new DTO_AdvertisingPlatformsService_Params(23){
                        // Параметры валидации данных файла
                        AllowingTheUseOfCapitalLetters = true,
                        CapitaLetterSensitivity = false,
                        LocationsWithTheSameName = true,
                        RepeatingSubLocations = false,
                        // Хранилище для поиска
                        Content =
                            """
                            Item#0: /rU
                            Item#1: /Ru, /kz/RU
                            """,
                        // Строка поиска
                        SearchLocation = "/pz",
                        // Ожидаемый ответ поиска по локации
                        ValidationResult = true,
                        // Ожидаемый результат запроса
                        Result = new List<AdvertisingPlatforms_Test>()
                    }
                ];

            // Тест на не правильный ответ
            // Базовый запрос. Разрешён верхний регистр, но он не учитывается
            // Разрешены повторения в разных локациях
            // Ошибка в валидации локации для поиска
            // Не правильно записана строка для поиска
            yield return
                [
                    new DTO_AdvertisingPlatformsService_Params(24){
                        // Параметры валидации данных файла
                        AllowingTheUseOfCapitalLetters = true,
                        CapitaLetterSensitivity = false,
                        LocationsWithTheSameName = true,
                        RepeatingSubLocations = false,
                        // Хранилище для поиска
                        Content =
                            """
                            Item#0: /rU
                            Item#1: /Ru, /kz/RU
                            """,
                        // Строка поиска
                        SearchLocation = "pz", // ошибка
                        // Ожидаемый ответ поиска по локации
                        ValidationResult = false,
                        // Ожидаемый результат запроса
                        Result = null
                    }
                ];


            // Тест на не правильный ответ
            // Базовый запрос. Разрешён верхний регистр, но он не учитывается
            // Разрешены повторения в разных локациях
            // Ошибка в валидации локации для поиска
            // Не правильно записана строка для поиска, пробел не нужен
            yield return
                [
                    new DTO_AdvertisingPlatformsService_Params(25){
                        // Параметры валидации данных файла
                        AllowingTheUseOfCapitalLetters = true,
                        CapitaLetterSensitivity = false,
                        LocationsWithTheSameName = true,
                        RepeatingSubLocations = false,
                        // Хранилище для поиска
                        Content =
                            """
                            Item#0: /rU
                            Item#1: /Ru, /kz/RU
                            """,
                        // Строка поиска
                        SearchLocation = "/kz /RU", // ошибка
                        // Ожидаемый ответ поиска по локации
                        ValidationResult = false,
                        // Ожидаемый результат запроса
                        Result = null
                    }
                ];

            // Тест на не правильный ответ
            // Базовый запрос. Разрешён верхний регистр, но он не учитывается
            // Разрешены повторения в разных локациях
            // Ошибка в валидации локации для поиска
            // Дублирование подлокаций в полной локации
            yield return
                [
                    new DTO_AdvertisingPlatformsService_Params(26){
                        // Параметры валидации данных файла
                        AllowingTheUseOfCapitalLetters = true,
                        CapitaLetterSensitivity = false,
                        LocationsWithTheSameName = true,
                        RepeatingSubLocations = false,
                        // Хранилище для поиска
                        Content =
                            """
                            Item#0: /rU
                            Item#1: /Ru, /kz/RU
                            """,
                        // Строка поиска
                        SearchLocation = "/rU/Ru", // ошибка
                        // Ожидаемый ответ поиска по локации
                        ValidationResult = false,
                        // Ожидаемый результат запроса
                        Result = null
                    }
                ];

            //=================================================================//
            // - Разрешено использование верхнего регистра,
            // но система не чуствительна к нему
            // - Разрешены использование подлокаций с одинаковыми названиями подлокаций,
            // например: /ru/fp и /kz/fp 
            // В одной локации /ru/ru и /ru.../ru/ru -> валидно
            //=================================================================//

            // Тест на правильный ответ
            // Базовый запрос. Разрешён верхний регистр, но он не учитывается
            // Разрешены повторения 
            yield return
                [
                    new DTO_AdvertisingPlatformsService_Params(27){
                        // Параметры валидации данных файла
                        AllowingTheUseOfCapitalLetters = true,
                        CapitaLetterSensitivity = false,
                        LocationsWithTheSameName = true,
                        RepeatingSubLocations = true,
                        // Хранилище для поиска
                        Content =
                            """
                            Item#0: /ru
                            Item#1: /ru/ru
                            """,
                        // Строка поиска
                        SearchLocation = "/rU/Ru",
                        // Ожидаемый ответ поиска по локации
                        ValidationResult = true,
                        // Ожидаемый результат запроса
                        Result = new List<AdvertisingPlatforms_Test>() {
                            new AdvertisingPlatforms_Test(){
                                NameLocation = "/ru/ru",
                                AdvertisingPlatforms = new List<string>(){
                                    "Item#1"
                                }
                            },
                            new AdvertisingPlatforms_Test(){
                                NameLocation = "/ru",
                                AdvertisingPlatforms = new List<string>(){
                                    "Item#0"
                                }
                            }
                        }
                    }
                ];

            // Тест на правильный ответ
            // Базовый запрос. Разрешён верхний регистр, но он не учитывается
            // Разрешены повторения
            // С локацией для поиска, которой нет в хранилище,
            // но с подлокацией, которая есть в хранилище
            yield return
                [
                    new DTO_AdvertisingPlatformsService_Params(28){
                        // Параметры валидации данных файла
                        AllowingTheUseOfCapitalLetters = true,
                        CapitaLetterSensitivity = false,
                        LocationsWithTheSameName = true,
                        RepeatingSubLocations = true,
                        // Хранилище для поиска
                        Content =
                            """
                            Item#0: /ru
                            Item#1: /ru/ru
                            """,
                        // Строка поиска
                        SearchLocation = "/rU/RU/Ru",
                        // Ожидаемый ответ поиска по локации
                        ValidationResult = true,
                        // Ожидаемый результат запроса
                        Result = new List<AdvertisingPlatforms_Test>() {
                            new AdvertisingPlatforms_Test(){
                                NameLocation = "/ru/ru",
                                AdvertisingPlatforms = new List<string>(){
                                    "Item#1"
                                }
                            },
                            new AdvertisingPlatforms_Test(){
                                NameLocation = "/ru",
                                AdvertisingPlatforms = new List<string>(){
                                    "Item#0"
                                }
                            }
                        }
                    }
                ];

            // Тест на правильный ответ
            // Базовый запрос. Разрешён верхний регистр, но он не учитывается
            // Разрешены повторения
            // Пустой ответ
            yield return
                [
                    new DTO_AdvertisingPlatformsService_Params(29){
                        // Параметры валидации данных файла
                        AllowingTheUseOfCapitalLetters = true,
                        CapitaLetterSensitivity = false,
                        LocationsWithTheSameName = true,
                        RepeatingSubLocations = true,
                        // Хранилище для поиска
                        Content =
                            """
                            Item#0: /ru
                            Item#1: /ru/ru
                            """,
                        // Строка поиска
                        SearchLocation = "/PZ",
                        // Ожидаемый ответ поиска по локации
                        ValidationResult = true,
                        // Ожидаемый результат запроса
                        Result = new List<AdvertisingPlatforms_Test>()
                    }
                ];

            // Тест на не правильный ответ
            // Базовый запрос. Разрешён верхний регистр, но он не учитывается
            // Разрешены повторения
            // Ошибка в валидации локации для поиска
            // Не правильно записана строка для поиска
            yield return
                [
                    new DTO_AdvertisingPlatformsService_Params(30){
                        // Параметры валидации данных файла
                        AllowingTheUseOfCapitalLetters = true,
                        CapitaLetterSensitivity = false,
                        LocationsWithTheSameName = true,
                        RepeatingSubLocations = true,
                        // Хранилище для поиска
                        Content =
                            """
                            Item#0: /ru
                            Item#1: /ru/ru
                            """,
                        // Строка поиска
                        SearchLocation = "pz", // ошибка
                        // Ожидаемый ответ поиска по локации
                        ValidationResult = false,
                        // Ожидаемый результат запроса
                        Result = null
                    }
                ];

            // Тест на не правильный ответ
            // Базовый запрос. Разрешён верхний регистр, но он не учитывается
            // Разрешены повторения
            // Ошибка в валидации локации для поиска
            // Не правильно записана строка для поиска, пробел не нужен
            yield return
                [
                    new DTO_AdvertisingPlatformsService_Params(31){
                        // Параметры валидации данных файла
                        AllowingTheUseOfCapitalLetters = true,
                        CapitaLetterSensitivity = false,
                        LocationsWithTheSameName = true,
                        RepeatingSubLocations = true,
                        // Хранилище для поиска
                        Content =
                            """
                            Item#0: /ru
                            Item#1: /ru/ru
                            """,
                        // Строка поиска
                        SearchLocation = "/ru /ru", // ошибка
                        // Ожидаемый ответ поиска по локации
                        ValidationResult = false,
                        // Ожидаемый результат запроса
                        Result = null
                    }
                ];
        }
    }
}
