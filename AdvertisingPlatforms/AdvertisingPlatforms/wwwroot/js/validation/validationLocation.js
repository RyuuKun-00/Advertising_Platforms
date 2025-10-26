
// метод валидации локации для поиска
function isValidLocation(location){

    // Функция проверки строки на разрешённые символы
    const IsValidPatternCheck = (loc)=>{
        return appParams.stringValidationPattern.test(loc);
    }

    let locTrim = location.trim();
    // Проверка на пустую строку
    if (locTrim.Length == 0)
    {
        return false;
    }

    // Проверка на разрешённые символы по патерну
    let isValidPatternCheck = IsValidPatternCheck(locTrim);
    if (!isValidPatternCheck)
    {
        return false;
    }

    //Если нет чуствительности в символам верхнего регистра, то преобразовываем в нижний регистр
    if (!appParams.capitaLetterSensitivity) {
        locTrim = locTrim.toLowerCase();
    }
    
    // Поиск повторений, если они запрещены
    if(!appParams.repeatingSubLocations){
        let subLoc = locTrim.split("/").filter(Boolean);
        let duplicates = subLoc.filter((item, index) => subLoc.indexOf(item) !== index);
        if(duplicates.length > 0){
            return false;
        }
    }

    return true;
}
