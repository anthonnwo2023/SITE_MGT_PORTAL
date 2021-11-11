using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Project.V1.Lib.Extensions;
using Project.V1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.V1.Web.Pages.Components
{
    public partial class Breadcrumb
    {
        [Parameter] public List<PathInfo> Paths { get; set; }
        [Parameter] public EventCallback<bool> OnAuthenticationCheck { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }
        [Inject] protected IHttpContextAccessor HttpContext { get; set; }
        [Inject] protected NavigationManager NavMan { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (!await UserAuth.IsAuthenticatedAsync())
            {
                string rt = string.Empty;
                string returnUrl = NavMan.ToBaseRelativePath(NavMan.Uri);

                returnUrl = (returnUrl == "access-denied" || returnUrl.Contains("logout")) ? null : returnUrl;

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    rt = $"?returnUrl={returnUrl}";
                }

                NavMan.NavigateTo($"Identity/Account/Login{rt}", forceLoad: true);
                return;
            }

            await OnAuthenticationCheck.InvokeAsync(true);
        }
    }
}
