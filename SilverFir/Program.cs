using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
                           services.AddTransient<ISearchBonds, SearchBonds.SearchBonds>();
                           //services.AddTransient<ILanguageService, EngLanguageService>();
                           //services.AddTransient<ILanguageService, RusLanguageService>();
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