using AdvertisingPlatforms.Application.Validators;
using AdvertisingPlatforms.Core.Abstractions;
using AdvertisingPlatforms.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdvertisingPlatforms.Controllers
{
    /// <summary>
    /// Сонтроллер обработки запросов к рекламным площадкам
    /// </summary>
    [ApiController]
    [Route("api/advertising_platforms")]
    public class AdvertisingPlatformsController : ControllerBase
    {


        private readonly ILogger<AdvertisingPlatformsController> _logger;
        private readonly IFileValidator _fileValidator;
        private readonly IDataInitializationService _dataInitializationService;
        private readonly IAdvertisingPlatformsService _platformsService;
        private readonly IAppParameters _parameters;

        public AdvertisingPlatformsController(ILogger<AdvertisingPlatformsController> logger,
                                              IFileValidator fileValidator,
                                              IDataInitializationService dataInitializationService,
                                              IAdvertisingPlatformsService platformsService,
                                              IAppParameters parameters)
        {
            _logger = logger;
            _fileValidator = fileValidator;
            _dataInitializationService = dataInitializationService;
            _platformsService = platformsService;
            _parameters = parameters;
        }

        /// <summary>
        /// Метод загрузки файла для инициализации списка рекламных площадок
        /// </summary>
        /// <param name="file">Файл для инициализации рекламных площадок</param>
        /// <returns>Результат загрузки файла</returns>
        [HttpPost]
        public async Task<ActionResult<string?>> LoadingInitializationFile(IFormFile? file)
        {
            // Проверка на ограничения загружаемого файла
            if(!_fileValidator.IsValid(file, out string? errorValid))
            {
                string errorInfo =$"Не удалось загрузить файл.\r\nФайл: {file?.FileName}\r\nОшибка: {errorValid}";
                _logger.LogInformation(errorInfo);
                return BadRequest(errorInfo);
            }

            // Загрузка иициализируещего файла
            string? result = await _dataInitializationService.UploadDataFile(file!);

            // Обработка результата загрузки файла
            if(result is null)
            {
                string info = $"Файл успешно загружен.\r\nФайл: {file?.FileName}";
                _logger.LogInformation(info);
                return Ok(info);
            }
            else
            {
                string error = $"Не удалось загрузить файл.\r\nФайл: {file?.FileName}";
                _logger.LogInformation(error+ "\r\nОшибка: ошибка валидации данных.");
                return BadRequest(error + $"\r\nОшибка: {result}");
            }
        }

        /// <summary>
        /// Метод обработки запроса поиска рекламных площадок
        /// </summary>
        /// <param name="location">Шаблон поска</param>
        /// <returns>Список рекламных площадок, работающих в данной локации</returns>
        [HttpGet]
        public async Task<ActionResult<List<AdvertisingPlatform>>> Search([FromQuery]string? location)
        {
            // Проверка на наличие шаблона поиска
            if(String.IsNullOrEmpty(location))
            {
                return BadRequest("Локация для поиска не может быть пустой.");
            }

            // Поиск площадок по шаблону
            (List<AdvertisingPlatform>? result,string? error) = await _platformsService.Search(location);

            // Обработка результата поиска
            if(error is null)
            {
                
                string resultInfo = $"Запрос: {location!}\r\nРезультат: {result!.Count} - элементов.";

                _logger.LogInformation(resultInfo);

                return Ok(result);
            }
            else
            {
                string resultInfo = $"Запрос: {location!}\r\nОшибка: локация задана не верно.";

                _logger.LogInformation(resultInfo);

                return BadRequest(error);
            }
        }

        /// <summary>
        /// Возвращаем параметры приложения для валидации на фронте
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("application_parameters")]
        public ActionResult<IAppParameters> GetApplicationParameters()
        {
            _logger.LogInformation("Запрос параметров приложения.");

            return Ok(_parameters);
        }
    }
}
