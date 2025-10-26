"use strict";

// Метод загрузки файла
function processingUploadFile(file) {

    // Клиентская проверка файла
    var error = isValidFile(file);

    if(error !== null){
        setStatus(error,true);
        return false;
    }
            
    const dropZoneData = new FormData();
    const xhr = new XMLHttpRequest();
    const url = hostUrl+"/api/advertising_platforms";

    dropZoneData.append("file", file);

    xhr.open("POST", url, true);

    xhr.send(dropZoneData);

    xhr.onload = function () {
        if (xhr.status == 200) {
            setStatus("Файл успешно загружен.");
        }else{
            setStatus(xhr.responseText.replaceAll("\n","<br>"),true);
        }

        console.log("Загрузка "+file.name + " результат:\n"+ xhr.responseText);
        formDataReset();
    };

    xhr.onerror = function () {
        setStatus("Не удалось загрузить файл. Сервер временно не доступен.",true);
        formDataReset();
    };

}

// Метод поиска рекламных площадок обсуживающих заданную локацию
async function searchAdvertisingPlatforms(searchString) {
    // Запрос
    const response = await fetch(hostUrl+"/api/advertising_platforms?location="+searchString, {
        method: "GET",
        headers: { "Accept": "application/json" }
    }).catch((error)=>{
        console.log("Сервер временно недоступен.\n"+error);
    });

    var result;

    if(response){
        
        if(response.ok === true){
            // Получем ответ
            result = await response.json();
        }else{
            result = {error: await response.json()};
        }
        
    }else{
        // Если червер не доступен
        result = {error:"Сервер временно недоступен."} ;
    }

    return result;
}

// Метод получения параметров валидации
async function getParameters() {
    // Запрос
    const response = await fetch(hostUrl+"/api/advertising_platforms/application_parameters", {
        method: "GET",
        headers: { "Accept": "application/json" }
    }).catch((error)=>{
        console.log("Сервер временно недоступен.\n"+error);
    });

    // если ответ есть и результат 200, то получаем данные
    if (response && response.ok === true) {
        // получаем данные
        let params = await response.json();

        // Инициализируем глобальный экземпляр класса параметров валидации
        appParams.allowedExtensions = params.allowedExtensions;
        appParams.allowedMimeTypes = params.allowedMimeTypes;
        appParams.maxSizeFile = params.maxSizeFile;
        appParams.allowingTheUseOfCapitalLetters = params.allowingTheUseOfCapitalLetters;
        appParams.capitaLetterSensitivity = params.capitaLetterSensitivity;
        appParams.locationsWithTheSameName = params.locationsWithTheSameName;
        appParams.repeatingSubLocations = params.repeatingSubLocations;
        appParams.stringValidationPattern = AppParameters.GetStringValidationPattern(appParams.allowingTheUseOfCapitalLetters);
        appParams.dataTemplateDescription = params.dataTemplateDescription;
        appParams.locationTemplateDescription = params.locationTemplateDescription;
        appParams.SetTemplate();
    }
}

// Вызываем получения парамметров
(async ()=> await getParameters())();