

namespace AdvertisingPlatforms.Core.Abstractions
{
    /// <summary>
    /// Шаблоны ответов в случае неуспешной валидации
    /// </summary>
    public interface IResponseTemplates
    {
        /// <summary>
        /// Описание шаблона данных в файле
        /// <para>
        /// Возвращаем эту строку, если в данных файла есть ошибка
        /// </para>
        /// </summary>
        string DataTemplateDescription { get; }
        /// <summary>
        /// Описание шаблона локации в файле
        /// <para>
        /// Возвращаем эту строку, если в локации есть ошибка
        /// </para>
        /// </summary>
        string LocationTemplateDescription { get; }
    }
}
