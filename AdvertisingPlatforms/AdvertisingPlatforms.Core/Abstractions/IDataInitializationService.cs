using Microsoft.AspNetCore.Http;

namespace AdvertisingPlatforms.Core.Abstractions
{
    /// <summary>
    /// Шаблон сервиса загрзки данных из файла
    /// </summary>
    public interface IDataInitializationService
    {
        /// <summary>
        /// Метод загрузки данных из файла
        /// </summary>
        /// <returns>Ошибка, если она есть</returns>
        Task<string?> UploadDataFile(IFormFile pathFile);
    }
}