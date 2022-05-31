using System.Dynamic;
using System.Threading;

namespace Project.V1.Web.Pages
{
    public partial class ManageAccess : IDisposable
    {
        [Inject] public IHttpContextAccessor Context { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] public IVendor IVendor { get; set; }
        [Inject] public IRegion IRegion { get; set; }
        [Inject] public IUser IUser { get; set; }
        [Inject] public IClaimService IClaim { get; set; }
        [Inject] public IClaimCategory IClaimCategory { get; set; }
        [Inject] public RoleManager<IdentityRole> Role { get; set; }
        [Inject] public UserManager<ApplicationUser> User { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }
        [Inject] public IClaimService Claim { get; set; }
        [Inject] protected NavigationManager NavMan { get; set; }
        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        public ClaimsPrincipal Principal { get; set; }
        public List<PathInfo> Paths { get; set; }
        public int Level { get; set; }
        public List<VendorModel> Vendors { get; set; }
        public List<RegionViewModel> Regions { get; set; }
        public List<IdentityRole> IdentityRolesOld { get; set; }
        public List<ApplicationUser> ApplicationUsersOld { get; set; }
        //public List<ExpandoObject> ApplicationUsers { get; set; } = new List<ExpandoObject>();
        public List<ApplicationUser> ApplicationUsers { get; set; } = new();
        public List<ExpandoObject> IdentityRoles { get; set; } = new List<ExpandoObject>();
        public List<ClaimViewModel> ClaimModels { get; set; }
        public List<ClaimCategoryModel> ClaimCategories { get; set; }
        //public List<Claim> UserClaims { get; set; }
        public List<ClaimListManager> UserClaims { get; set; }
        public List<ClaimViewModel> RoleClaims { get; set; }
        public List<ClaimViewModel> ProjectClaims { get; set; }

        public string[] SelectedUserRoles { get; set; } = Array.Empty<string>();
        public string[] SelectedUserRegions { get; set; } = Array.Empty<string>();
        public List<ClaimViewModel> SelectedRolePermissions { get; set; }
        public string[] SelectedUserProjects { get; set; } = Array.Empty<string>();

        public bool ShouldRequirePassword { get; set; } = false;

        public SecureInput SecureUserInput { get; set; } = new();

        public class SecureInput
        {
            //[RequiredWhen(nameof(ShouldRequirePassword), true, AllowEmptyStrings = false, ErrorMessage = "Password is required for vendor accounts.")]
            //[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string SelectedUserPassword { get; set; }

            //[RequiredWhen(nameof(ShouldRequirePassword), true, AllowEmptyStrings = false, ErrorMessage = "Password is required for vendor accounts.")]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare(nameof(SelectedUserPassword), ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        protected SfGrid<VendorModel> Grid_Vendor { get; set; }
        protected SfGrid<ExpandoObject> Grid_IdentityRole { get; set; }
        protected SfGrid<ClaimViewModel> Grid_Claim { get; set; }
        protected SfGrid<ClaimCategoryModel> Grid_ClaimCategory { get; set; }
        protected SfGrid<ApplicationUser> Grid_User { get; set; }

        public List<string> ToolbarItems = new() { "Add", "ColumnChooser", "Search" };

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

        protected override async Task OnInitializedAsync()
        {
            Paths = new()
            {
                new PathInfo { Name = "Manage Access", Link = "access" },
            };

            await Task.CompletedTask;
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

        private async Task DoReset<T>(T data, string model)
        {
            Dictionary<string, Action<T>> processor = new()
            {
                ["VendorModel"] = async (data) =>
                {
                    try
                    {
                        await ProcessReset(IVendor, data as VendorModel, Vendors);
                    }
                    catch
                    {
                        return;
                    }
                },
                ["IdentityRole"] = async (data) =>
                {
                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();
                        if (((dynamic)data).Id != null)
                        {
                            List<IdentityRole> FullData = await Role.Roles.ToListAsync();
                            IdentityRole DataToReset = FullData.FirstOrDefault(x => x.Id == ((dynamic)data).Id);

                            IdentityRoles.ForEach(x =>
                            {
                                if (((dynamic)x).Id == ((dynamic)data).Id)
                                {
                                    ((dynamic)x).Id = DataToReset.Id;
                                    ((dynamic)x).Name = DataToReset.Name;
                                    ((dynamic)x).NormalizedName = DataToReset.NormalizedName;
                                    ((dynamic)x).ConcurrencyStamp = DataToReset.ConcurrencyStamp;
                                }
                            });
                        }
                    }
                    catch
                    {
                        return;
                    }
                },
                ["ClaimViewModel"] = async (data) =>
                {
                    try
                    {
                        await ProcessReset(IClaim, data as ClaimViewModel, ClaimModels);
                    }
                    catch
                    {
                        return;
                    }
                },
                ["ClaimCategoryModel"] = async (data) =>
                {
                    try
                    {
                        await ProcessReset(IClaimCategory, data as ClaimCategoryModel, ClaimCategories);
                    }
                    catch
                    {
                        return;
                    }
                },
                ["ApplicationUser"] = async (data) =>
                {
                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();
                        if ((data as ApplicationUser).Id != null)
                        {
                            List<ApplicationUser> FullData = await User.Users.ToListAsync();
                            ApplicationUser DataToReset = FullData.FirstOrDefault(x => x.Id == (data as ApplicationUser).Id);

                            ApplicationUsers.ForEach(x =>
                            {
                                if (x.Id == (data as ApplicationUser).Id)
                                {
                                    x.Id = DataToReset.Id;
                                    x.Fullname = DataToReset.Fullname;
                                    x.DateCreated = DataToReset.DateCreated;
                                    x.IsActive = DataToReset.IsActive;
                                    //x.Department = DataToReset.Department;
                                    x.UserName = DataToReset.UserName;
                                    x.Email = DataToReset.Email;
                                    //x.JobTitle = DataToReset.JobTitle;
                                    x.PhoneNumber = DataToReset.PhoneNumber;
                                    x.Regions = DataToReset.Regions;
                                    x.VendorId = DataToReset.VendorId;
                                }
                            });
                        }
                    }
                    catch
                    {
                        return;
                    }
                },
            };

            await Task.Run(() => processor[model](data));
        }

        private async Task<T> DoUpdate<T>(string Id, T data, string model) where T : class
        {
            Dictionary<string, Func<T, Task<T>>> processor = new()
            {
                ["VendorModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessUpdate(IVendor, data as VendorModel, Grid_Vendor, Id);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["IdentityRole"] = async (data) =>
                {
                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();

                        IDictionary<string, object> dataDict = data as ExpandoObject;
                        IdentityRole roleObj = Role.FindByIdAsync(dataDict["Id"].ToString()).GetAwaiter().GetResult();

                        roleObj.Name = dataDict["Name"].ToString();

                        ((dynamic)data).Id = roleObj.Id;
                        IdentityResult result = await Role.UpdateAsync(roleObj);

                        if (result.Succeeded)
                        {
                            List<ClaimViewModel> userClaims = RoleClaims.Where(x => x.IsSelected).ToList();

                            await Role.AddRoleClaims(roleObj, SelectedRolePermissions);
                        }

                        return data;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["ClaimViewModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessUpdate(IClaim, data as ClaimViewModel, Grid_Claim, Id);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["ClaimCategoryModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessUpdate(IClaimCategory, data as ClaimCategoryModel, Grid_ClaimCategory, Id);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["ApplicationUser"] = async (data) =>
                {
                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();

                        var user = data as ApplicationUser;

                        bool result = await IUser.UpdateUser(user, SecureUserInput.SelectedUserPassword, SelectedUserRoles.ToList());

                        if (result)
                        {
                            List<ClaimViewModel> userClaims = UserClaims.SelectMany(x => x.Claims).Where(x => SelectedUserProjects.Contains(x.Id)).ToList();

                            await User.AddUserClaims(user, userClaims);

                            ApplicationUsers.FirstOrDefault(x => x.Id == user.Id).Regions = user.Regions;
                        }

                        return user as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
            };

            return await Task.Run(() => processor[model](data));
        }

        private async Task<T> DoDelete<T>(T data, string model) where T : class
        {
            Dictionary<string, Func<T, Task<T>>> processor = new()
            {
                ["VendorModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessDelete(IVendor, data as VendorModel, Grid_Vendor);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["IdentityRole"] = async (data) =>
                {
                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();

                        IDictionary<string, object> dataDict = data as ExpandoObject;

                        IdentityRole role = await Role.Roles.FirstOrDefaultAsync(x => x.Id == dataDict["Id"].ToString());

                        if (role != null)
                        {
                            IdentityResult result = await Role.DeleteAsync(role);
                            return data;
                        }

                        return null;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["ClaimViewModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessDelete(IClaim, data as ClaimViewModel, Grid_Claim);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["ClaimCategoryModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessDelete(IClaimCategory, data as ClaimCategoryModel, Grid_ClaimCategory);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["ApplicationUser"] = async (data) =>
                {
                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();
                        string userId = (data as ApplicationUser).Id;

                        IdentityResult result = await User.DeleteAsync(data as ApplicationUser);
                        return data;
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
                ["VendorModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessAdd(IVendor, data as VendorModel, Grid_Vendor);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["IdentityRole"] = async (data) =>
                {
                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();
                        IDictionary<string, object> dataDict = data as ExpandoObject;

                        IdentityRole role = new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = dataDict["Name"].ToString(),
                        };

                        ((dynamic)data).Id = role.Id;

                        IdentityResult result = await Role.CreateAsync(role);

                        if (result.Succeeded)
                        {
                            List<ClaimViewModel> userClaims = RoleClaims.Where(x => x.IsSelected).ToList();

                            await Role.AddRoleClaims(role, SelectedRolePermissions);

                            await HandleOpResponse(Grid_IdentityRole, "", data as ExpandoObject);

                            return data;
                        }

                        await HandleOpResponse(Grid_IdentityRole, $"An error as occured. {string.Join(". ", result.Errors.Select(x => x.Description))}", data as ExpandoObject);
                        return data;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["ClaimViewModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessAdd(IClaim, data as ClaimViewModel, Grid_Claim);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["ClaimCategoryModel"] = async (data) =>
                {
                    try
                    {
                        var result = await ProcessAdd(IClaimCategory, data as ClaimCategoryModel, Grid_ClaimCategory);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["ApplicationUser"] = async (data) =>
                {
                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();

                        var user = data as ApplicationUser;

                        (bool result, string response) = await IUser.CreateUser(user, SecureUserInput.SelectedUserPassword, SelectedUserRoles.ToList());

                        if (result)
                        {
                            List<ClaimViewModel> userClaims = UserClaims.SelectMany(x => x.Claims).Where(x => SelectedUserProjects.Contains(x.Id)).ToList();

                            await User.AddUserClaims(user, userClaims);

                            ((dynamic)data).Regions = user.Regions;

                            ToastContent = "Account created successfully.";
                            await Task.Delay(200);
                            SuccessBtnOnClick();

                            return data;
                        }

                        ToastContent = response;
                        await Task.Delay(200);
                        ErrorBtnOnClick();

                        await Grid_User.DeleteRow(user.Id);

                        return null;
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
            //return (Principal != null) ? Principal.HasClaim(x => x.Type == claimName) : false;
            return (Principal != null) && Principal.HasClaim(x => x.Type == claimName);
        }

        private void RolesChangeHandler(MultiSelectChangeEventArgs<string[]> args)
        {
            SelectedUserRoles = args?.Value;
        }

        private void ProjectsChangeHandler(MultiSelectChangeEventArgs<string[]> args)
        {
            SelectedUserProjects = args?.Value;
        }

        private void RegionsChangeHandler(MultiSelectChangeEventArgs<string[]> args)
        {
            SelectedUserRegions = args?.Value;
        }

        private async Task InitData(string model = null)
        {
            var ActiveClaims = await Claim.Get(y => y.IsActive, null, "Category");

            if (model == null || model == "VendorModel")
            {
                Vendors = (await IVendor.Get()).OrderBy(x => x.Name).ToList();
            }

            if (model == null || model == "IdentityRole")
            {
                RoleClaims = ActiveClaims.ToList().OrderBy(x => x.Category.Name).Where(x => x.Category.Name != "Project").GroupBy(x => x.Category.Name)
                    .ToList()
                    .Select(async x => new ClaimListManager
                    {
                        Category = x.Key,
                        Claims = await x.ToList().FormatClaimSelection()
                    }).SelectMany(x => (x.GetAwaiter().GetResult()).Claims).ToList();

                IdentityRoles = await Task.FromResult((await Role.Roles.ToListAsync()).OrderBy(x => x.Name).Select(x =>
                {
                    dynamic d = new ExpandoObject();
                    d.Id = x.Id;
                    d.Name = x.Name;
                    d.NormalizedName = x.NormalizedName;
                    d.ConcurrencyStamp = x.ConcurrencyStamp;

                    List<ClaimListManager> roleClaims = ActiveClaims.Where(z => z.Category.Name != "Project").GroupBy(v => v.Category.Name).Select(u => new ClaimListManager
                    {
                        Category = u.Key,
                        Claims = u.ToList().FormatClaimSelection(x)
                    }).ToList();

                    d.Permissions = roleClaims.SelectMany(x => x.Claims).Where(x => x.IsSelected).ToList();

                    return d;
                }).Cast<ExpandoObject>().ToList());
            }

            if (model == null || model == "ClaimViewModel")
            {
                ClaimModels = (await IClaim.Get()).OrderBy(x => x.Name).ToList();
            }

            if (model == null || model == "ClaimCategoryModel")
            {
                ClaimCategories = (await IClaimCategory.Get()).OrderBy(x => x.Name).ToList();
            }

            if (model == null || model == "ApplicationUser")
            {
                Regions = (await IRegion.Get()).OrderBy(x => x.Name).ToList();

                UserClaims = ActiveClaims.Where(z => z.Category.Name == "Project").GroupBy(v => v.Category.Name).Select(u => new ClaimListManager
                {
                    Category = u.Key,
                    Claims = u.ToList().FormatClaimSelection().GetAwaiter().GetResult()
                }).ToList();

                ProjectClaims = UserClaims.SelectMany(x => x.Claims).OrderBy(x => x.Name).ToList();

                ApplicationUsers = (await IUser.GetUsersNoTrack()).OrderBy(x => x.Vendor.Name).ToList();

                ApplicationUsers.ForEach((user) =>
                {
                    VendorModel mtnVendor = Vendors.First(y => y.Name == "MTN Nigeria");

                    user.IsMTNUser = mtnVendor == null || !(mtnVendor.Id == user.VendorId);
                    //user.Roles = (IUser.GetUserRolesId(user).GetAwaiter().GetResult()).ToArray();
                    List<ClaimListManager> userClaims = (Claim.Get(y => y.IsActive, null, "Category").GetAwaiter().GetResult()).Where(z => z.Category.Name == "Project").GroupBy(v => v.Category.Name).Select(u => new ClaimListManager
                    {
                        Category = u.Key,
                        Claims = u.ToList().FormatClaimSelection(user)
                    }).ToList();

                    user.Projects = userClaims.SelectMany(x => x.Claims).Where(x => x.IsSelected).ToList();

                });
            }
        }

        protected async Task AuthenticationCheck(bool isAuthenticated)
        {
            if (isAuthenticated)
            {
                try
                {
                    await InvokeAsync(async () =>
                    {
                        if (!await UserAuth.IsAutorizedForAsync("Can:ManageAccess"))
                        {
                            NavMan.NavigateTo("access-denied");
                            return;
                        }

                        Principal = (await AuthenticationStateTask).User;
                        ApplicationUser userData = await IUser.GetUserByUsername(Principal.Identity.Name);

                        await InitData();
                    });
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error loading Access data", new { }, ex);
                }
            }
        }

        public void OnPasswordInput(InputEventArgs args)
        {
            SecureUserInput.SelectedUserPassword = args.Value;

            StateHasChanged();
        }

        public void OnConfirmPasswordInput(InputEventArgs args)
        {
            SecureUserInput.ConfirmPassword = args.Value;

            StateHasChanged();
        }

        public async Task ActionBegin<T>(ActionEventArgs<T> args, string model = null) where T : class
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                if (model == "ApplicationUser")
                {
                    string[] roles = (string[])((dynamic)args.RowData).Roles;
                    var userRoles = IdentityRoles.Where(x => roles.Contains(((dynamic)x).Name as string)).Select(x => (string)((dynamic)x).Id).ToArray();

                    SelectedUserRoles = userRoles;
                    SelectedUserProjects = ((((dynamic)args.RowData).Projects as List<ClaimViewModel>) == null) ? Array.Empty<string>()
                        : (((dynamic)args.RowData).Projects as List<ClaimViewModel>).Select(x => x.Id).ToArray();
                    SelectedUserRegions = ((((dynamic)args.RowData).Regions as List<RegionViewModel>) == null) ? Array.Empty<string>()
                        : (((dynamic)args.RowData).Regions as List<RegionViewModel>).Select(x => x.Id).ToArray();

                    ((dynamic)args.Data).Id = ((dynamic)args.RowData).Id;
                    ((dynamic)args.Data).UserName = ((dynamic)args.RowData).UserName;
                    ((dynamic)args.Data).Email = ((dynamic)args.RowData).Email;
                    ((dynamic)args.Data).PhoneNumber = ((dynamic)args.RowData).PhoneNumber;

                    //var visibles = Grid_User.Columns.Where(x => x.Visible).Select(x => x.HeaderText).ToArray();
                    //var hiddens = Grid_User.Columns.Where(x => !x.Visible).Select(x => x.HeaderText).ToArray();
                    var visibles = new string[] { "Username", "Fullname", "Phone Number", "Email", "Job Title", "Department", "Roles", "Projects", "Vendor", "Regions", "Is Active", "Created", "Password", "Confirm Password" };
                    var hiddens = new string[] { "Id" };
                    await Grid_User.ShowColumnsAsync(visibles);
                    await Grid_User.HideColumnsAsync(hiddens);
                }
                if (model == "IdentityRole")
                {
                    SelectedRolePermissions = ((dynamic)args.Data).Permissions;

                    ((dynamic)args.Data).Id = ((dynamic)args.RowData).Id;
                    ((dynamic)args.Data).Name = ((dynamic)args.RowData).Name;

                    string[] visibles = new string[] { "Permissions", "Role Name" };
                    string[] hiddens = new string[] { "Id" };
                    await Grid_IdentityRole.ShowColumnsAsync(visibles);
                    await Grid_IdentityRole.HideColumnsAsync(hiddens);
                }
            }
            else if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add)
            {
                if (model == "ApplicationUser")
                {
                    SelectedUserRoles = Array.Empty<string>();
                    SelectedUserProjects = Array.Empty<string>();
                    SelectedUserRegions = Array.Empty<string>();

                    string[] visibles = new string[] { "Username", "Fullname", "Phone Number", "Email", "Job Title", "Department", "Roles", "Projects", "Vendor", "Regions", "Is Active", "Created", "Password", "Confirm Password" };
                    string[] hiddens = new string[] { "Id" };
                    await Grid_User.ShowColumnsAsync(visibles);
                    await Grid_User.HideColumnsAsync(hiddens);
                }
                if (model == "IdentityRole")
                {
                    SelectedRolePermissions = new();
                    ((dynamic)args.Data).Permissions = new List<ClaimViewModel>();

                    string[] visibles = new string[] { "Permissions", "Role Name" };
                    string[] hiddens = new string[] { "Id" };
                    await Grid_IdentityRole.ShowColumnsAsync(visibles);
                    await Grid_IdentityRole.HideColumnsAsync(hiddens);
                }
            }
            else if (args.RequestType == Syncfusion.Blazor.Grids.Action.Cancel)
            {
                await DoReset(args.Data, model);

                if (model == "ApplicationUser")
                {
                    string[] visibles = new string[] { "Fullname", "Phone Number", "Email", "Job Title", "Department", "Roles", "Projects", "Vendor", "Is Active", "Created", };
                    string[] hiddens = new string[] { "Id", "Password", "Confirm Password", "Username", "Regions" };
                    await Grid_User.ShowColumnsAsync(visibles);
                    await Grid_User.HideColumnsAsync(hiddens);
                }
                if (model == "IdentityRole")
                {
                    string[] visibles = new string[] { "Role Name" };
                    string[] hiddens = new string[] { "Id", "Permissions" };
                    await Grid_IdentityRole.ShowColumnsAsync(visibles);
                    await Grid_IdentityRole.HideColumnsAsync(hiddens);
                }
            }
            else if (args.RequestType == Syncfusion.Blazor.Grids.Action.Save)
            {
                string Id = (string)args.PrimaryKeyValue;

                var oldEntry = Id != null;

                if (model == "ApplicationUser")
                {
                    oldEntry = ApplicationUsers.Any(x => x.Id == Id);

                    ((dynamic)args.Data).Roles = SelectedUserRoles;
                    ((dynamic)args.Data).Projects = ProjectClaims.Where(x => SelectedUserProjects.Contains(x.Id)).ToList();
                    ((dynamic)args.Data).Regions = Regions.Where(x => SelectedUserRegions.Contains(x.Id)).ToList();
                }

                if (!oldEntry)
                {
                    T SavedData = await DoAddNew(args.Data, model);
                    //args.PrimaryKeyValue = ((dynamic)SavedData)?.Id;
                    //((dynamic)args.Data).Id = ((dynamic)SavedData)?.Id;
                }
                else
                {
                    await DoUpdate((string)args.PrimaryKeyValue, args.Data, model);
                }
            }
            else if (args.RequestType == Syncfusion.Blazor.Grids.Action.Delete)
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

                if (model == "ApplicationUser")
                {
                    Grid_User.Refresh();

                    string[] visibles = new string[] { "Fullname", "Phone Number", "Email", "Job Title", "Department", "Roles", "Projects", "Vendor", "Is Active", "Created", };
                    string[] hiddens = new string[] { "Id", "Password", "Confirm Password", "Username", "Regions" };
                    await Grid_User.ShowColumnsAsync(visibles);
                    await Grid_User.HideColumnsAsync(hiddens);
                }
                if (model == "IdentityRole")
                {
                    SelectedRolePermissions = ((dynamic)args.Data).Permissions;

                    string[] visibles = new string[] { "Role Name" };
                    string[] hiddens = new string[] { "Id", "Permissions" };
                    await Grid_IdentityRole.ShowColumnsAsync(visibles);
                    await Grid_IdentityRole.HideColumnsAsync(hiddens);
                }
            }
            //else if (args.RequestType == Syncfusion.Blazor.Grids.Action.Delete)
            //{
            //    //await DoDeleteFromGrid(args.RowIndex, model);
            //}

            StateHasChanged();
        }

        public void Dispose()
        {
            Logger.LogInformation("Executing FormSetup Disposal", new { });

            cts.Cancel();
            cts.Dispose();
            //IVendor?.Dispose();
            //IUser?.Dispose();
            //IClaim?.Dispose();
            //IClaimCategory?.Dispose();

            GC.SuppressFinalize(this);

            Logger.LogInformation("Manage Access Disposed", new { });
        }
    }
}
