using AdvertisingPlatforms.ApplicationSettings;
using AdvertisingPlatforms.Core.Abstractions;

namespace AdvertisingPlatforms.Tests.TestResources.Models
{
    /// <summary>
    /// Тестовый класс пользовательских параметров приложения
    /// </summary>
    public class AppParameters_Test : IAppParameters
    {
        public string[] AllowedExtensions { get; set; } = [".txt"];

        public string[] AllowedMimeTypes { get; set; } = ["text/plain"];

        public int MaxSizeFile { get; set; } = 50 * 1024 * 1024;// 50 Мб - по умолчанию

        private bool _allowingTheUseOfCapitalLetters = false;
        public bool AllowingTheUseOfCapitalLetters
        {
            get
            {
                return _allowingTheUseOfCapitalLetters;
            }
            set
            {
                _allowingTheUseOfCapitalLetters = value;
                // Инициализация патерна для валидации разрешённых символов
                StringValidationPattern = value
                       ? @"^[a-zA-Z/]+$" // Тут верхний регист разрешён
                       : @"^[a-z/]+$";   // Тут верхний регист вызывает ошибки
            }
        }

        private bool _capitaLetterSensitivity = false;
        public bool CapitaLetterSensitivity
        {
            get
            {
                return _capitaLetterSensitivity;
            }
            set
            {
                _capitaLetterSensitivity = value;

                // Получение разрешения на использование верхнего регистра
                if (value)
                {
                    // Если локации чувствительны к верхнему регистру, то верхний регистр автоматически разрешён
                    AllowingTheUseOfCapitalLetters = true;
                }
            }
        }

        private bool _locationsWithTheSameName = false;
        public bool LocationsWithTheSameName
        {
            get
            {
                return _locationsWithTheSameName;
            }
            set
            {
                _locationsWithTheSameName = value;
                // Получение разрешения на использование одинаковых подлокаций в полной локации
                if (!value)
                {
                    // Если запрещено использовать локации с одинаковыми названиями, то повторений быть не может
                    RepeatingSubLocations = false;
                }
            }
        }

        public bool RepeatingSubLocations { get; set; } = false;

        public string StringValidationPattern { get; set; } = string.Empty;

        public string DataTemplateDescription { get; set; } = "Описание шаблона данных в файле";

        public string LocationTemplateDescription { get; set; } = "Описание шаблона локаций в файле";


        #region Реализация паттерна одиночка (Singleton)

        // Свойтво гарантирует патокобезопасность и отложенное создание экземпляра класса
        public static AppParameters_Test GetInstance { get; } = new();
        #endregion

        public AppParameters_Test()
        {
        }

        public override bool Equals(object? obj)
        {
            if(obj is AppParameters_Test app)
            {
                return AllowedExtensions.SequenceEqual(app.AllowedExtensions)
                    && AllowedMimeTypes.SequenceEqual(app.AllowedMimeTypes)
                    && MaxSizeFile == app.MaxSizeFile
                    && AllowingTheUseOfCapitalLetters == app.AllowingTheUseOfCapitalLetters
                    && CapitaLetterSensitivity == app.CapitaLetterSensitivity
                    && LocationsWithTheSameName == app.LocationsWithTheSameName
                    && RepeatingSubLocations == app.RepeatingSubLocations
                    && StringValidationPattern == app.StringValidationPattern
                    && LocationTemplateDescription == app.LocationTemplateDescription
                    && DataTemplateDescription == app.DataTemplateDescription
                    ? true
                    : false;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// метод установки параметров по умолчанию
        /// </summary>
        public void SetDefaultParameters()
        {
            AllowedExtensions = [".txt"];
            AllowedMimeTypes = ["text/plain"];
            MaxSizeFile = 50 * 1024 * 1024;// 50 Мб - по умолчанию
            AllowingTheUseOfCapitalLetters = false;
            CapitaLetterSensitivity = false;
            LocationsWithTheSameName = false;
            RepeatingSubLocations = false;
        }
    }
}
