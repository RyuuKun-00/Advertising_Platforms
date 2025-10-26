  
  // Метод валидации файла
  function isValidFile(file){
    if(!file){
      return "Файл не обнаружен."
    }

    const fileNameExten = file.name.split('.').reverse()[0];
    const allowedExtensions = appParams.allowedExtensions;
    // Валидация на расширение файла
    if (allowedExtensions.length > 0 && !allowedExtensions.includes("."+fileNameExten)) {
      return `Недопустимый тип файла.<br>Разрешены только:<br>${appParams.allowedExtensions.join(",")}.`
    }

    // Валидация на контент фала
    const allowedTypes = appParams.allowedMimeTypes;
    if (allowedTypes.length > 0 && !allowedTypes.includes(file.type)) {
      return `Недопустимый контент файла.<br>Разрешены только:<br>${appParams.allowedMimeTypes.join(",")}.`
    }

    // Валидация на размер загружаемого файла
    const maxSizeInBytes = appParams.maxSizeInBytes;
    if (file.size > maxSizeInBytes) {
        return `Недопустимый размер файла.<br>Максимальный разрешенный размер:<br>\n${maxSizeInBytes} байт.`
    }

    return null;
  }