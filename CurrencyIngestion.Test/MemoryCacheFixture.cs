using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace CurrencyIngestion.Test
{
    public class MemoryCacheFixture
    {
        public IMemoryCache CreateMemoryCache()
        {
            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider.GetRequiredService<IMemoryCache>();
        }
    }
}