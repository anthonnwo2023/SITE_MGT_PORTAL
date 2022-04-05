using System.Threading;

namespace Project.V1.Web.Pages.Acceptance
{
    public partial class FormSetup/* : IDisposable*/
    {
        [Inject] public IHttpContextAccessor Context { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }
        [Inject] protected IRegion IRegion { get; set; }
        [Inject] protected ISpectrum ISpectrum { get; set; }
        [Inject] protected IAntennaType IAntennaType { get; set; }
        [Inject] protected IAntennaMake IAntennaMake { get; set; }
        [Inject] protected ISummerConfig ISummerConfig { get; set; }
        [Inject] protected IProjectType IProjectType { get; set; }
        [Inject] protected ITechType ITechType { get; set; }
        [Inject] protected IBaseBand IBaseBand { get; set; }
        [Inject] public IVendor IVendor { get; set; }


        [Inject] protected NavigationManager NavMan { get; set; }
        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        public ClaimsPrincipal Principal { get; set; }
        public List<PathInfo> Paths { get; set; }
        public List<VendorModel> Vendors { get; set; }
        public List<RegionViewModel> Regions { get; set; }
        public List<SpectrumViewModel> Spectrums { get; set; }
        public List<AntennaTypeModel> AntennaTypes { get; set; }
        public List<AntennaMakeModel> AntennaMakes { get; set; }
        public List<SummerConfigModel> SummerConfigs { get; set; }
        public List<ProjectTypeModel> ProjectTypes { get; set; }
        public List<TechTypeModel> TechTypes { get; set; }
        public List<BaseBandModel> BaseBands { get; set; }

        protected SfGrid<RegionViewModel> Grid_Region { get; set; }
        protected SfGrid<SpectrumViewModel> Grid_Spectrum { get; set; }
        protected SfGrid<AntennaMakeModel> Grid_AntennaMake { get; set; }
        protected SfGrid<AntennaTypeModel> Grid_AntennaType { get; set; }
        protected SfGrid<SummerConfigModel> Grid_SummerConfig { get; set; }
        protected SfGrid<ProjectTypeModel> Grid_ProjectType { get; set; }
        protected SfGrid<TechTypeModel> Grid_TechType { get; set; }
        protected SfGrid<BaseBandModel> Grid_BaseBand { get; set; }

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

        private async Task DoReset<T>(T data, string model)
        {
            Dictionary<string, Action<T>> processor = new()
            {
                ["SpectrumViewModel"] = async (data) =>
                {
                    try
                    {
                        await ProcessReset(ISpectrum, data as SpectrumViewModel, Spectrums);
                    }
                    catch
                    {
                        return;
                    }
                },
                ["RegionViewModel"] = async (data) =>
                {
                    try
                    {
                        await ProcessReset(IRegion, data as RegionViewModel, Regions);
                    }
                    catch
                    {
                        return;
                    }
                },
                ["AntennaMakeModel"] = async (data) =>
                {
                    try
                    {
                        await ProcessReset(IAntennaMake, data as AntennaMakeModel, AntennaMakes);
                    }
                    catch
                    {
                        return;
                    }
                },
                ["AntennaTypeModel"] = async (data) =>
                {
                    try
                    {
                        await ProcessReset(IAntennaType, data as AntennaTypeModel, AntennaTypes);
                    }
                    catch
                    {
                        return;
                    }
                },
                ["SummerConfigModel"] = async (data) =>
                {
                    try
                    {
                        await ProcessReset(ISummerConfig, data as SummerConfigModel, SummerConfigs);
                    }
                    catch
                    {
                        return;
                    }
                },
                ["ProjectTypeModel"] = async (data) =>
                {
                    try
                    {
                        await ProcessReset(IProjectType, data as ProjectTypeModel, ProjectTypes);
                    }
                    catch
                    {
                        return;
                    }
                },
                ["TechTypeModel"] = async (data) =>
                {
                    try
                    {
                        await ProcessReset(ITechType, data as TechTypeModel, TechTypes);
                    }
                    catch
                    {
                        return;
                    }
                },
                ["BaseBandModel"] = async (data) =>
                {
                    try
                    {
                        await ProcessReset(IBaseBand, data as BaseBandModel, BaseBands);
                    }
                    catch
                    {
                        return;
                    }
                },
            };

            await Task.Run(() => processor[model](data));
        }

        private async Task<bool> DoDeleteFromGrid(double Id, string model)
        {
            Dictionary<string, Func<double, bool>> processor = new()
            {
                ["SpectrumViewModel"] = (Id) =>
                {
                    try
                    {
                        Grid_Spectrum.DeleteRow(Id);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["RegionViewModel"] = (Id) =>
                {
                    try
                    {
                        Grid_Region.DeleteRow(Id);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["AntennaMakeModel"] = (Id) =>
                {
                    try
                    {
                        Grid_AntennaMake.DeleteRow(Id);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["AntennaTypeModel"] = (Id) =>
                {
                    try
                    {
                        Grid_AntennaType.DeleteRow(Id);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["SummerConfigModel"] = (Id) =>
                {
                    try
                    {
                        Grid_SummerConfig.DeleteRow(Id);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["ProjectTypeModel"] = (Id) =>
                {
                    try
                    {
                        Grid_ProjectType.DeleteRow(Id);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["TechTypeModel"] = (Id) =>
                {
                    try
                    {
                        Grid_TechType.DeleteRow(Id);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["BaseBandModel"] = (Id) =>
                {
                    try
                    {
                        Grid_BaseBand.DeleteRow(Id);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
            };

            return await Task.Run(() => processor[model](Id));
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

            if (Exists?.Name == null)
            {
                var (result, error) = await genericRepo.Create(data);

                await HandleOpResponse(grid, error, data);

                return result as T;
            }

            await HandleOpResponse(grid, "Duplicate entry found", data);

            return null;
        }

        private async Task<T> DoDelete<T>(T data, string model) where T : class
        {
            Dictionary<string, Func<T, Task<T>>> processor = new()
            {
                ["SpectrumViewModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessDelete(ISpectrum, data as SpectrumViewModel, Grid_Spectrum);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["RegionViewModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessDelete(IRegion, data as RegionViewModel, Grid_Region);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["AntennaMakeModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessDelete(IAntennaMake, data as AntennaMakeModel, Grid_AntennaMake);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["AntennaTypeModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessDelete(IAntennaType, data as AntennaTypeModel, Grid_AntennaType);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["SummerConfigModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessDelete(ISummerConfig, data as SummerConfigModel, Grid_SummerConfig);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["ProjectTypeModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessDelete(IProjectType, data as ProjectTypeModel, Grid_ProjectType);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["TechTypeModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessDelete(ITechType, data as TechTypeModel, Grid_TechType);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["BaseBandModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessDelete(IBaseBand, data as BaseBandModel, Grid_BaseBand);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
            };

            return await Task.Run(() => processor[model](data));
        }

        private async Task<T> DoAddNew<T>(T data, string model) where T : class
        {
            Dictionary<string, Func<T, Task<T>>> processor = new()
            {
                ["SpectrumViewModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessAdd(ISpectrum, data as SpectrumViewModel, Grid_Spectrum);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["RegionViewModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessAdd(IRegion, data as RegionViewModel, Grid_Region);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["AntennaMakeModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessAdd(IAntennaMake, data as AntennaMakeModel, Grid_AntennaMake);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["AntennaTypeModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessAdd(IAntennaType, data as AntennaTypeModel, Grid_AntennaType);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["SummerConfigModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessAdd(ISummerConfig, data as SummerConfigModel, Grid_SummerConfig);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["ProjectTypeModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessAdd(IProjectType, data as ProjectTypeModel, Grid_ProjectType);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["TechTypeModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessAdd(ITechType, data as TechTypeModel, Grid_TechType);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["BaseBandModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessAdd(IBaseBand, data as BaseBandModel, Grid_BaseBand);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
            };

            return await Task.Run(() => processor[model](data));
        }

        private async Task<T> DoUpdate<T>(string Id, T data, string model) where T : class
        {
            Dictionary<string, Func<T, Task<T>>> processor = new()
            {
                ["SpectrumViewModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessUpdate(ISpectrum, data as SpectrumViewModel, Grid_Spectrum, Id);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["RegionViewModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessUpdate(IRegion, data as RegionViewModel, Grid_Region, Id);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["AntennaMakeModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessUpdate(IAntennaMake, data as AntennaMakeModel, Grid_AntennaMake, Id);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["AntennaTypeModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessUpdate(IAntennaType, data as AntennaTypeModel, Grid_AntennaType, Id);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["SummerConfigModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessUpdate(ISummerConfig, data as SummerConfigModel, Grid_SummerConfig, Id);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["ProjectTypeModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessUpdate(IProjectType, data as ProjectTypeModel, Grid_ProjectType, Id);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["TechTypeModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessUpdate(ITechType, data as TechTypeModel, Grid_TechType, Id);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["BaseBandModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessUpdate(IBaseBand, data as BaseBandModel, Grid_BaseBand, Id);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
            };

            return await Task.Run(() => processor[model](data));
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
            if (model == null || model == "RegionViewModel")
                Regions = await IRegion.Get(null, x => x.OrderBy(y => y.Name));
            if (model == null || model == "AntennaTypeModel")
                AntennaTypes = await IAntennaType.Get(null, x => x.OrderBy(y => y.Name));
            if (model == null || model == "AntennaMakeModel")
                AntennaMakes = await IAntennaMake.Get(null, x => x.OrderBy(y => y.Name));
            if (model == null || model == "ProjectTypeModel")
                ProjectTypes = await IProjectType.Get(null, x => x.OrderBy(y => y.Name));
            if (model == null || model == "TechTypeModel")
                TechTypes = await ITechType.Get(null, x => x.OrderBy(y => y.Name));
            if (model == null || model == "BaseBandModel")
                BaseBands = await IBaseBand.Get(null, x => x.OrderBy(y => y.Name));
            if (model == null || model == "SummerConfigModel")
                SummerConfigs = await ISummerConfig.Get(null, x => x.OrderBy(y => y.Name));
            if (model == null || model == "SpectrumViewModel")
                Spectrums = await ISpectrum.Get(null, x => x.OrderBy(y => y.Name));
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
                await DoReset(args.Data, model);
            }
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Save)
            {
                string Id = (string)args.PrimaryKeyValue;

                if (Id == null)
                {
                    T SavedData = await DoAddNew(args.Data, model);
                    args.PrimaryKeyValue = ((dynamic)SavedData)?.Id;
                    ((dynamic)args.Data).Id = ((dynamic)SavedData)?.Id;
                }
                else
                {
                    await DoUpdate((string)args.PrimaryKeyValue, args.Data, model);
                }
            }
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Delete)
            {
                await DoDelete(args.Data, model);
                //await DoDeleteFromGrid(args.RowIndex, model);
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

        //public void Dispose()
        //{
        //    Logger.LogInformation("Executing FormSetup Disposal", new { });

        //    cts.Cancel();
        //    cts.Dispose();
        //    //IRegion?.Dispose();
        //    //IAntennaType?.Dispose();
        //    //IProjectType?.Dispose();
        //    //IBaseBand?.Dispose();
        //    //ISummerConfig?.Dispose();
        //    //ISpectrum?.Dispose();

        //    GC.SuppressFinalize(this);

        //    Logger.LogInformation("Manage Access Disposed", new { });
        //}
    }
}
