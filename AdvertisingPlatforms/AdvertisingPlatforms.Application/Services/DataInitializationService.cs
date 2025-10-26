


using AdvertisingPlatforms.Core.Abstractions;
using AdvertisingPlatforms.Core.Models;
using Microsoft.AspNetCore.Http;

namespace AdvertisingPlatforms.Application.Services
{
    public class DataInitializationService : IDataInitializationService
    {
        private readonly IResponseTemplates _responseTemplates;
        private readonly IAdvertisingPlatformValidation _validation;
        private readonly IStorageBuilder _builder;

        public DataInitializationService(IResponseTemplates responseTemplates,
                                         IAdvertisingPlatformValidation validation,
                                         IStorageBuilder builder)
        {
            _responseTemplates = responseTemplates;
            _validation = validation;
            _builder = builder;
        }
        /// <summary>
        /// Метод загрузки данных из файла
        /// </summary>
        /// <returns>Ошибка, если она есть</returns>
        public async Task<string?> UploadDataFile(IFormFile pathFile)
        {
            using (var reader = new StreamReader(pathFile.OpenReadStream()))
            {
                string? line;
                List<AdvertisingPlatformDTO> platforms = new();

                // Считываем по строчно файл
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    // Валидация, при успехе возвращает десериализованный объект
                    bool isValid = _validation.IsValid(line, out AdvertisingPlatformDTO? advertisingPlatformDTO);
                    // Если есть хоть одна не валидная строка, то файл не валиден.
                    if (isValid)
                    {
                        platforms.Add(advertisingPlatformDTO!);
                    }
                    else
                    {
                        string error = $"В файле одна или несколько ошибок, следуйте следующим правилам:\r\n{_responseTemplates.DataTemplateDescription}";
                        return error;
                    }

                }

                await _builder.CreateStorage(platforms);
            }

            return null;
        }
    }
}
