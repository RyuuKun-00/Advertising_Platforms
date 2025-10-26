
// Метод получения рекламных площадок по локации
async function InitHTMLElements(searchString){
    // Валидируем строку для поиска
    var IsValid = isValidLocation(searchString);

    // Если строка не прошла валидацию, то выводим ошибку
    if(!IsValid){
        setTextErrorSearch(appParams.locationTemplateDescription);
        return;
    }

    // Получаем список рекламных площадок согласно запросу
    var platforms = await searchAdvertisingPlatforms(searchString);

    // Обрабатываем ошибку, если она есть
    if(platforms.error){
        var errors = platforms.error.replaceAll("\n","<br>");
        errors = errors.replaceAll("\t","&nbsp;&nbsp;&nbsp;");
        setTextErrorSearch(errors);
        return;
    }
    
    // Удаление высвеченной ошибки, если она была
    setTextErrorSearch("");
    // Показываем результирующий блок
    showResultBlock();

    // Генерируем результат запроса из полученных данных
    var htmlPlatforms="";
    platforms.forEach(element => {
        var loc = element.nameLocation;
        var platforms = element.advertisingPlatforms;

        var advertisingPlatforms="";
        platforms.forEach(element=>{
            advertisingPlatforms+=`<span>${element}</span>`;
        });
        htmlPlatforms+=`
        <details class="location">
            <summary class="nameLocation">${loc}</summary>
            <div class="group">
                ${advertisingPlatforms}
            </div>
        </details>
        `;
        
    });

    // если элементов нет, то говорим об этом
    if(htmlPlatforms===""){
        htmlPlatforms="Список пуст.";
    }

    // Выводи список рекламных площадок
    setResultSearch(htmlPlatforms);
}