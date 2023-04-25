﻿using Microsoft.Extensions.DependencyInjection;
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
            // Создаем хост приложения
            using (var host = Host.CreateDefaultBuilder()
                       // Внедряем сервисы
                       .ConfigureServices(services =>
                       {
                           services.AddSingleton<App>();
                           services.AddSingleton<MainWindow>();
                           services.AddSingleton<ISearchBonds, SearchBonds.SearchBonds>();

                           services.AddSingleton<EngLanguageService>();
                           services.AddSingleton<RusLanguageService>();
                           services.AddSingleton<HebLanguageService>();

                           services.AddSingleton<ProxyLanguage.ProxyLanguageResolver>(serviceProvider => language =>
                           {
                               return language switch
                               {
                                   LanguageEnum.English => serviceProvider.GetService<EngLanguageService>(),
                                   LanguageEnum.Russian => serviceProvider.GetService<RusLanguageService>(),
                                   LanguageEnum.Hebrew => serviceProvider.GetService<HebLanguageService>(),
                                   _ => null
                               };
                           });
                       })
                       .Build())
            {
                // Получаем сервис - объект класса App
                var app = host.Services.GetService<App>();

                // Запускаем приложения
                app?.Run();
            }
        }
    }
}