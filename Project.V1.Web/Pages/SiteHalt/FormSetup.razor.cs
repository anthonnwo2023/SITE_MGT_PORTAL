using Syncfusion.Blazor.DropDowns;
using System.Threading;

namespace Project.V1.Web.Pages.SiteHalt
{
    public partial class FormSetup
    {
        [Inject] public IHttpContextAccessor Context { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }
        [Inject] public IVendor IVendor { get; set; }


        [Inject] protected NavigationManager NavMan { get; set; }
        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        public ClaimsPrincipal Principal { get; set; }
        public List<PathInfo> Paths { get; set; }
        public List<VendorModel> Vendors { get; set; }


        public List<string> ToolbarItems = new() { "Add", "Search" };

        private string ToastPosition { get; set; } = "Right";
        public string ToastTitle { get; set; } = "Error Notification";
        public string ToastContent { get; set; }
        public string ToastCss { get; set; } = "e-toast-danger";
        protected SfToast ToastObj { get; set; }

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

        private async Task HandleOpResponse<T>(SfGrid<T> sfGrid, string error, T data) where T : class
        {
            if (error.Length == 0)
            {
                ToastContent = "Operation Successful.";
                await Task.Delay(200);
                SuccessBtnOnClick();
            }
            else
            {
                ToastContent = error;
                await Task.Delay(200);
                ErrorBtnOnClick();
            }
        }


        public IEditorSettings DropdownEditParams = new DropDownEditCellParams
        {
            Params = new DropDownListModel<object, object>() { AllowFiltering = true, ShowClearButton = true }
        };

        public IEditorSettings DateEditParams = new DateEditCellParams
        {
            Params = new DatePickerModel() { EnableRtl = true, ShowClearButton = false }
        };

        public IEditorSettings DateEditReadonlyParams = new DateEditCellParams
        {
            Params = new DatePickerModel() { EnableRtl = true, ShowClearButton = false, Readonly = true }
        };

        public DialogSettings DialogEditParams = new()
        {
            Width = "450px",
        };

        private readonly CancellationTokenSource cts = new();

        protected override void OnInitialized()
        {
            Paths = new()
            {
                new PathInfo { Name = $"Request Setup", Link = "acceptance/setup" },
                new PathInfo { Name = $"Acceptance", Link = "acceptance" },
            };
        }

        private async Task ProcessReset<T>(IGenericRepo<T> genericRepo, T data, List<T> items) where T : ObjectBase
        {
            cts.Token.ThrowIfCancellationRequested();

            if (data.Id != null)
            {
                string Id = data.Id;

                var DBData4Reset = await genericRepo.GetById(x => x.Id == Id);

                var changedData = items.First(x => x.Id == Id);
                changedData = DBData4Reset;
            }
        }

        private async Task<T> ProcessUpdate<T>(IGenericRepo<T> genericRepo, T data, SfGrid<T> grid, string Id) where T : ObjectBase
        {
            cts.Token.ThrowIfCancellationRequested();

            var (result, error) = await genericRepo.Update(data, x => x.Id == Id);

            await HandleOpResponse(grid, error, data);

            return result as T;
        }

        private async Task<T> ProcessDelete<T>(IGenericRepo<T> genericRepo, T data, SfGrid<T> grid) where T : ObjectBase
        {
            cts.Token.ThrowIfCancellationRequested();

            string spectrumId = data.Id;
            var (result, error) = await genericRepo.Delete(data, x => x.Id == spectrumId);

            await HandleOpResponse(grid, error, data);


            return result as T;
        }

        private async Task<T> ProcessAdd<T>(IGenericRepo<T> genericRepo, T data, SfGrid<T> grid) where T : ObjectBase
        {
            cts.Token.ThrowIfCancellationRequested();

            var Exists = await genericRepo.GetById(x => x.Name == data.Name);

            if (Exists == null)
            {
                var (result, error) = await genericRepo.Create(data);

                await HandleOpResponse(grid, error, data);

                return result as T;
            }

            await HandleOpResponse(grid, "Duplicate entry found", data);

            return null;
        }


        protected bool CheckClaimPermission(string claimName)
        {
            return Principal.HasClaim(x => x.Type == claimName);
        }

        protected async Task AuthenticationCheck(bool isAuthenticated)
        {
            if (isAuthenticated)
            {
                try
                {
                    if (!await UserAuth.IsAutorizedForAsync("Can:ManageSetupSA"))
                    {
                        NavMan.NavigateTo("access-denied");
                        return;
                    }

                    Principal = (await AuthenticationStateTask).User;

                    await InitData();
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error loading setup data", new { }, ex);
                }
            }
        }

        private async Task InitData(string model = null)
        {
            if (model == null)
                Vendors = await IVendor.Get(null, x => x.OrderBy(y => y.Name));
        }

        public async Task ActionBegin<T>(ActionEventArgs<T> args, string model = null) where T : class
        {
            //if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            //{

            //}
            //else if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add)
            //{

            //}
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Cancel)
            {
            }
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Save)
            {

            }
        }

        public async Task ActionComplete<T>(ActionEventArgs<T> args, string model) where T : class
        {
            //if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            //{
            //}
            //else if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add)
            //{
            //}
            //else if (args.RequestType == Syncfusion.Blazor.Grids.Action.Cancel)
            //{

            //}
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Save)
            {
                await InitData(model);
            }
            //else if (args.RequestType == Syncfusion.Blazor.Grids.Action.Delete)
            //{
            //    //await DoDeleteFromGrid(args.RowIndex, model);
            //}

            StateHasChanged();
        }
    }
}
