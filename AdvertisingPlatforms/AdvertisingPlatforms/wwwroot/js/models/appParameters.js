// Прототип паарметров приложения со значениями по умолчанию
class AppParameters{

    // Список допустимых расширений файла<br/>
    // Если список пуст, то допустимы все расширения
    allowedExtensions = [".txt"];

    // Список допустимых MIME типов файла<br/>
    // Если список пуст, то допустимы все MIME типы
    allowedMimeTypes = ["text/plain"];

    // Максимальный размер файла
    maxSizeFile = 50*1024*124;// 50 Мб

    // Если параметр равен true и CapitaLetterSensitivity = false,
    // то следующие локации одинаковые: /ru /Ru /rU /RU
    // Если параметр равен false и CapitaLetterSensitivity = false
    // то файл с локациями в верхнем регистром не валиден: /Ru /rU /RU -> ошибки
    allowingTheUseOfCapitalLetters = false;

    // Параметр чувствительности к символам верхнего регистра
    // Если параметр равен true, то следующие локации разные: /ru /Ru /rU /RU
    capitaLetterSensitivity = false;

    // Разрешение на использование подлокаций с одинаковыми названиями
    // Если параметр true,
    // то могут существовать одновременно локации типа: /ru/fp, /kz/.../fp -> валидны
    // Иначе говоря, нельзя однозначно определить локацию по последней подлокации
    // /fp -> /ru/fp и /kz/.../fp
    locationsWithTheSameName = false;

    // Разрешение на использование одинаковых подлокаций в полной локации
    // Если параметр равен true и LocationsWithTheSameName = true
    // то следующие локации типа: /ru/ru, /ru/.../ru -> валидны
    // Если параметр равен false и LocationsWithTheSameName = true
    // то следующие локации типа: /ru/ru, /ru/.../ru -> ошибки
    repeatingSubLocations = false;

    // Патерн для валидации разрешённых символов
    // Если AllowingTheUseOfCapitalLetters = true,
    // то значение: /^[a-zA-Z/]+$/ - верхний регистр разрешён
    // иначе значение: /^[a-z/]+$/ - верхний регистр генирирует отслеживаемые ошибки
    stringValidationPattern = /^[a-z/]+$/;

    // Шаблон ответа для описания ошибки данных в файле
    dataTemplateDescription;

    // Шаблон ответа для описания ошибки локации
    locationTemplateDescription;
    
    static GetStringValidationPattern(allowingTheUseOfCapitalLetters){
        return allowingTheUseOfCapitalLetters ? /^[a-zA-Z/]+$/
                                              : /^[a-z/]+$/;
    }
    
    // Метод инициализации шаблонов ответов для ошибок валидации на клиенте
    SetTemplate() {
        var RepeatingSubLocationsDescription = this.locationsWithTheSameName && this.repeatingSubLocations
                    ? "также разрешено использовать повторения в полной локации: /ru/ru или /ru.../gz/ru - допустимо."
                    : "но повторения в полной локации запрещены, например: /ru/ru или /ru.../gz/ru - запрещены.";

        var LocationsWithTheSameNameDescription = this.locationsWithTheSameName
                    ? `- Конечные подлокации могут повторяться, например: /ru и /pz/ru - допустимо,
                       <br>&nbsp;&nbsp;&nbsp;${RepeatingSubLocationsDescription}`
                    : "- Повторяющиеся локации запрещены, например: /ru и /pz/ru , /ru/ru или /ru.../gz/ru - ошибки.";

        this.locationTemplateDescription =
        `Требования к локациям:
        <br>&nbsp;&nbsp;&nbsp;- Локации должны начинаться и отделяться символом '/';
        <br>&nbsp;&nbsp;&nbsp;- Локации задаются символами нижнего ${(this.allowingTheUseOfCapitalLetters ? "и вержнего " : "")}регистра латинского алфавита;
        <br>&nbsp;&nbsp;&nbsp;${LocationsWithTheSameNameDescription}`;

        this.dataTemplateDescription =
        `Рекламные площадки должны быть представлены в виде шаблона:
        <br>&nbsp;&nbsp;&nbsp;<Название площдадки> : <Локации работы площадки>
        <br>Локации работы площадки перечисляются через запятую и задаются следующим шаблоном:
        <br>&nbsp;&nbsp;&nbsp;/<Локация>/<Локация>.../<Локация>
        <br>${this.locationTemplateDescription}`;

        this.stringValidationPattern = this.allowingTheUseOfCapitalLetters ? /^[a-zA-Z/]+$/
                                                                      : /^[a-z/]+$/;
    }
}