

namespace AdvertisingPlatforms.Core.Abstractions
{
    /// <summary>
    /// Шаблон параметров валидации рекламных площадок
    /// </summary>
    public interface IAdvertisingPlatformValidationParameters
    {

        /// <summary>
        /// Разрешение на использование символов верхнего регистра
        /// <para>
        /// Если параметр равен <b>true</b> и <see cref="CapitaLetterSensitivity">CapitaLetterSensitivity</see> = <b>false</b>,<br/>
        /// то следующие локации одинаковые: <b>/ru /Ru /rU /RU</b><br/><br/>
        /// Если параметр равен <b>false</b> и <see cref="CapitaLetterSensitivity">CapitaLetterSensitivity</see> = <b>false</b><br/>
        /// то файл с локациями в верхнем регистром не валиден: <b>/Ru /rU /RU -> ошибки</b>
        /// </para>
        /// По умолчанию: <b>false</b>
        /// </summary>
        bool AllowingTheUseOfCapitalLetters { get; }
        /// <summary>
        /// Параметр чувствительности к символам верхнего регистра
        /// <para>
        /// Если параметр равен <b>true</b>,<br/> то следующие локации разные: <b>/ru /Ru /rU /RU</b>
        /// </para>
        /// По умолчанию: <b>false</b>
        /// </summary>
        bool CapitaLetterSensitivity { get; }
        /// <summary>
        /// Разрешение на использование подлокаций с одинаковыми названиями
        /// <para>
        /// Если параметр <b>true</b>,<br/>
        /// то могут существовать одновременно локации типа: <b>/ru/fp, /kz/.../fp -> валидны</b><br/>
        /// Иначе говоря, нельзя однозначно определить локацию по последней подлокации<br/>
        /// /fp -> /ru/fp и /kz/.../fp
        /// </para>
        /// По умолчанию: <b>false</b>
        /// </summary>
        bool LocationsWithTheSameName { get; }
        /// <summary>
        /// Разрешение на использование одинаковых подлокаций в полной локации
        /// <para>
        /// Если параметр равен <b>true</b> и <see cref="LocationsWithTheSameName">LocationsWithTheSameName</see> = <b>true</b><br/>
        /// то следующие локации типа: <b>/ru/ru, /ru/.../ru -> валидны</b>
        /// </para>
        /// <para>
        /// Если параметр равен <b>false</b> и <see cref="LocationsWithTheSameName">LocationsWithTheSameName</see> = <b>true</b><br/>
        /// то следующие локации типа: <b>/ru/ru, /ru/.../ru -> ошибки</b>
        /// </para>
        /// По умолчанию: <b>false</b>
        /// </summary>
        bool RepeatingSubLocations { get; }

        /// <summary>
        /// Патерн для валидации разрешённых символов
        /// <para>
        /// Если <see cref="AllowingTheUseOfCapitalLetters">AllowingTheUseOfCapitalLetters</see> = <b>true</b>,<br/>
        /// то значение: <b>@"^[a-zA-Z/]+$"</b> - верхний регистр разрешён<br/>
        /// иначе значение:<b> @"^[a-z/]+$"</b> - верхний регистр генирирует отслеживаемые ошибки
        /// </para>
        /// </summary>
        string StringValidationPattern { get; }

    }
}