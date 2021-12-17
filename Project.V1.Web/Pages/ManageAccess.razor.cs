using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.V1.DLL.Helpers;
using Project.V1.DLL.Services.Interfaces;
using Project.V1.DLL.Services.Interfaces.FormSetup;
using Project.V1.Lib.Extensions;
using Project.V1.Lib.Interfaces;
using Project.V1.Lib.Services;
using Project.V1.Models;
using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Notifications.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

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

        public string[] SelectedUserRoles { get; set; }
        public string[] SelectedUserRegions { get; set; }
        public List<ClaimViewModel> SelectedRolePermissions { get; set; }
        public string[] SelectedUserProjects { get; set; }

        public bool ShouldRequirePassword { get; set; } = false;

        //[RequiredWhen(nameof(ShouldRequirePassword), true, AllowEmptyStrings = false, ErrorMessage = "Password is required for vendor accounts.")]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string SelectedUserPassword { get; set; } = "Network@55555";

        //[RequiredWhen(nameof(ShouldRequirePassword), true, AllowEmptyStrings = false, ErrorMessage = "Password is required for vendor accounts.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare(nameof(SelectedUserPassword), ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = "Network@55555";

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

        private async Task DoReset<T>(T data, string model)
        {
            Dictionary<string, Action<T>> processor = new()
            {
                ["VendorModel"] = async (data) =>
                {
                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();

                        if (((dynamic)data).Id != null)
                        {
                            List<VendorModel> FullData = await IVendor.Get();
                            VendorModel DataToReset = FullData.FirstOrDefault(x => x.Id == ((dynamic)data).Id);

                            Vendors.ForEach(x =>
                            {
                                if (x.Id == ((dynamic)data).Id)
                                {
                                    x.Id = DataToReset.Id;
                                    x.Name = DataToReset.Name;
                                    x.DateCreated = DataToReset.DateCreated;
                                    x.IsActive = DataToReset.IsActive;
                                    x.MailList = DataToReset.MailList;
                                }
                            });
                        }
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
                        cts.Token.ThrowIfCancellationRequested();
                        if (((dynamic)data).Id != null)
                        {
                            List<ClaimViewModel> FullData = await IClaim.Get();
                            ClaimViewModel DataToReset = FullData.FirstOrDefault(x => x.Id == ((dynamic)data).Id);

                            ClaimModels.ForEach(x =>
                            {
                                if (x.Id == ((dynamic)data).Id)
                                {
                                    x.Id = DataToReset.Id;
                                    x.Category = DataToReset.Category;
                                    x.DateCreated = DataToReset.DateCreated;
                                    x.IsActive = DataToReset.IsActive;
                                    x.IsSelected = DataToReset.IsSelected;
                                    x.ClaimName = DataToReset.ClaimName;
                                    x.ClaimValue = DataToReset.ClaimValue;
                                }
                            });
                        }
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
                        cts.Token.ThrowIfCancellationRequested();
                        if (((dynamic)data).Id != null)
                        {
                            List<ClaimCategoryModel> FullData = await IClaimCategory.Get();
                            ClaimCategoryModel DataToReset = FullData.FirstOrDefault(x => x.Id == ((dynamic)data).Id);

                            ClaimCategories.ForEach(x =>
                            {
                                if (x.Id == ((dynamic)data).Id)
                                {
                                    x.Id = DataToReset.Id;
                                    x.Name = DataToReset.Name;
                                    x.DateCreated = DataToReset.DateCreated;
                                    x.IsActive = DataToReset.IsActive;
                                }
                            });
                        }
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

        private async Task<bool> DoDeleteFromGrid(double Id, string model)
        {
            Dictionary<string, Func<double, bool>> processor = new()
            {
                ["VendorModel"] = (Id) =>
                {
                    try
                    {
                        Grid_Vendor.DeleteRow(Id);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["IdentityRole"] = (Id) =>
                {
                    try
                    {
                        Grid_IdentityRole.DeleteRow(Id);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["ClaimViewModel"] = (Id) =>
                {
                    try
                    {
                        Grid_Claim.DeleteRow(Id);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["ClaimCategoryModel"] = (Id) =>
                {
                    try
                    {
                        Grid_ClaimCategory.DeleteRow(Id);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["ApplicationUser"] = (Id) =>
                {
                    try
                    {
                        Grid_User.DeleteRow(Id);
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

        private async Task<bool> DoGridUpdate<T>(double Id, T data, string model)
        {
            Dictionary<string, Func<T, bool>> processor = new()
            {
                ["VendorModel"] = (data) =>
                {
                    try
                    {
                        Grid_Vendor.UpdateRow(Id, data as VendorModel);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["IdentityRole"] = (data) =>
                {
                    try
                    {
                        Grid_IdentityRole.UpdateRow(Id, data as ExpandoObject);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["ClaimViewModel"] = (data) =>
                {
                    try
                    {
                        Grid_Claim.UpdateRow(Id, data as ClaimViewModel);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["ClaimCategoryModel"] = (data) =>
                {
                    try
                    {
                        Grid_ClaimCategory.UpdateRow(Id, data as ClaimCategoryModel);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["ApplicationUser"] = (data) =>
                {
                    try
                    {
                        Grid_User.UpdateRow(Id, data as ApplicationUser);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
            };

            return await Task.Run(() => processor[model](data));
        }

        private static async Task<bool> DoGridAddNew<T>(T data, string model)
        {
            Dictionary<string, Func<T, bool>> processor = new()
            {
                ["VendorModel"] = (data) =>
                {
                    try
                    {
                        //Grid_Project.AddRecord(data as VendorModel);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["IdentityRole"] = (data) =>
                {
                    try
                    {
                        //Grid_IdentityRole.AddRecord(data as NESICTRequestCategory);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["ClaimViewModel"] = (data) =>
                {
                    try
                    {
                        //Grid_Claim.AddRecord(data as ClaimViewModel);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["ClaimCategoryModel"] = (data) =>
                {
                    try
                    {
                        //Grid_ClaimCategory.AddRecord(data as ClaimCategoryModel);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["ApplicationUser"] = (data) =>
                {
                    try
                    {
                        //Grid_User.AddRecord(data as ApplicationUser);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
            };

            return await Task.Run(() => processor[model](data));
        }

        private async Task<T> DoUpdate<T>(string Id, T data, string model) where T : class
        {
            Dictionary<string, Func<T, Task<T>>> processor = new()
            {
                ["VendorModel"] = async (data) =>
                {
                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();

                        var (result, error) = await IVendor.Update(data as VendorModel, x => x.Id == Id);

                        await HandleOpResponse(Grid_Vendor, error, data as VendorModel);

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
                        cts.Token.ThrowIfCancellationRequested();

                        var (result, error) = await IClaim.Update(data as ClaimViewModel, x => x.Id == Id);

                        await HandleOpResponse(Grid_Claim, error, data as ClaimViewModel);

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
                        cts.Token.ThrowIfCancellationRequested();

                        var (result, error) = await IClaimCategory.Update(data as ClaimCategoryModel, x => x.Id == Id);

                        await HandleOpResponse(Grid_ClaimCategory, error, data as ClaimCategoryModel);

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

                        bool result = await IUser.UpdateUser(user, SelectedUserPassword, SelectedUserRoles.ToList());

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
                        cts.Token.ThrowIfCancellationRequested();
                        string projectId = (data as VendorModel).Id;
                        var (result, error) = await IVendor.Delete(data as VendorModel, x => x.Id == projectId);

                        await HandleOpResponse(Grid_Vendor, error, data as VendorModel);

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

                        IdentityRole role = await Role.FindByIdAsync(dataDict["Id"].ToString());

                        IdentityResult result = await Role.DeleteAsync(role);
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
                        cts.Token.ThrowIfCancellationRequested();
                        string subCategoryId = (data as ClaimViewModel).Id;
                        var (result, error) = await IClaim.Delete(data as ClaimViewModel, x => x.Id == subCategoryId);

                        await HandleOpResponse(Grid_Claim, error, data as ClaimViewModel);

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
                        cts.Token.ThrowIfCancellationRequested();
                        string statusId = (data as ClaimCategoryModel).Id;
                        var (result, error) = await IClaimCategory.Delete(data as ClaimCategoryModel, x => x.Id == statusId);

                        await HandleOpResponse(Grid_ClaimCategory, error, data as ClaimCategoryModel);

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
                        string stageId = (data as ApplicationUser).Id;
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
                        cts.Token.ThrowIfCancellationRequested();

                        var Exists = await IVendor.GetById(x => x.Name == (data as VendorModel).Name);

                        if (Exists == null)
                        {
                            var (result, error) = await IVendor.Create(data as VendorModel);

                            await HandleOpResponse(Grid_Vendor, error, data as VendorModel);

                            return result as T;
                        }

                        await HandleOpResponse(Grid_Vendor, "Duplicate entry found", data as VendorModel);

                        return null;
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

                        //var roleMapped = (data as ExpandoObject).Map<IdentityRole>(role);
                        ((dynamic)data).Id = role.Id;
                        //((dynamic)data).NormalizedName = role.NormalizedName;

                        IdentityResult result = await Role.CreateAsync(role);

                        if (result.Succeeded)
                        {
                            List<ClaimViewModel> userClaims = RoleClaims.Where(x => x.IsSelected).ToList();

                            await Role.AddRoleClaims(role, SelectedRolePermissions);

                            await HandleOpResponse(Grid_IdentityRole, "", data as ExpandoObject);
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
                        cts.Token.ThrowIfCancellationRequested();

                        var Exists = await IClaim.GetById(x => x.ClaimName == (data as ClaimViewModel).ClaimName);

                        if (Exists == null)
                        {
                            var (result, error) = await IClaim.Create(data as ClaimViewModel);

                            await HandleOpResponse(Grid_Claim, error, data as ClaimViewModel);

                            return result as T;
                        }

                        await HandleOpResponse(Grid_Claim, "Duplicate entry found", data as ClaimViewModel);

                        return null;
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
                        cts.Token.ThrowIfCancellationRequested();

                        var Exists = await IClaimCategory.GetById(x => x.Name == (data as ClaimCategoryModel).Name);

                        if (Exists == null)
                        {
                            var (result, error) = await IClaimCategory.Create(data as ClaimCategoryModel);

                            await HandleOpResponse(Grid_ClaimCategory, error, data as ClaimCategoryModel);

                            return result as T;
                        }

                        await HandleOpResponse(Grid_ClaimCategory, "Duplicate entry found", data as ClaimCategoryModel);

                        return null;
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

                        (bool result, string response) = await IUser.CreateUser(user, SelectedUserPassword, SelectedUserRoles.ToList());

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

        private async Task InitData(string model = null)
        {
            var ActiveClaims = await Claim.Get(y => y.IsActive);

            if (model == null || model == "VendorModel")
                Vendors = (await IVendor.Get()).OrderBy(x => x.Name).ToList();

            if (model == null || model == "IdentityRole")
            {
                RoleClaims = ActiveClaims.OrderBy(x => x.Category.Name).Where(x => x.Category.Name != "Project").GroupBy(x => x.Category.Name).Select(async x => new ClaimListManager
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
                ClaimModels = (await IClaim.Get()).OrderBy(x => x.ClaimName).ToList();

            if (model == null || model == "ClaimCategoryModel")
                ClaimCategories = (await IClaimCategory.Get()).OrderBy(x => x.Name).ToList();

            if (model == null || model == "ApplicationUser")
            {
                Regions = (await IRegion.Get()).OrderBy(x => x.Name).ToList();

                UserClaims = ActiveClaims.Where(z => z.Category.Name == "Project").GroupBy(v => v.Category.Name).Select(u => new ClaimListManager
                {
                    Category = u.Key,
                    Claims = u.ToList().FormatClaimSelection().GetAwaiter().GetResult()
                }).ToList();

                ProjectClaims = UserClaims.SelectMany(x => x.Claims).OrderBy(x => x.ClaimName).ToList();

                ApplicationUsers = (await IUser.GetUsers()).OrderBy(x => x.Vendor.Name).ToList();

                ApplicationUsers.ForEach((user) =>
                {
                    VendorModel mtnVendor = Vendors.First(y => y.Name == "MTN Nigeria");

                    user.IsMTNUser = mtnVendor == null || !(mtnVendor.Id == user.VendorId);
                    user.Roles = (IUser.GetUserRolesId(user).GetAwaiter().GetResult()).ToArray();
                    List<ClaimListManager> userClaims = (Claim.Get(y => y.IsActive).GetAwaiter().GetResult()).Where(z => z.Category.Name == "Project").GroupBy(v => v.Category.Name).Select(u => new ClaimListManager
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

        public async Task ActionBegin<T>(ActionEventArgs<T> args, string model = null) where T : class
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                if (model == "ApplicationUser")
                {
                    SelectedUserRoles = ((dynamic)args.RowData).Roles;
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
                await DoDeleteFromGrid(args.RowIndex, model);
            }
        }

        public async Task ActionComplete<T>(ActionEventArgs<T> args, string model) where T : class
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
            }
            else if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add)
            {
            }
            else if (args.RequestType == Syncfusion.Blazor.Grids.Action.Cancel)
            {

            }
            else if (args.RequestType == Syncfusion.Blazor.Grids.Action.Save)
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
            else if (args.RequestType == Syncfusion.Blazor.Grids.Action.Delete)
            {
                await DoDeleteFromGrid(args.RowIndex, model);
            }

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
