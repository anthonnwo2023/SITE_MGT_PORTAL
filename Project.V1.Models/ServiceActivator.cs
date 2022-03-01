using Microsoft.Extensions.DependencyInjection;

namespace Project.V1.Models
{
    public class ServiceActivator
    {
        internal static IServiceProvider _serviceProvider = null;
        public static void Configure(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static IServiceScope GetScope(IServiceProvider serviceProvider = null)
        {
            IServiceProvider provider = serviceProvider ?? _serviceProvider;
            return provider?
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();
        }
    }
}
