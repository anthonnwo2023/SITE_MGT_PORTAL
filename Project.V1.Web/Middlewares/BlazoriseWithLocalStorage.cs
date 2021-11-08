using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.Extensions.DependencyInjection;

namespace Project.V1.Web.Middlewares
{
    public static class BlazoriseWithLocalStorage
    {
        public static void ConfigureBlazorizeOptions(this IServiceCollection services)
        {
            services.AddBlazorise(options =>
            {
                options.ChangeTextOnKeyPress = true; // optional
            })
                  .AddBootstrapProviders()
                  .AddFontAwesomeIcons();
        }
    }
}
