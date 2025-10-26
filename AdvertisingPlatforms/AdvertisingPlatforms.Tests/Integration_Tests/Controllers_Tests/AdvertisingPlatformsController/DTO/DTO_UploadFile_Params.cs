using AdvertisingPlatforms.Core.Abstractions;
using AdvertisingPlatforms.Tests.TestResources.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace AdvertisingPlatforms.Tests.Integration_Tests.Controllers_Tests.AdvertisingPlatformsController.DTO
{
    /// <summary>
    /// Структура передачи параметров в тест, для тестирования контроллера рекламных площадок<br/>
    /// Параметры для тестирования загружаемого файла
    /// </summary>
    public class DTO_UploadFile_Params
        :IFileValidationParameters
    {
        /// <summary>
        /// Номер теста
        /// </summary>
        public int ID_test {  get; init; }

        #region Реализация интерфейса IFileValidationParameters
        public string[] AllowedExtensions { get; init; } = [".txt"];
        public string[] AllowedMimeTypes { get; init; } = ["text/plain"];
        public int MaxSizeFile { get; init; } = 50 * 1024 * 1024;// 50 Мб - по умолчанию
        #endregion
        /// <summary>
        /// Название контента файла в передаваемой форме
        /// <para>
        /// По умолчанию: <b>"file"</b>
        /// </para>
        /// </summary>
        public string ContentName { get; } = "file";
        /// <summary>
        /// Навзание файла
        /// <para>
        /// По умолчанию: <b>"test"</b>
        /// </para>
        /// </summary>
        public string FileName { get; init; } = "test";
        /// <summary>
        /// Расширение файла
        /// <para>
        /// По умолчанию: <b>".txt"</b>
        /// </para>
        /// </summary>
        public string FileExtension { get; init; } = ".txt";
        /// <summary>
        /// Тип контекста загружаемого файла
        /// <para>
        /// По умолчанию: <b>"text/plain"</b>
        /// </para>
        /// </summary>
        public string MIMETypeFile { get; init; } = "text/plain";
        /// <summary>
        /// Контент загружаемого файла
        /// <para>
        /// По умолчанию: <b>"Item#0: /ru"</b>
        /// </para>
        /// </summary>
        public string Content { get; set; } = "Item#0: /ru";
        /// <summary>
        /// Полное имя файла
        /// <para>
        /// Возвращает: <b><see cref="FileName">FileName</see>.Trim()+<see cref="FileExtension">FileExtension</see>.Trim()</b>
        /// </para>
        /// </summary>
        public string FullFileName { 
            get
            {
                return FileName.Trim()+FileExtension.Trim();
            } 
        }
        /// <summary>
        /// Ожидаемый резльтат загрузки файла
        /// <para>
        /// По умолчанию: <b><see cref="HttpStatusCode">HttpStatusCode</see>.OK</b>
        /// </para>
        /// </summary>
        public HttpStatusCode CorrectResultCodeUploadFile { get; init; } = HttpStatusCode.OK;

        /// <summary>
        /// Конструктор структуры для передачи данных инициализации в тест
        /// </summary>
        /// <param name="id">Индетификатор теста</param>
        public DTO_UploadFile_Params(int id = 0)
        {
            ID_test = id;
        }

        /// <summary>
        /// Метод создания отправляемой формы в клиент
        /// </summary>
        /// <returns>Отправляемый контент</returns>
        public MultipartFormDataContent GetFormDataContent()
        {
            var formData = new MultipartFormDataContent();
            byte[] arrContent = Encoding.UTF8.GetBytes(Content);
            var stream = new MemoryStream(arrContent);
            var fileStreamContent = new StreamContent(stream);
            fileStreamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MIMETypeFile);
            formData.Add(content: fileStreamContent,
                            name: ContentName,
                        fileName: FullFileName);
            return formData;
        }
        /// <summary>
        /// Установка параметров приложения
        /// </summary>
        /// <param name="app">Параметры приложения для изменения</param>
        public void SetAppParameters(AppParameters_Test app)
        {
            app.SetDefaultParameters();
            app.AllowedExtensions = AllowedExtensions;
            app.AllowedMimeTypes = AllowedMimeTypes;
            app.MaxSizeFile = MaxSizeFile;
        }
    }
}
