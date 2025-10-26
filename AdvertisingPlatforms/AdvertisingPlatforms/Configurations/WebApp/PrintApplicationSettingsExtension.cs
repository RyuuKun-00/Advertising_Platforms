using AdvertisingPlatforms.ApplicationSettings;
using AdvertisingPlatforms.Core.Abstractions;

namespace AdvertisingPlatforms.Configurations.WebApp
{
    public static class PrintApplicationSettingsExtension
    {
        /// <summary>
        /// Расширение для <see cref="WebApplication"/><br/>
        /// Выводит в логгер (lvl: info) пользовательские настройки приложения
        /// </summary>
        public static WebApplication PrintApplicationSettingsInLog(this WebApplication app)
        {
            // Получение экземпляра пользовательских настроек приложения
            IAdvertisingPlatformValidationParameters advertisingPlatformValidation = (IAdvertisingPlatformValidationParameters?)app.Services.GetService(typeof(IAdvertisingPlatformValidationParameters))
                                       ?? AppParametersSingleton.GetInstance;
            IFileValidationParameters fileValidationParameters = (IFileValidationParameters?)app.Services.GetService(typeof(IFileValidationParameters))
                                       ?? AppParametersSingleton.GetInstance;
            string info =
                $"""
                Пользовательские настройки приложения:
                 - AllowedExtensions: [{String.Join(", ", fileValidationParameters.AllowedExtensions)}]
                 - AllowedMimeTypes: [{String.Join(", ", fileValidationParameters.AllowedMimeTypes)}]
                 - MaxSizeFile: {fileValidationParameters.MaxSizeFile} bytes
                 - AllowingTheUseOfCapitalLetters: {advertisingPlatformValidation.AllowingTheUseOfCapitalLetters}
                 - CapitaLetterSensitivity: {advertisingPlatformValidation.CapitaLetterSensitivity}
                 - LocationsWithTheSameName: {advertisingPlatformValidation.LocationsWithTheSameName}
                 - RepeatingSubLocations: {advertisingPlatformValidation.RepeatingSubLocations}
                """;

            app.Logger.LogInformation(info);

            return app;
        }
    }
}
