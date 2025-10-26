using AdvertisingPlatforms.Tests.Integration_Tests.Controllers_Tests.AdvertisingPlatformsController.DTO;
using AdvertisingPlatforms.Tests.TestResources.Models;
using System.Net;
using Xunit;


namespace AdvertisingPlatforms.Tests.Integration_Tests.AdvertisingPlatformsController
{
    public partial class Tests_AdvertisingPlatformsController
    {
        [Theory]
        [MemberData(nameof(GetParams_UploadFile))]
        public async Task Test_02_POST_UploadFile(DTO_UploadFile_Params parameters)
        {
            //_output.WriteLine($"UploadFile: {fileParameters.ID_test}  {DateTime.Now.Ticks}");

            /////////////
            // Arrange //
            /////////////
            
            //Инициализация параметров приложения
            parameters.SetAppParameters(_parameters);
            // Создание формы для отправки в клиент
            using var multipartFormContent = parameters.GetFormDataContent();

            //Получение параметров системы для сравнения с тестируемыми установками
            var response_getParams = await _client.GetAsync("/api/advertising_platforms/application_parameters");
            var appParameters = await response_getParams.Content.ReadFromJsonAsync<AppParameters_Test>();

            /////////
            // Act //
            /////////

            // Тестируемый запрос
            // Загрузка файла 
            var responce_postUploadFile = await _client.PostAsync("/api/advertising_platforms", multipartFormContent);
            var responce_result = await responce_postUploadFile.Content.ReadAsStringAsync();

            ////////////
            // Assert //
            ////////////

            // Проверка, что параметры приложения получены и они соответствуют установкам теста
            Assert.Equal(HttpStatusCode.OK, response_getParams.StatusCode);
            Assert.Equal(_parameters, appParameters);

            // Проверка результата загрузки файла с заданными ожиданиями
            Assert.Equal(parameters.CorrectResultCodeUploadFile, responce_postUploadFile.StatusCode);
        }

        /// <summary>
        /// Шаблоны параметров для теста
        /// </summary>
        public static IEnumerable<object[]> GetParams_UploadFile()
        {
            // Тест на правильный ответ
            yield return
                [
                    new DTO_UploadFile_Params(1){
                        // Параметры валидации файла
                        AllowedExtensions = [".txt",".json"],
                        AllowedMimeTypes = ["text/plain","application/json"],
                        MaxSizeFile = 50 * 1024 * 1024,// 50 Мб - по умолчанию
                        // Значения файла
                        FileExtension = ".txt",
                        MIMETypeFile = "text/plain",
                        Content = "Item#0: /ru",
                        // Значения результата
                        CorrectResultCodeUploadFile = HttpStatusCode.OK
                    }
                ];

            // Тест на правильный ответ
            yield return
                [
                    new DTO_UploadFile_Params(2){
                        // Параметры валидации файла
                        AllowedExtensions = [".txt",".json"],
                        AllowedMimeTypes = ["text/plain","application/json"],
                        MaxSizeFile = 50 * 1024 * 1024,// 50 Мб - по умолчанию
                        // Значения файла
                        FileExtension = ".json",
                        MIMETypeFile = "text/plain",
                        Content = "Item#0: /ru",
                        // Ожидаемый ответ запроса
                        CorrectResultCodeUploadFile = HttpStatusCode.OK
                    }
                ];

            // Тест на не правильный ответ
            // Ошибка расшрения файла
            yield return
                [
                    new DTO_UploadFile_Params(3){
                        // Параметры валидации файла
                        AllowedExtensions = [".txt",".json"],
                        AllowedMimeTypes = ["text/plain","application/json"],
                        MaxSizeFile = 50 * 1024 * 1024,// 50 Мб - по умолчанию
                        // Значения файла
                        FileExtension = ".csv",// его нет в списке AllowedExtensions
                        MIMETypeFile = "text/plain",
                        Content = "Item#0: /ru",
                        // Ожидаемый ответ запроса
                        CorrectResultCodeUploadFile = HttpStatusCode.BadRequest
                    }
                ];

            // Тест на не правильный ответ
            // Ошибка в типе контента файла
            yield return
                [
                    new DTO_UploadFile_Params(4){
                        // Параметры валидации файла
                        AllowedExtensions = [".txt",".json",".csv"],
                        AllowedMimeTypes = ["text/plain","application/json"],
                        MaxSizeFile = 50 * 1024 * 1024,// 50 Мб - по умолчанию
                        // Значения файла
                        FileExtension = ".csv",
                        MIMETypeFile = "text/csv",// его нет в списке AllowedMimeTypes
                        Content = "Item#0: /ru",
                        // Ожидаемый ответ запроса
                        CorrectResultCodeUploadFile = HttpStatusCode.BadRequest
                    }
                ];

            // Тест на не правильный ответ
            // Ошибка в размере файла
            yield return
                [
                    new DTO_UploadFile_Params(5){
                        // Параметры валидации файла
                        AllowedExtensions = [".txt",".json"],
                        AllowedMimeTypes = ["text/plain","application/json"],
                        MaxSizeFile = 5,// байт
                        // Значения файла
                        FileExtension = ".txt",
                        MIMETypeFile = "text/plain",
                        Content = "Item#0: /ru",// контент файла больше 5 байт
                        // Ожидаемый ответ запроса
                        CorrectResultCodeUploadFile = HttpStatusCode.BadRequest
                    }
                ];
        }
    }
}
