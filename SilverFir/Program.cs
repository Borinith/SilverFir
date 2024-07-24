using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SilverFir.LanguageService;
using SilverFir.SearchBonds;
using System;

namespace SilverFir
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            // Создаем билдер
            var builder = Host.CreateApplicationBuilder();

            // Внедряем сервисы
            builder.Services.AddSingleton<App>();
            builder.Services.AddSingleton<MainWindow>();

            builder.Services.AddSingleton<EngLanguageService>();
            builder.Services.AddSingleton<RusLanguageService>();
            builder.Services.AddSingleton<HebLanguageService>();

            builder.Services.AddSingleton<ProxyLanguage.ProxyLanguageResolver>(serviceProvider => language =>
            {
                return language switch
                {
                    LanguageEnum.English => serviceProvider.GetService<EngLanguageService>(),
                    LanguageEnum.Russian => serviceProvider.GetService<RusLanguageService>(),
                    LanguageEnum.Hebrew => serviceProvider.GetService<HebLanguageService>(),
                    _ => null
                };
            });

            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<ISearchBonds, SearchBonds.SearchBonds>();

            // Создаем хост приложения
            using var host = builder.Build();

            // Получаем сервис - объект класса App
            var app = host.Services.GetService<App>();

            // Запускаем приложение
            app?.Run();
        }
    }
}