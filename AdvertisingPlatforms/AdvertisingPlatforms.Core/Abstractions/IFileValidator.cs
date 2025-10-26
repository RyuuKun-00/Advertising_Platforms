using Microsoft.AspNetCore.Http;

namespace AdvertisingPlatforms.Application.Validators
{
    /// <summary>
    /// Шаблон валидации загружаемого файла
    /// </summary>
    public interface IFileValidator
    {
        /// <summary>
        /// Проверка на соответствие ограничениям загружаемого файла
        /// </summary>
        /// <param name="uploadedFile">Файл для валидации</param>
        /// <param name="errorValid">Ошибка проверки файла</param>
        /// <returns><b>true</b> - если файл прошёл проверку, иначе: <b>false</b></returns>
        bool IsValid(IFormFile? uploadedFile, out string? errorValid);
    }
}