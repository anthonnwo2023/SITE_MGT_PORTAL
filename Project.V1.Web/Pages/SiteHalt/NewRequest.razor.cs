namespace Project.V1.Web.Pages.SiteHalt
{
    public partial class NewRequest
    {
        [Inject] protected IUserAuthentication UserAuth { get; set; }
        [Inject] protected NavigationManager NavMan { get; set; }
        [Inject] public ICLogger Logger { get; set; }

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        public ClaimsPrincipal Principal { get; set; }
        public ApplicationUser User { get; set; }
        public List<PathInfo> Paths { get; set; }
        public SiteHaltRequestModel SiteHaltRequestModel { get; set; }

        private string ToastPosition { get; set; } = "Right";
        public string ToastTitle { get; set; } = "Error Notification";
        public string ToastContent { get; set; }
        public string ToastCss { get; set; } = "e-toast-danger";

        public bool DisableButton { get; set; } = true;
        public string UploadIconCss { get; set; } = "fas fa-paper-plane ml-2";

        protected SfToast ToastObj { get; set; }

        protected override void OnInitialized()
        {
            Paths = new()
            {
                new PathInfo { Name = $"New Request", Link = "halt/request" },
                new PathInfo { Name = $"Halt | Unhalt | Decom", Link = "halt" },
            };
        }

        protected async Task AuthenticationCheck(bool isAuthenticated)
        {
            if (isAuthenticated)
            {
                try
                {
                    Logger.LogInformation("Loading create site halt & unhult request page", new { });

                    if (!await UserAuth.IsAutorizedForAsync("Can:AddRequest"))
                    {
                        NavMan.NavigateTo("access-denied");
                    }

                    SiteHaltRequestModel = new();
                }
                catch (Exception ex)
                {
                    Logger.LogError("Error creating request", new { }, ex);
                }
            }
        }

        private readonly List<ToastModel> Toast = new()
        {
            new ToastModel { Title = "Warning!", Content = "There was a problem with your network connection.", CssClass = "e-toast-warning", Icon = "e-warning toast-icons", Timeout = 0 },
            new ToastModel { Title = "Success!", Content = "Your message has been sent successfully.", CssClass = "e-toast-success", Icon = "e-success toast-icons", Timeout = 0 },
            new ToastModel { Title = "Error!", Content = "A problem has been occurred while submitting your data.", CssClass = "e-toast-danger", Icon = "e-error toast-icons", Timeout = 0 },
            new ToastModel { Title = "Information!", Content = "Please read the comments carefully.", CssClass = "e-toast-info", Icon = "e-info toast-icons", Timeout = 0 }
        };

        private async void SuccessBtnOnClick()
        {
            Toast[1].Content = ToastContent;

            await this.ToastObj.Show(Toast[1]);
        }

        private async void ErrorBtnOnClick()
        {
            Toast[2].Content = ToastContent;

            await this.ToastObj.Show(Toast[2]);
        }

        public void OnToastClickHandler(ToastClickEventArgs args)
        {
            args.ClickToClose = true;
        }

        private void EnableDisableActionButton(bool IsSERRUType)
        {
            DisableButton = false;
        }

        private static void OnClear(ClearingEventArgs args)
        {
            return;
        }

        private static void OnRemove(RemovingEventArgs args)
        {
            return;
        }

        private static void IsSEValid(bool SEValid)
        {
            return;
        }
        private static async Task<bool> OnFileSSVUploadChange(UploadChangeEventArgs args, string type)
        {
            return await Task.Run(() => true);
        }

        protected async Task HandleValidSubmit()
        {
            try
            {
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Logger.LogError("Error creating request. ", new { }, ex);
            }
        }
    }
}
