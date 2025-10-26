
using AdvertisingPlatforms.Core.Abstractions;

namespace AdvertisingPlatforms.ApplicationSettings
{
    /// <summary>
    /// Реализация интерфейса <see cref="IAppParameters"/> <br/>
    /// Пользовательские настройки приложения
    /// </summary>
    public class AppParametersSingleton : IAppParameters
    {
        public string[] AllowedExtensions { get;  set; }

        public string[] AllowedMimeTypes { get; set; }

        public int MaxSizeFile { get;  set; }

        public bool AllowingTheUseOfCapitalLetters { get;  set; }

        public bool CapitaLetterSensitivity { get;  set; }

        public bool LocationsWithTheSameName { get;  set; }

        public bool RepeatingSubLocations { get;  set; }

        public string StringValidationPattern { get;  set; }

        public string DataTemplateDescription { get;  set; }

        public string LocationTemplateDescription { get; set; }

        #region Реализация паттерна одиночка (Singleton)

        // Свойтво гарантирует патокобезопасность и отложенное создание экземпляра класса
        public static AppParametersSingleton GetInstance { get; } = new();

        #endregion

        /// <summary>
        /// Инициализация пользовательских настроек приложения
        /// </summary>
        public AppParametersSingleton()
        {
            // Получение пользовательских настроек
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();


            #region Получаем настройки валидации файла

            var fileValidationParameters = configuration.GetSection("ApplicationSettings:FileValidationParameters");


            // Получаем список допустимых расширений файла
            AllowedExtensions = fileValidationParameters.GetSection("AllowedExtensions").Get<string[]>()
                                ?? [".txt"];// По умолчанию


            // Инициализируем список разрешённых MIME типов файлов (разрешённый контекст файла)
            AllowedMimeTypes = fileValidationParameters.GetSection("AllowedMimeTypes").Get<string[]>()
                               ?? ["text/plain"];// По умолчанию


            // Инициализируем ограничение по весу файла
            MaxSizeFile = fileValidationParameters.GetValue<int>("MaxSize");
            if (MaxSizeFile == 0)
            {
                MaxSizeFile = 50 * 1024 * 1024;// 50 Мб - по умолчанию
            }

            #endregion


            #region Получаем настройки валидации рекламных платформ

            var validationParameters = configuration.GetSection("ApplicationSettings:AdvertisingPlatformValidationParameters");

            // Получение параметра на чувствительнось к символам верхнего регистра.
            CapitaLetterSensitivity = validationParameters.GetValue<bool>("CapitaLetterSensitivity");


            // Получение разрешения на использование верхнего регистра
            if (CapitaLetterSensitivity)
            {
                // Если локации чувствительны к верхнему регистру, то верхний регистр автоматически разрешён
                AllowingTheUseOfCapitalLetters = true;
            }
            else
            {
                // Получение параметра на разрешение использования верхнего регистра
                AllowingTheUseOfCapitalLetters = validationParameters.GetValue<bool>("AllowingTheUseOfCapitalLetters");
            }

            // Получение параметра на наличие локаций с одинаковыми названиями
            LocationsWithTheSameName = validationParameters.GetValue<bool>("LocationsWithTheSameName");

            // Получение разрешения на использование одинаковых подлокаций в полной локации
            if (LocationsWithTheSameName)
            {
                // Получение параметра на возможность использовать одинаковые подлокации в полной локации
                RepeatingSubLocations = validationParameters.GetValue<bool>("RepeatingSubLocations");
            }
            else
            {
                // Если запрещено использовать локации с одинаковыми названиями, то повторений быть не может
                RepeatingSubLocations = false;
            }

            // Инициализация патерна для валидации разрешённых символов
            StringValidationPattern = AllowingTheUseOfCapitalLetters
                       ? @"^[a-zA-Z/]+$" // Тут верхний регист разрешён
                       : @"^[a-z/]+$";   // Тут верхний регист вызывает ошибки

            #endregion


            #region Генерируем описание шаблона данных

            string RepeatingSubLocationsDescription = LocationsWithTheSameName && RepeatingSubLocations
                    ? "также разрешено использовать повторения в полной локации:  /ru/ru или /ru.../gz/ru - допустимо."
                    : "но повторения в полной локации запрещены, например: /ru/ru или /ru.../gz/ru - запрещены.";

            string LocationsWithTheSameNameDescription = LocationsWithTheSameName
                    ? $"- Конечные подлокации могут повторяться, например: /ru и /pz/ru - допустимо,"+
                      $"\n\t{RepeatingSubLocationsDescription}"
                    : "- Повторяющиеся локации запрещены, например: /ru и /pz/ru , /ru/ru или /ru.../gz/ru - ошибки.";


            LocationTemplateDescription =
                "Требования к локациям:"+
                "\n\t- Локации должны начинаться и отделяться символом '/';"+
                $"\n\t- Локации задаются символами нижнего {(AllowingTheUseOfCapitalLetters ? "и вержнего " : "")}регистра латинского алфавита;"+
                $"\n\t{LocationsWithTheSameNameDescription}";

            DataTemplateDescription =
                "Рекламные площадки должны быть представлены в виде шаблона:"+
                "\n\t<Название площдадки> : <Локации работы площадки>"+
                "\nЛокации работы площадки перечисляются через запятую и задаются следующим шаблоном:"+
                "\n\t/<Локация>/<Локация>.../<Локация>"+
                $"\n{LocationTemplateDescription}";


            #endregion
        }
    }
}
