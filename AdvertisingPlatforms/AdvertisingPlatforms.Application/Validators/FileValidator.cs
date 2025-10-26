using AdvertisingPlatforms.Core.Abstractions;
using Microsoft.AspNetCore.Http;

namespace AdvertisingPlatforms.Application.Validators
{
    /// <summary>
    /// Реализация интерфейса <see cref="IFileValidator"/> <br/>
    /// Класс проверки загружаемого файла на соответвие установленным ограничениям
    /// </summary>
    public class FileValidator : IFileValidator
    {
        private readonly IFileValidationParameters _validationParameters;

        /// <summary>
        /// Конструктор класса валидации файла
        /// </summary>
        public FileValidator(IFileValidationParameters validationParameters)
        {
            _validationParameters = validationParameters;
        }
        /// <summary>
        /// Проверка на соответствие ограничениям загружаемого файла
        /// </summary>
        /// <param name="uploadedFile">Файл для валидации</param>
        /// <param name="errorValid">Ошибка проверки файла</param>
        /// <returns><b>true</b> - если файл прошёл проверку, иначе: <b>false</b></returns>
        public bool IsValid(IFormFile? uploadedFile, out string? errorValid)
        {
            // Проверка на наличие файла
            if (!IsExists(uploadedFile, out errorValid)) return false;
            // Проверка на допустимое расширение файла
            if (!IsValid_Extension(uploadedFile!, out errorValid)) return false;
            // Проверка на допустимый контент файла
            if (!IsValid_MIMEType(uploadedFile!, out errorValid)) return false;
            // Проверка на допустимый размер файла
            if (!IsValid_FileSize(uploadedFile!, out errorValid)) return false;

            return true;
        }

        /// <summary>
        /// Проверка на наличие файла
        /// </summary>
        /// <param name="formFile">Файл для валидации</param>
        /// <param name="error">Ошибка проверки файла</param>
        /// <returns><b>true</b> - если файл прошёл проверку, иначе: <b>false</b></returns>
        private bool IsExists(IFormFile? formFile, out string? error)
        {
            error = null;
            // Проверка на наличие файла
            if (formFile == null)
            {
                error = "Файл не обнаружен.";
                return false;
            }

            if (formFile.Length == 0)
            {
                error = "Файл пуст.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Проверка на допустимое расширение файла
        /// </summary>
        /// <param name="formFile">Файл для валидации</param>
        /// <param name="error">Ошибка проверки файла</param>
        /// <returns><b>true</b> - если файл прошёл проверку, иначе: <b>false</b></returns>
        private bool IsValid_Extension(IFormFile formFile, out string? error)
        {
            error = null;

            // Получение допустимых расширений файлов
            string[] allowedExtensions = _validationParameters.AllowedExtensions;

            // Отсутствие допустимых расширений -> разрешены все
            if (allowedExtensions.Length == 0) return true;

            string fileExtension = Path.GetExtension(formFile.FileName).ToLower();

            // Проверка на соответсвие допустимого расширения файла
            if (!allowedExtensions.Contains(fileExtension))
            {
                error = $"""
                        Недопустимый тип файла. 
                        Разрешены только: {String.Join(", ", allowedExtensions)}.
                        """;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Проверка на допустимый контент файла
        /// </summary>
        /// <param name="formFile">Файл для валидации</param>
        /// <param name="error">Ошибка проверки файла</param>
        /// <returns><b>true</b> - если файл прошёл проверку, иначе: <b>false</b></returns>
        private bool IsValid_MIMEType(IFormFile formFile, out string? error)
        {
            error = null;

            // Получение допутимых MIME типов
            string[] allowedMimeTypes = _validationParameters.AllowedMimeTypes;

            // Отсутствие допустимых MIME типов -> разрешены все
            if (allowedMimeTypes.Length == 0) return true;

            string mimeType = formFile.ContentType.ToLower();

            if (!allowedMimeTypes.Contains(mimeType))
            {
                error = $"""
                        Недопустимый контент файла. 
                        Разрешены только: {String.Join(", ", allowedMimeTypes)}.
                        """;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Проверка на допустимый размер файла
        /// </summary>
        /// <param name="formFile">Файл для валидации</param>
        /// <param name="error">Ошибка проверки файла</param>
        /// <returns><b>true</b> - если файл прошёл проверку, иначе: <b>false</b></returns>
        private bool IsValid_FileSize(IFormFile formFile, out string? error)
        {
            error = null;

            // Получение максимального размера файла
            int maxSize = _validationParameters.MaxSizeFile;
            if (maxSize == 0)
            {
                maxSize = 50 * 1024 * 1024;// 50 Мб\
            }

            // Проверка на размер файла
            if (formFile.Length > maxSize)
            {
                error = $"""
                        Недопустимый размер файла. 
                        Максимальный разрешенный размер: {maxSize} байт.
                        """;
                return false;
            }

            return true;
        }
    }
}
