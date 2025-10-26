using AdvertisingPlatforms.Application.Services;
using AdvertisingPlatforms.Application.Validators;
using AdvertisingPlatforms.ApplicationSettings;
using AdvertisingPlatforms.Core.Abstractions;
using AdvertisingPlatforms.DataAccess.Repositories;
using AdvertisingPlatforms.DataAccess.Storage;

namespace AdvertisingPlatforms.Configurations.Builder
{
    public static class AddServicesExtention
    {
        /// <summary>
        /// Расширение для <see cref="WebApplicationBuilder"/><br/>
        /// Добавление сервисов в веб-приложение
        /// </summary>
        public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
        {
            var s = builder.Services;

            s.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                // Отключение втроенных фильтров валидации
                options.SuppressModelStateInvalidFilter = true;
            });

            // Сервисы для Swagger
            s.AddEndpointsApiExplorer();
            s.AddSwaggerGen(c =>
            {
                c.CustomSchemaIds(r => r.FullName);
            });


            // Для доступа к API из вне, только для разработки
            //if (builder.Environment.IsDevelopment())
            //{
            //    builder.Services.AddCors(options =>
            //    {
            //        options.AddPolicy("AllowAll",
            //            builder =>
            //            {
            //                builder.AllowAnyOrigin() // Разрешает запросы из любого источника
            //                       .AllowAnyMethod() // Разрешает любые HTTP-методы (GET, POST и т.д.)
            //                       .AllowAnyHeader(); // Разрешает любые заголовки
            //            });
            //    });
            //}


            // Добавление сервисов приложения
            s.AddTransient<IAdvertisingPlatformsRepository, AdvertisingPlatformsRepository>();
            s.AddTransient<IAdvertisingPlatformsService,AdvertisingPlatformsService>();
            s.AddTransient<IAdvertisingPlatformValidation,AdvertisingPlatformValidation>();
            s.AddSingleton<IAdvertisingPlatformValidationParameters, AppParametersSingleton>(_ => AppParametersSingleton.GetInstance);
            s.AddSingleton<IFileValidationParameters, AppParametersSingleton>(_ => AppParametersSingleton.GetInstance);
            s.AddSingleton<IResponseTemplates, AppParametersSingleton>(_ => AppParametersSingleton.GetInstance);
            s.AddSingleton<IAppParameters, AppParametersSingleton>(_ => AppParametersSingleton.GetInstance);
            s.AddTransient<IDataInitializationService,DataInitializationService>();
            s.AddTransient<IFileValidator,FileValidator>();
            s.AddSingleton<IStorage, StorageSingleton>(_ => StorageSingleton.GetInstance);
            s.AddTransient<IStorageBuilder,StorageBuilder>();



            return builder;
        }
    }
}
