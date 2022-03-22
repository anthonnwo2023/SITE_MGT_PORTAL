using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;

namespace Project.V1.Web.Middlewares
{
    public static class BlazoriseWithLocalStorage
    {
        public static void ConfigureBlazorizeOptions(this IServiceCollection services)
        {
            services.AddBlazorise()
                  .AddBootstrapProviders()
                  .AddFontAwesomeIcons();
        }
    }
}
