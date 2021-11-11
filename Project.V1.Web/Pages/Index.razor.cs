using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Project.V1.Lib.Interfaces;
using Project.V1.Lib.Extensions;
using System;
using System.Threading.Tasks;

namespace Project.V1.Web.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject]
        protected IHttpContextAccessor HttpContextAccessor { get; set; }
        [Inject] protected NavigationManager NavMan { get; set; }
        [Inject] protected ICLogger Logger { get; set; }
        [Inject] protected IHttpContextAccessor HttpContext { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }

        protected override async Task OnInitializedAsync()
        {
            string returnUrl = Uri.EscapeDataString(NavMan.ToBaseRelativePath(NavMan.Uri));
            returnUrl = (returnUrl == "access-denied" || returnUrl.Contains("logout")) ? null : returnUrl;

            if (await UserAuth.IsAuthenticatedAsync())
            {
                NavMan.NavigateTo($"dashboard");
                return;
            }

            NavMan.NavigateTo($"Identity/Account/Login?returnUrl={returnUrl}", forceLoad: true);
            return;
        }
    }
}
