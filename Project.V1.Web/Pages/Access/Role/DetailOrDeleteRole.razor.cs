using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Project.V1.Data.Interfaces;
using Project.V1.DLL.Extensions;
using Project.V1.DLL.Services.Interfaces;
using Project.V1.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.V1.Web.Pages.Access.Role
{
    public partial class DetailOrDeleteRole
    {
        [Parameter] public string Id { get; set; } = string.Empty;
        [Inject] protected NavigationManager NavMan { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] protected RoleManager<IdentityRole> Role { get; set; }
        public List<PathInfo> Paths { get; set; }
        public string PageText { get; set; } = "Delete";
        public IdentityRole RoleModel { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }

        protected override void OnInitialized()
        {
            Paths = new()
            {
                new PathInfo { Name = $"{PageText} Role", Link = "access/role/delete" },
                new PathInfo { Name = "Manage Access", Link = "access" },
                new PathInfo { Name = "Settings", Link = "access" },
            };
        }

        protected async Task AuthenticationCheck(bool isAuthenticated)
        {
            if (isAuthenticated)
            {
                try
                {
                    if (!await UserAuth.IsAutorizedForAsync("Can:DeleteRole") && !await UserAuth.IsAutorizedForAsync("Can:ViewRole"))
                    {
                        NavMan.NavigateTo("access-denied");
                    }

                    RoleModel = await Role.FindByIdAsync(Id);

                    Logger.LogInformation("Loading Role", new { });
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error loading Role", new { }, ex);
                }
            }
        }
        protected async Task HandleDeleteRole()
        {
            try
            {
                if (Id != null)
                {
                    RoleModel = await Role.FindByIdAsync(Id);

                    if (RoleModel != null)
                    {
                        await Role.DeleteAsync(RoleModel);
                    }
                }

                NavMan.NavigateTo("access");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error Deleting Role", new { }, ex);
            }
        }
    }
}
