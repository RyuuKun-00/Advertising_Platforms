"use strict";

//=========================================//
// Методы для обработки загружаемого файла //
//=========================================//

// Устанавливает результат загрузки файла
var setStatus;

// Скидывает состояние формы загрузки файла
var formDataReset;

//=========================================//
// Методы для обработки результатов поиска //
//=========================================//

// Метод установки ошибки поиска
var setTextErrorSearch;

// Метод проявления результата поиска
var showResultBlock;

// Метод установки результата поиска
var setResultSearch;

//=========================================//

(function ($) {

  if(!$) {
    return;
  }

  ////////////////
  // COLOR MODE //
  ////////////////
  $('.color-mode').click(function(){
      $('body').toggleClass('dark-mode')
  })
  ////////////
  // HEADER //
  ////////////
  $(".headroom").headroom();

  /////////////////
  // OPEN DIALOG //
  /////////////////

  // При нажатии на иконку загрузки файла
  // Открываем диалоговое окно и блокируем скроллинг
  $(".upload-file").on("click",function () {
    $("#myDialog").get(0).showModal();
    document.body.classList.add('scroll-lock');
  });

  // Если нажимаем вне области диалогового окна, то окно закрываем
  $("#myDialog").on("click",function ({ currentTarget, target }) {
      const dialog = currentTarget;
      const isClickedOnBackDrop = target === dialog;
      if (isClickedOnBackDrop) {
        close();
      }
  });

  // Если закрывается диалогое окно, то возвращаем возможность скролинга
  $("#myDialog"). on("cancel",function () {
    returnScroll();
  });

  // При нажатии на кнопку закрытия, закрываем диалоговое окно
  $(".closeDialogBtn").on("click",function (event) {
      event.stopPropagation();
      close();
  });
  
  // Функция для восстановления скролинга на странице
  function returnScroll() {
    document.body.classList.remove('scroll-lock')
  }
  
  // Функция закрытия фиалогового окна
  function close() {
    $("#myDialog").get(0).close();
    $("#uploadForm_Hint").html("");
    returnScroll();
  }

  ////////////////
  // UPLOADFILE //
  ////////////////

  // Инициализация глобальной переменной
  // Установка результата обработки файла
  setStatus = (text,isError=false) => 
  {
    var hint = $("#uploadForm_Hint");
    hint.html(text);
    if(isError){
      hint.addClass("error");
      hint.removeClass("success");
    }else{
      hint.addClass("success");
      hint.removeClass("error");
    }
  }

  // Инициализация глобальной переменной
  // Сброс данных формы
  formDataReset = ()=>{
    $("#uploadForm").get(0).reset();
  };

  // Блокируем обработку событий у документа на драг-дроп
  ["dragover", "drop"].forEach(function(event) {
    document.addEventListener(event, function(evt) {
      evt.preventDefault()
      return false
    })
  });

  // Действия при переменщении объекта над полем
  $(".upload-zone-dragover").on("dragenter", function(){
    $(this).addClass('_active');
  });

  // Действия при покидания объектом границ полня
  $(".upload-zone-dragover").on("dragleave", function(){
    $(this).removeClass('_active');
  });

  // Действия при "сбрасывании" объекта над полем
  $(".upload-zone-dragover").on("drop", function(){
    $(this).removeClass('_active');
      const file = event.dataTransfer?.files[0];
      var a = $(".form-upload-input").get(0);
      processingUploadFile(file)
  });

  // Действия при звгрузке файла
  $(".form-upload-input").on("change", function(){
    const file = $(this).get(0).files?.[0];
    processingUploadFile(file);
  });

  ////////////
  // SEARCH //
  ////////////

  // Обработчик нажатия кнопки поиска
  $("#btnSearch").on("click",async function(){
    const searchString = $("#boxSearch").get(0).value.trim();
    await InitHTMLElements(searchString);
  });

  // Обработчик нажатия на энтер в поле ввода поиска строки
  $("#boxSearch").keyup( async function (e) {
    if (e.keyCode === 13) {
      const searchString = $("#boxSearch").get(0).value.trim();
      await InitHTMLElements(searchString);
    }
  });

  // Инициализация глобальной переменной
  // Добавление текста ошибки
  setTextErrorSearch = (html) =>{
    $(".textErrorSearch").html(html);
  };

  // Инициализация глобальной переменной
  // Появление результата поиска
  showResultBlock = () =>{
    $("#contetnt").removeClass("d-none");
  };

  // Инициализация глобальной переменной
  // Добавление результата поиска
  setResultSearch = (result)=>{
    $("#AdvertisingPlatforms").html(result);
  };

})(jQuery);
