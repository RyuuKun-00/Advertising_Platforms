using AdvertisingPlatforms.Application.Validators;
using AdvertisingPlatforms.Core.Abstractions;
using AdvertisingPlatforms.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdvertisingPlatforms.Controllers
{
    /// <summary>
    /// ���������� ��������� �������� � ��������� ���������
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
        /// ����� �������� ����� ��� ������������� ������ ��������� ��������
        /// </summary>
        /// <param name="file">���� ��� ������������� ��������� ��������</param>
        /// <returns>��������� �������� �����</returns>
        [HttpPost]
        public async Task<ActionResult<string?>> LoadingInitializationFile(IFormFile? file)
        {
            // �������� �� ����������� ������������ �����
            if(!_fileValidator.IsValid(file, out string? errorValid))
            {
                string errorInfo =$"�� ������� ��������� ����.\r\n����: {file?.FileName}\r\n������: {errorValid}";
                _logger.LogInformation(errorInfo);
                return BadRequest(errorInfo);
            }

            // �������� ���������������� �����
            string? result = await _dataInitializationService.UploadDataFile(file!);

            // ��������� ���������� �������� �����
            if(result is null)
            {
                string info = $"���� ������� ��������.\r\n����: {file?.FileName}";
                _logger.LogInformation(info);
                return Ok(info);
            }
            else
            {
                string error = $"�� ������� ��������� ����.\r\n����: {file?.FileName}";
                _logger.LogInformation(error+ "\r\n������: ������ ��������� ������.");
                return BadRequest(error + $"\r\n������: {result}");
            }
        }

        /// <summary>
        /// ����� ��������� ������� ������ ��������� ��������
        /// </summary>
        /// <param name="location">������ �����</param>
        /// <returns>������ ��������� ��������, ���������� � ������ �������</returns>
        [HttpGet]
        public async Task<ActionResult<List<AdvertisingPlatform>>> Search([FromQuery]string? location)
        {
            // �������� �� ������� ������� ������
            if(String.IsNullOrEmpty(location))
            {
                return BadRequest("������� ��� ������ �� ����� ���� ������.");
            }

            // ����� �������� �� �������
            (List<AdvertisingPlatform>? result,string? error) = await _platformsService.Search(location);

            // ��������� ���������� ������
            if(error is null)
            {
                
                string resultInfo = $"������: {location!}\r\n���������: {result!.Count} - ���������.";

                _logger.LogInformation(resultInfo);

                return Ok(result);
            }
            else
            {
                string resultInfo = $"������: {location!}\r\n������: ������� ������ �� �����.";

                _logger.LogInformation(resultInfo);

                return BadRequest(error);
            }
        }

        /// <summary>
        /// ���������� ��������� ���������� ��� ��������� �� ������
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("application_parameters")]
        public ActionResult<IAppParameters> GetApplicationParameters()
        {
            _logger.LogInformation("������ ���������� ����������.");

            return Ok(_parameters);
        }
    }
}
