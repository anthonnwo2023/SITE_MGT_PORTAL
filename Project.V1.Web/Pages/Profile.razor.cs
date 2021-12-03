using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Project.V1.DLL.Services.Interfaces;
using Project.V1.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.V1.Web.Pages
{
    public partial class Profile
    {
        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }
        [Inject] protected NavigationManager NavMan { get; set; }
        [Inject] protected IUser User { get; set; }
        [Inject] protected UserManager<ApplicationUser> UserManager { get; set; }
        [Inject] protected IHttpContextAccessor HttpContextAccessor { get; set; }

        protected bool IsUpdateSuccessful { get; set; } = false;
        protected bool ShowUpdateNotification { get; set; } = false;
        public ApplicationUser UserData { get; set; } = new();

        public List<PathInfo> Paths { get; set; }
        public string UserType { get; set; }
        public string Direction { get; set; } = "row";
        public string PwdChgMessage { get; set; }
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        protected override void OnInitialized()
        {
            Paths = new()
            {
                new PathInfo { Name = $"Profile", Link = "profile" },
            };
        }

        protected async Task AuthenticationCheck(bool isAuthenticated)
        {
            if (isAuthenticated)
            {
                try
                {
                    System.Security.Claims.ClaimsPrincipal user = HttpContextAccessor.HttpContext.User;

                    UserData = await User.GetUserByUsername(user.Identity.Name);
                    UserType = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(UserData.UserType));

                    Direction = (UserType == "External") ? "column" : Direction;
                }
                catch (System.Exception)
                {

                    throw;
                }
            }
        }

        private void CloseDialog()
        {
            ShowUpdateNotification = false;
        }

        protected async Task HandleValidSubmit()
        {
            try
            {
                string token = await UserManager.GeneratePasswordResetTokenAsync(UserData);
                IdentityResult resultChangedPW = await UserManager.ResetPasswordAsync(UserData, token, Input.Password);

                if (resultChangedPW.Succeeded)
                {
                    UserData.IsNewPassword = false;
                    await UserManager.UpdateAsync(UserData);

                    IsUpdateSuccessful = true;

                    PwdChgMessage = "Password Changed Successfully.";
                }

                ShowUpdateNotification = true;
            }
            catch (Exception ex)
            {
                PwdChgMessage = $"Password failed to change. Please try again later";
                Log.Logger.Error(ex, $"{ex.Message} - {UserData.UserName} {UserData.Fullname}");

                ShowUpdateNotification = true;
            }
        }
    }
}
