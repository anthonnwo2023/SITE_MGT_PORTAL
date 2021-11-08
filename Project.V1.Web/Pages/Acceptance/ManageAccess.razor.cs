using Hanssens.Net;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Project.V1.Data.Interfaces;
using Project.V1.DLL.Extensions;
using Project.V1.DLL.Helpers;
using Project.V1.DLL.Services;
using Project.V1.DLL.Services.Interfaces;
using Project.V1.Models;
using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace Project.V1.Web.Pages.Acceptance
{
    public partial class ManageAccess : IDisposable
    {
        [Inject] public IHttpContextAccessor Context { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] public IVendor IVendor { get; set; }
        [Inject] public IUser IUser { get; set; }
        [Inject] public IClaimService IClaim { get; set; }
        [Inject] public IClaimCategory IClaimCategory { get; set; }
        [Inject] public RoleManager<IdentityRole> Role { get; set; }
        [Inject] public UserManager<ApplicationUser> User { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }
        [Inject] public IClaimService Claim { get; set; }
        [Inject] protected NavigationManager NavMan { get; set; }
        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        public List<PathInfo> Paths { get; set; }
        public int Level { get; set; }
        public List<VendorModel> Vendors { get; set; }
        public List<IdentityRole> IdentityRolesOld { get; set; }
        public List<ApplicationUser> ApplicationUsersOld { get; set; }
        public List<ExpandoObject> ApplicationUsers { get; set; } = new List<ExpandoObject>();
        public List<ExpandoObject> IdentityRoles { get; set; } = new List<ExpandoObject>();
        public List<ClaimViewModel> ClaimModels { get; set; }
        public List<ClaimCategoryModel> ClaimCategories { get; set; }
        //public List<Claim> UserClaims { get; set; }
        public List<ClaimListManager> UserClaims { get; set; }
        public List<ClaimViewModel> RoleClaims { get; set; }
        public List<ClaimViewModel> ProjectClaims { get; set; }

        public string[] SelectedUserRoles { get; set; }
        public List<ClaimViewModel> SelectedRolePermissions { get; set; }
        public string[] SelectedUserProjects { get; set; }

        public bool ShouldRequirePassword { get; set; } = false;

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

        protected SfGrid<VendorModel> Grid_Vendor { get; set; }
        protected SfGrid<ExpandoObject> Grid_IdentityRole { get; set; }
        protected SfGrid<ClaimViewModel> Grid_Claim { get; set; }
        protected SfGrid<ClaimCategoryModel> Grid_ClaimCategory { get; set; }
        protected SfGrid<ExpandoObject> Grid_User { get; set; }

        public List<string> ToolbarItems = new() { "Add", "ColumnChooser", "Search" };

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
                new PathInfo { Name = "Manage Access", Link = "access" },
            };
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
                        if (((dynamic)data).Id != null)
                        {
                            List<ApplicationUser> FullData = await User.Users.ToListAsync();
                            ApplicationUser DataToReset = FullData.FirstOrDefault(x => x.Id == ((dynamic)data).Id);

                            ApplicationUsers.ForEach(x =>
                            {
                                if (((dynamic)x).Id == ((dynamic)data).Id)
                                {
                                    ((dynamic)x).Id = DataToReset.Id;
                                    ((dynamic)x).Fullname = DataToReset.Fullname;
                                    ((dynamic)x).DateCreated = DataToReset.DateCreated;
                                    ((dynamic)x).IsActive = DataToReset.IsActive;
                                    //((dynamic)x).Department = DataToReset.Department;
                                    ((dynamic)x).UserName = DataToReset.UserName;
                                    ((dynamic)x).Email = DataToReset.Email;
                                    //((dynamic)x).JobTitle = DataToReset.JobTitle;
                                    ((dynamic)x).PhoneNumber = DataToReset.PhoneNumber;
                                    //((dynamic)x).Region = DataToReset.Region;
                                    ((dynamic)x).Vendor = DataToReset.Vendor;
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
                        Grid_User.UpdateRow(Id, data as ExpandoObject);
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

                        VendorModel result = await IVendor.Update(data as VendorModel, x => x.Id == Id);
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

                        var dataDict = ((IDictionary<string, object>)(data as ExpandoObject));
                        var roleObj = Role.FindByIdAsync(dataDict["Id"].ToString()).GetAwaiter().GetResult();

                        roleObj.Name = dataDict["Name"].ToString();

                        ((dynamic)data).Id = roleObj.Id;
                        var result = await Role.UpdateAsync(roleObj);

                        if (result.Succeeded)
                        {
                            var userClaims = RoleClaims.Where(x => x.IsSelected).ToList();

                            await Role.AddRoleClaims(roleObj, SelectedRolePermissions);
                        }

                        return data as T;
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

                        ClaimViewModel result = await IClaim.Update(data as ClaimViewModel, x => x.Id == Id);
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

                        ClaimCategoryModel result = await IClaimCategory.Update(data as ClaimCategoryModel, x => x.Id == Id);
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

                        //var result = await User.UpdateAsync(data as ApplicationUser);
                        var dataDict = ((IDictionary<string, object>)(data as ExpandoObject));

                        ApplicationUser user = new()
                        {
                            Id = dataDict["Id"].ToString(),
                            Fullname = dataDict["Fullname"].ToString(),
                            UserName = dataDict["UserName"].ToString(),
                            Email = dataDict["Email"].ToString(),
                            PhoneNumber = dataDict["PhoneNumber"].ToString(),
                            JobTitle = dataDict["JobTitle"].ToString(),
                            Department = dataDict["Department"].ToString(),
                            IsActive = Convert.ToBoolean(dataDict["IsActive"]),
                            VendorId = dataDict["VendorId"].ToString(),
                            //Roles = dataDict["Roles"] as string[],
                        };

                        var result = await IUser.UpdateUser(user, SelectedUserPassword, SelectedUserRoles.ToList());

                        if (result)
                        {
                            var userClaims = UserClaims.SelectMany(x => x.Claims).Where(x => SelectedUserProjects.Contains(x.Id)).ToList();

                            await User.AddUserClaims(user, userClaims);
                        }

                        return data as T;
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
                        VendorModel result = await IVendor.Delete(data as VendorModel, x => x.Id == projectId);
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

                        var dataDict = ((IDictionary<string, object>)(data as ExpandoObject));

                        var role = await Role.FindByIdAsync(dataDict["Id"].ToString());

                        var result = await Role.DeleteAsync(role);
                        return data as T;
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
                        ClaimViewModel result = await IClaim.Delete(data as ClaimViewModel, x => x.Id == subCategoryId);
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
                        ClaimCategoryModel result = await IClaimCategory.Delete(data as ClaimCategoryModel, x => x.Id == statusId);
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
                        var result = await User.DeleteAsync(data as ApplicationUser);
                        return data as T;
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
                        VendorModel result = await IVendor.Create(data as VendorModel);
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
                        var dataDict = ((IDictionary<string, object>)(data as ExpandoObject));

                        IdentityRole role = new()
{
                            Id = (dataDict.ContainsKey("Id")) ? dataDict["Id"].ToString() : default,
                            Name = dataDict["Name"].ToString(),
                            NormalizedName = dataDict["NormalizedName"].ToString(),
                        };

                        //var roleMapped = (data as ExpandoObject).Map<IdentityRole>(role);
                        ((dynamic)data).Id = role.Id;
                        //((dynamic)data).NormalizedName = role.NormalizedName;

                        var result = await Role.CreateAsync(role);

                        if (result.Succeeded)
                        {
                            var userClaims = RoleClaims.Where(x => x.IsSelected).ToList();

                            await Role.AddRoleClaims(role, SelectedRolePermissions);
                        }

                        return data as T;
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

                        ClaimViewModel result = await IClaim.Create(data as ClaimViewModel);
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

                        ClaimCategoryModel result = await IClaimCategory.Create(data as ClaimCategoryModel);
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

                        var dataDict = ((IDictionary<string, object>)(data as ExpandoObject));

                        ApplicationUser user = new()
                        {
                            Id = (dataDict.ContainsKey("Id")) ? dataDict["Id"].ToString() : default,
                            Fullname = dataDict["Fullname"].ToString(),
                            UserName = dataDict["UserName"].ToString(),
                            Email = dataDict["Email"].ToString(),
                            PhoneNumber = dataDict["PhoneNumber"].ToString(),
                            JobTitle = dataDict["JobTitle"].ToString(),
                            Department = dataDict["Department"].ToString(),
                            IsActive = Convert.ToBoolean(dataDict["IsActive"]),
                            VendorId = dataDict["VendorId"].ToString(),
                        };


                        var (result, Id) = await IUser.CreateUser(user, SelectedUserPassword, SelectedUserRoles.ToList());

                        if (result)
                        {
                            var userClaims = UserClaims.SelectMany(x => x.Claims).Where(x => SelectedUserProjects.Contains(x.Id)).ToList();

                            await User.AddUserClaims(user, userClaims);
                        }

                        ((dynamic)data).Id = Id;

                        return data as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
            };

            return await Task.Run(() => processor[model](data));
        }

        protected async Task AuthenticationCheck(bool isAuthenticated)
        {
            if (isAuthenticated)
            {
                try
                {
                    if (!await UserAuth.IsAutorizedForAsync("Can:ManageAccess"))
                    {
                        NavMan.NavigateTo("access-denied");
                        return;
                    }

                    ClaimsPrincipal user = (await AuthenticationStateTask).User;
                    ApplicationUser userData = await IUser.GetUserByUsername(user.Identity.Name);

                    Vendors = await IVendor.Get();
                    StateHasChanged();

                    RoleClaims = (await Claim.Get(x => x.IsActive)).Where(x => x.Category.Name != "Project").GroupBy(x => x.Category.Name).Select(x => new ClaimListManager
                    {
                        Category = x.Key,
                        Claims = x.ToList().FormatClaimSelection()
                    }).ToList().SelectMany(x => x.Claims).ToList();

                    IdentityRoles = Role.Roles.ToList().Select(x =>
                    {
                        dynamic d = new ExpandoObject();
                        d.Id = x.Id;
                        d.Name = x.Name;
                        d.NormalizedName = x.NormalizedName;
                        d.ConcurrencyStamp = x.ConcurrencyStamp;

                        var roleClaims = (Claim.Get(y => y.IsActive).GetAwaiter().GetResult()).Where(z => z.Category.Name != "Project").GroupBy(v => v.Category.Name).Select(u => new ClaimListManager
                        {
                            Category = u.Key,
                            Claims = u.ToList().FormatClaimSelection(x)
                        }).ToList();

                        d.Permissions = roleClaims.SelectMany(x => x.Claims).Where(x => x.IsSelected).ToList();

                        return d;
                    }).Cast<ExpandoObject>().ToList<ExpandoObject>();

                    ClaimModels = (await IClaim.Get()).OrderBy(x => x.ClaimName).ToList();
                    ClaimCategories = (await IClaimCategory.Get()).ToList();

                    UserClaims = (await Claim.Get(y => y.IsActive)).Where(z => z.Category.Name == "Project").GroupBy(v => v.Category.Name).Select(u => new ClaimListManager
                    {
                        Category = u.Key,
                        Claims = u.ToList().FormatClaimSelection()
                    }).ToList();

                    ProjectClaims = UserClaims.SelectMany(x => x.Claims).ToList();

                    StateHasChanged();
                    ApplicationUsers = (await IUser.GetUsers()).Select(x =>
                    {
                        var mtnVendor = Vendors.First(y => y.Name == "MTN Nigeria");

                        dynamic d = new ExpandoObject();
                        d.Id = x.Id;
                        d.Fullname = x.Fullname;
                        d.UserName = x.UserName;
                        d.Email = x.Email;
                        d.PhoneNumber = x.PhoneNumber;
                        d.JobTitle = x.JobTitle;
                        d.Department = x.Department;
                        d.DateCreated = x.DateCreated;
                        d.LastLoginDate = x.LastLoginDate;
                        d.ConcurrencyStamp = x.ConcurrencyStamp;
                        d.VendorId = x.VendorId;
                        d.IsMTNUser = (mtnVendor != null) ? !(mtnVendor.Id == x.VendorId) : true;
                        d.Password = "";
                        d.ConfirmPassword = "";
                        d.IsActive = x.IsActive;
                        d.Roles = (IUser.GetUserRolesId(x).GetAwaiter().GetResult()).ToArray();
                        var userClaims = (Claim.Get(y => y.IsActive).GetAwaiter().GetResult()).Where(z => z.Category.Name == "Project").GroupBy(v => v.Category.Name).Select(u => new ClaimListManager
                        {
                            Category = u.Key,
                            Claims = u.ToList().FormatClaimSelection(x)
                        }).ToList();

                        d.Projects = userClaims.SelectMany(x => x.Claims).Where(x => x.IsSelected).ToList();

                        return d;
                    }).Cast<ExpandoObject>().ToList<ExpandoObject>();
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
                    SelectedUserRoles = ((dynamic)args.Data).Roles;
                    SelectedUserProjects = (((dynamic)args.Data).Projects as List<ClaimViewModel>).Select(x => x.Id).ToArray();

                    //var visibles = Grid_User.Columns.Where(x => x.Visible).Select(x => x.HeaderText).ToArray();
                    //var hiddens = Grid_User.Columns.Where(x => !x.Visible).Select(x => x.HeaderText).ToArray();
                    var visibles = new string[] { "Username", "Fullname", "Phone Number", "Email", "JobTitle", "Department", "Roles", "Projects", "Vendor", "Is Active", "Created", "Password", "Confirm Password" };
                    var hiddens = new string[] { "Id" };
                    await Grid_User.ShowColumnsAsync(visibles);
                    await Grid_User.HideColumnsAsync(hiddens);
                }
                if(model == "IdentityRole")
                {
                    SelectedRolePermissions = ((dynamic)args.Data).Permissions;

                    var visibles = new string[] { "Permissions", "Role Name" };
                    var hiddens = new string[] { "Id" };
                    await Grid_IdentityRole.ShowColumnsAsync(visibles);
                    await Grid_IdentityRole.HideColumnsAsync(hiddens);
                }
            }
            else if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add)
            {
                if (model == "ApplicationUser")
                {
                    var visibles = new string[] { "Username", "Fullname", "Phone Number", "Email", "JobTitle", "Department", "Roles", "Projects", "Vendor", "Is Active", "Created", "Password", "Confirm Password" };
                    var hiddens = new string[] { "Id" };
                    await Grid_User.ShowColumnsAsync(visibles);
                    await Grid_User.HideColumnsAsync(hiddens);
                }
                if (model == "IdentityRole")
                {
                    SelectedRolePermissions = ((dynamic)args.Data).Permissions;

                    var visibles = new string[] { "Permissions", "Role Name" };
                    var hiddens = new string[] { "Id" };
                    await Grid_IdentityRole.ShowColumnsAsync(visibles);
                    await Grid_IdentityRole.HideColumnsAsync(hiddens);
                }
            }
            else if (args.RequestType == Syncfusion.Blazor.Grids.Action.Cancel)
            {
                await DoReset(args.Data, model);

                if (model == "ApplicationUser")
                {
                    var visibles = new string[] { "Fullname", "Phone Number", "Email", "JobTitle", "Department", "Roles", "Projects", "Vendor", "Is Active", "Created",  };
                    var hiddens = new string[] { "Id", "Password", "Confirm Password", "Username" };
                    await Grid_User.ShowColumnsAsync(visibles);
                    await Grid_User.HideColumnsAsync(hiddens);
                }
                if (model == "IdentityRole")
                {
                    SelectedRolePermissions = ((dynamic)args.Data).Permissions;

                    var visibles = new string[] { "Role Name" };
                    var hiddens = new string[] { "Id", "Permissions" };
                    await Grid_IdentityRole.ShowColumnsAsync(visibles);
                    await Grid_IdentityRole.HideColumnsAsync(hiddens);
                }
            }
            else if (args.RequestType == Syncfusion.Blazor.Grids.Action.Save)
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
                if (args.RowIndex == 0)
                {
                    await DoGridAddNew(args.RowData, model);
                }
                else
                {
                    await DoGridUpdate(args.RowIndex, args.Data, model);
                }

                if (model == "ApplicationUser")
                {
                    Grid_User.Refresh();
                
                    var visibles = new string[] { "Fullname", "Phone Number", "Email", "JobTitle", "Department", "Roles", "Projects", "Vendor", "Is Active", "Created", };
                    var hiddens = new string[] { "Id", "Password", "Confirm Password", "Username" };
                    await Grid_User.ShowColumnsAsync(visibles);
                    await Grid_User.HideColumnsAsync(hiddens);
                }
                if (model == "IdentityRole")
                {
                    SelectedRolePermissions = ((dynamic)args.Data).Permissions;

                    var visibles = new string[] { "Role Name" };
                    var hiddens = new string[] { "Id", "Permissions" };
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
            IVendor?.Dispose();
            IUser?.Dispose();
            IClaim?.Dispose();
            IClaimCategory?.Dispose();
            Role?.Dispose();

            GC.SuppressFinalize(this);

            Logger.LogInformation("Manage Access Disposed", new { });
        }
    }
}
