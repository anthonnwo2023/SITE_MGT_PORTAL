using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Project.V1.Lib.Extensions;
using Project.V1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.V1.Web.Pages.Acceptance.Components
{
    public partial class BreadcrumbAccess
    {
        [Parameter] public List<PathInfo> Paths { get; set; }
        [Parameter] public EventCallback<bool> OnAuthenticationCheck { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }
        [Inject] protected IHttpContextAccessor HttpContext { get; set; }
        [Inject] protected NavigationManager NavMan { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (!await UserAuth.IsAuthenticatedAsync() || !await UserAuth.IsAutorizedForAsync("Site Acceptance"))
            {
                //string returnUrl = NavMan.ToBaseRelativePath(NavMan.Uri);
                //string rt = string.Empty;

                //if (returnUrl.Length > 0)
                //{
                //    rt = $"?returnUrl={returnUrl}";
                //}

                //NavMan.NavigateTo($"Identity/Account/Login{rt}", forceLoad: true);
                NavMan.NavigateTo("access-denied");
                return;
            }

            await OnAuthenticationCheck.InvokeAsync(true);
        }
    }
}
