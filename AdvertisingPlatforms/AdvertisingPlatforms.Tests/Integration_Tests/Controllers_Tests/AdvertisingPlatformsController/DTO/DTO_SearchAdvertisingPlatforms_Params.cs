using AdvertisingPlatforms.Core.Abstractions;
using AdvertisingPlatforms.Tests.TestResources.Models;
using System.Net;

namespace AdvertisingPlatforms.Tests.Integration_Tests.Controllers_Tests.AdvertisingPlatformsController.DTO
{
    /// <summary>
    /// Структура передачи параметров в тест, для тестирования контроллера рекламных площадок<br/>
    /// Параметры для тестирования поиска рекламных площадок по локации
    /// </summary>
    public class DTO_SearchAdvertisingPlatforms_Params
        :DTO_UploadFile_Params,
        IAdvertisingPlatformValidationParameters
    {
        #region Реализация интерфейса IAdvertisingPlatformValidationParameters
        public bool AllowingTheUseOfCapitalLetters { get; init; } = false;

        public bool CapitaLetterSensitivity { get; init; } = false;

        public bool LocationsWithTheSameName { get; init; } = false;

        public bool RepeatingSubLocations { get; init; } = false;

        public string StringValidationPattern { get; } = string.Empty;

        #endregion

        /// <summary>
        /// Локация для поиска
        /// </summary>
        public string SearchLocation { get; init; } = string.Empty;


        /// <summary>
        /// Ожидаемый результат поиска локаций
        /// <para>
        /// По умолчанию: <b><see cref="HttpStatusCode">HttpStatusCode</see>.OK</b>
        /// </para>
        /// </summary>
        public HttpStatusCode CorrectResultCodeSearch { get; init; } = HttpStatusCode.OK;

        /// <summary>
        /// Ожидаемый результат выполнения запроса
        /// <br/> Если значение null или <see cref="CorrectResultCodeSearch">CorrectResultCodeSearch</see> 
        /// не равно <see cref="HttpStatusCode">HttpStatusCode</see>.OK,
        /// <br/> то тесты не проверяют ответ запроса
        /// <para>
        /// По умолчанию: <b>null</b>
        /// </para>
        /// </summary>
        public List<AdvertisingPlatforms_Test>? ResponceResult { get; init; } = null;

        

        public DTO_SearchAdvertisingPlatforms_Params(int id = 0) : base(id) { }

        /// <summary>
        /// Установка параметров приложения
        /// </summary>
        public new void SetAppParameters(AppParameters_Test app)
        {
            base.SetAppParameters(app);

            app.AllowingTheUseOfCapitalLetters = AllowingTheUseOfCapitalLetters;
            app.CapitaLetterSensitivity = CapitaLetterSensitivity;
            app.LocationsWithTheSameName = LocationsWithTheSameName;
            app.RepeatingSubLocations = RepeatingSubLocations;
        }
    }
}
