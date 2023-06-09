namespace Project.V1.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly IVendor _vendor;
        private readonly IUser _user;
        private readonly ICLogger _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LoginModel(SignInManager<ApplicationUser> signInManager,
           ICLogger logger, IVendor vendor, IConfiguration configuration, IUser user)
        {
            _logger = logger;
            Configuration = configuration;
            _signInManager = signInManager;
            _user = user;
            _vendor = vendor;
        }

        public IConfiguration Configuration { get; }

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
                TempData["IsLoggingIn"] = true;

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
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            _logger.LogInformation("Starting the user login process. ", new { Input.Username, Vendor = Input.VendorId });

            try
            {
                LoginObject.InitObjects();

                SignInResponse UserSignInResult = new();

                List<VendorModel> Vendors = (await _vendor.Get()).ToList();
                VendorList = new SelectList(Vendors, "Id", "Name");
                VendorModel MTN_Vendor = Vendors.FirstOrDefault(x => x.Name.ToUpper() == "MTN NIGERIA");

                bool isInternal = (Input.VendorId == MTN_Vendor.Id);
                IUserLogin LoginProcessor = GetLoginProcessor(isInternal);

                Input.Username = Input.Username.ToLower();
                TempData["Password"] = Input.Password.ToLower();

                if (ModelState.IsValid)
                {
                    UserSignInResult = await LoginProcessor.Login(Input.Username, Input.Password, Input.VendorId);

                    return await LoginActionRedirect(UserSignInResult, atype, returnUrl);
                }

                ModelState.AddModelError("", UserSignInResult.Message);
                // If we got this far, something failed, redisplay form
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while attempting to login", new { Input.Username, Vendor = Input.VendorId, ex.StackTrace });

                ModelState.AddModelError("", "Internal error occurred! Login failed.");
                return Page();
            }
        }

        private async Task<IActionResult> LoginActionRedirect(SignInResponse signInResponse, string atype, string returnUrl)
        {
            if (signInResponse.Result.Succeeded)
            {
                _logger.LogInformation("User logged in.", new { });
                string atye = (atype != null) ? "&atype=" + atype : "";

                LoginObject.ContextAccessor.HttpContext.User = await LoginObject.SignInManager.CreateUserPrincipalAsync(signInResponse.User);

                return GetRedirectPath(returnUrl, signInResponse, atye, Input.Password);
            }
            if (signInResponse.Result.IsNotAllowed)
            {
                _logger.LogInformation("User not allowed to log in.", new { });
            }
            if (signInResponse.Result.IsLockedOut)
            {
                _logger.LogInformation("User is locked out.", new { });
                //return RedirectToPage("./Lockout");
            }
            if (signInResponse.Result.RequiresTwoFactor)
            {
                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, Input.RememberMe, rt = signInResponse.UserType });
            }

            ModelState.AddModelError(string.Empty, signInResponse.Message);
            return Page();
        }

        private IActionResult GetRedirectPath(string returnUrl, SignInResponse UserSignInResult, string atye, string password)
        {

            if (UserShouldResetPassword(password) || UserSignInResult.User.IsNewPassword)
            {
                return LocalRedirect($"{Request.PathBase}/profile");
            }

            switch (HasReturnUrl(returnUrl))
            {
                case true:
                    {
                        return LocalRedirect($"{Request.PathBase}/{returnUrl}{atye}");
                    }

                default:
                    {
                        return LocalRedirect($"{Request.PathBase}/{GetRedirectUrl(UserSignInResult)}?rt={UserSignInResult.UserType}");
                    }
            }
        }

        private static bool HasReturnUrl(string returnUrl)
        {
            return (returnUrl != null && returnUrl != "/");
        }

        private static bool UserShouldResetPassword(string password)
        {
            return (password == "Network@55555" || password == "Password@2020");
        }

        private static string GetRedirectUrl(SignInResponse response)
        {
            string uri = "dashboard";
            var abbr = string.Empty;

            if (response.User.Projects.Any())
            {
                var claimNameChunk = response.User.Projects[0].Name.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => x[..1].ToUpper());
                abbr = string.Join("", claimNameChunk);
            }

            uri = response.User.Projects.Count switch
            {
                var count when count == 1 => HelperFunctions.GetAppLink(abbr),
                _ => uri,
            };

            return uri;
        }
    }
}
