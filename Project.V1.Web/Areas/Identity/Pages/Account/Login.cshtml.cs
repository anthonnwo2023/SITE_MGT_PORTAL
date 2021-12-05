using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Project.V1.Lib.Interfaces;
using Project.V1.Lib.Helpers;
using Project.V1.DLL.Services.Interfaces;
using Project.V1.Lib.Services.Login;
using Project.V1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Project.V1.DLL.Helpers;

namespace Project.V1.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly IVendor _vendor;
        private readonly IUser _user;
        private readonly ICLogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LoginModel(ICLogger logger, IVendor vendor,
            UserManager<ApplicationUser> userManager, IUser user, SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _user = user;
            _vendor = vendor;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public SelectList VendorList { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Vendor")]
            public string VendorId { get; set; }

            [Required]
            public string Username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(ErrorMessage))
                {
                    ModelState.AddModelError(string.Empty, ErrorMessage);
                }

                returnUrl ??= Url.Content("~/");

                VendorList = new SelectList(await _vendor.Get(x => x.IsActive), "Id", "Name");
                
                // Clear the existing external cookie to ensure a clean login process
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

                ReturnUrl = returnUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, new { }, ex);
            }
        }

        [SupportedOSPlatform("windows")]
        public IUserLogin GetLoginProcessor(bool isInternal)
        {
            IUserLogin LoginProcessor = LoginFactory.Create(LoginTypes.MTN);

            if (!isInternal)
            {
                LoginProcessor.ChangeLoginType(new ExternalUserLogin());
            }

            return LoginProcessor;
        }

        [SupportedOSPlatform("windows")]
        public async Task<IActionResult> OnPostAsync(string returnUrl = null, string atype = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            _logger.LogInformation("Starting the user login process. ", new { Input.Username, Vendor = Input.VendorId });

            if (ModelState.IsValid)
            {
                try
                {
                    if (LoginObject.Vendor == null)
                        LoginObject.InitObjects();

                    SignInResponse UserSignInResult = new();

                    List<VendorModel> Vendors = await _vendor.Get();
                    VendorList = new SelectList(Vendors, "Id", "Name");
                    VendorModel MTN_Vendor = Vendors.FirstOrDefault(x => x.Name.ToUpper() == "MTN NIGERIA");

                    bool isInternal = (Input.VendorId == MTN_Vendor.Id);
                    IUserLogin LoginProcessor = GetLoginProcessor(isInternal);

                    Input.Username = Input.Username.ToLower();

                    UserSignInResult = await LoginProcessor.Login(Input.Username, Input.Password, Input.VendorId);

                    //return await LoginActionRedirect(UserSignInResult, atype, returnUrl);
                    if (UserSignInResult.Result.Succeeded)
                    {
                        _logger.LogInformation("User logged in.", new { });
                        string atye = (atype != null) ? "&atype=" + atype : "";

                        await _user.GenerateScope(User);
                        //return GetRedirectPath(returnUrl, UserSignInResult, atye, Input.Password);

                        if (UserShouldResetPassword(Input.Password))
                        {
                            return LocalRedirect($"{Request.PathBase}/profile");
                        }

                        return HasReturnUrl(returnUrl) switch
                        {
                            true => LocalRedirect($"{Request.PathBase}/{returnUrl}{atye}"),
                            _ => LocalRedirect($"{Request.PathBase}/{GetRedirectUrl(UserSignInResult.Roles.First())}?rt={UserSignInResult.UserType}")
                        };
                    }
                    if (UserSignInResult.Result.IsNotAllowed)
                    {
                        _logger.LogInformation("User not allowed to log in.", new { });
                    }
                    if (UserSignInResult.Result.IsLockedOut)
                    {
                        _logger.LogInformation("User is locked out.", new { });
                    }
                    if (UserSignInResult.Result.RequiresTwoFactor)
                    {
                        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, Input.RememberMe, rt = UserSignInResult.UserType });
                    }

                    ModelState.AddModelError(string.Empty, UserSignInResult.Message);
                    return Page();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error occurred while attempting to login", new { Input.Username, Vendor = Input.VendorId, ex.StackTrace });

                    ModelState.AddModelError("", "Internal error occurred! Login failed.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        //private async Task<IActionResult> LoginActionRedirect(SignInResponse signInResponse, string atype, string returnUrl)
        //{
        //    if (signInResponse.Result.Succeeded)
        //    {
        //        _logger.LogInformation("User logged in.", new { });
        //        string atye = (atype != null) ? "&atype=" + atype : "";

        //        await _user.GenerateScope(User);
        //        return GetRedirectPath(returnUrl, signInResponse, atye, Input.Password);
        //    }
        //    if (signInResponse.Result.IsNotAllowed)
        //    {
        //        _logger.LogInformation("User not allowed to log in.", new { });
        //        ModelState.AddModelError(string.Empty, signInResponse.Message);
        //        return Page();
        //    }
        //    if (signInResponse.Result.IsLockedOut)
        //    {
        //        _logger.LogInformation("User is locked out.", new { });
        //        ModelState.AddModelError(string.Empty, signInResponse.Message);
        //        return Page();
        //        //return RedirectToPage("./Lockout");
        //    }
        //    if (signInResponse.Result.RequiresTwoFactor)
        //    {
        //        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, Input.RememberMe, rt = signInResponse.UserType });
        //    }

        //    ModelState.AddModelError(string.Empty, signInResponse.Message);
        //    return Page();
        //}

        //private IActionResult GetRedirectPath(string returnUrl, SignInResponse UserSignInResult, string atye, string password)
        //{
        //    if (UserShouldResetPassword(password))
        //    {
        //        return RedirectToPage($"{Request.PathBase}/profile", new { ReturnUrl = returnUrl, rt = UserSignInResult.UserType });
        //    }

        //    switch (HasReturnUrl(returnUrl))
        //    {
        //        case true:
        //            {
        //                return LocalRedirect($"{Request.PathBase}/{returnUrl}{atye}");
        //            }

        //        default:
        //            {
        //                return LocalRedirect($"{Request.PathBase}/{GetRedirectUrl(UserSignInResult.Roles.First())}?rt={UserSignInResult.UserType}");
        //            }
        //    }
        //}

        private static bool HasReturnUrl(string returnUrl)
        {
            return (returnUrl != null && returnUrl != "/");
        }

        private static bool UserShouldResetPassword(string password)
        {
            return (password == "Network@55555" || password == "Password@2020");
        }

        private static string GetRedirectUrl(string role)
        {
            string uri = "acceptance/request";

            uri = role switch
            {
                "Engineer" => "acceptance",
                "Admin" or "Super Admin" => "dashboard",
                _ => uri,
            };

            return uri;
        }
    }
}
