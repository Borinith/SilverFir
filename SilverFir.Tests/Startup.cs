using Microsoft.Extensions.DependencyInjection;
using SilverFir.SearchBonds;

namespace SilverFir.Tests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddTransient<ISearchBonds, SearchBonds.SearchBonds>();
        }
    }
}