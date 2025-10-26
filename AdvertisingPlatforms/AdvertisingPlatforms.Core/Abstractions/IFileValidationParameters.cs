

namespace AdvertisingPlatforms.Core.Abstractions
{
    /// <summary>
    /// Шаблон параметров валидации файла
    /// </summary>
    public interface IFileValidationParameters
    {
        /// <summary>
        /// Список допустимых расширений файла<br/>
        /// Если список пуст, то допустимы все расширения
        /// <para>
        /// По умолчанию: <b>[".txt"]</b>
        /// </para>
        /// </summary>
        string[] AllowedExtensions { get; }
        /// <summary>
        /// Список допустимых MIME типов файла<br/>
        /// Если список пуст, то допустимы все MIME типы
        /// <para>
        /// По умолчанию: <b>["text/plain"]</b>
        /// </para>
        /// </summary>
        string[] AllowedMimeTypes { get; }
        /// <summary>
        /// Максимальный размер файла
        /// <para>
        /// По умолчанию: <b>50 Мб</b>
        /// </para>
        /// </summary>
        int MaxSizeFile { get; }
    }
}
