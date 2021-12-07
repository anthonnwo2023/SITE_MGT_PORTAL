using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Project.V1.Lib.Interfaces;
using Project.V1.Lib.Extensions;
using Project.V1.Lib.Services;
using Project.V1.DLL.Services.Interfaces;
using Project.V1.DLL.Services.Interfaces.FormSetup;
using Project.V1.Models;
using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Project.V1.Web.Pages.Acceptance
{
    public partial class VendorSetup : IDisposable
    {
        [Inject] public IHttpContextAccessor Context { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }
        [Inject] protected IProjects IProjects { get; set; }
        [Inject] protected IUser IUser { get; set; }
        [Inject] public IVendor IVendor { get; set; }


        [Inject] protected NavigationManager NavMan { get; set; }
        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        public ClaimsPrincipal Principal { get; set; }
        public ApplicationUser User { get; set; }
        public List<PathInfo> Paths { get; set; }
        public List<VendorModel> Vendors { get; set; }
        public List<ProjectModel> Projects { get; set; }

        protected SfGrid<ProjectModel> Grid_Projects { get; set; }

        public List<string> ToolbarItems = new() { "Add", "Search" };

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
                new PathInfo { Name = $"Vendor Setup", Link = "acceptance/setup/vendor" },
                new PathInfo { Name = $"Acceptance", Link = "acceptance" },
            };
        }

        private async Task DoReset<T>(T data, string model)
        {
            Dictionary<string, Action<T>> processor = new()
            {
                ["ProjectModel"] = async (data) =>
                {
                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();

                        if (((dynamic)data).Id != null)
                        {
                            string Id = ((dynamic)data).Id;
                            ProjectModel DataToReset = await IProjects.GetById(x => x.Id == Id);

                            Projects.ForEach(x =>
                            {
                                if (x.Id == ((dynamic)data).Id)
                                {
                                    x.Id = DataToReset.Id;
                                    x.Name = DataToReset.Name;
                                    x.DateCreated = DataToReset.DateCreated;
                                    x.IsActive = DataToReset.IsActive;
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
                ["ProjectModel"] = (Id) =>
                {
                    try
                    {
                        Grid_Projects.DeleteRow(Id);
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
                ["ProjectModel"] = (data) =>
                {
                    try
                    {
                        Grid_Projects.UpdateRow(Id, data as ProjectModel);
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
                ["ProjectModel"] = (data) =>
                {
                    try
                    {
                        //Grid_RRUType.AddRecord(data as ProjectModel);
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
                ["ProjectModel"] = async (data) =>
                {
                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();

                        ProjectModel result = await IProjects.Update(data as ProjectModel, x => x.Id == Id);
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

        private async Task<T> DoDelete<T>(T data, string model) where T : class
        {
            Dictionary<string, Func<T, Task<T>>> processor = new()
            {
                ["ProjectModel"] = async (data) =>
                {
                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();

                        string spectrumId = (data as ProjectModel).Id;
                        ProjectModel result = await IProjects.Delete(data as ProjectModel, x => x.Id == spectrumId);

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
                ["ProjectModel"] = async (data) =>
                {
                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();
                        ProjectModel result = await IProjects.Create(data as ProjectModel);
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
                    if (!await UserAuth.IsAutorizedForAsync("Can:ManageVendorSetup"))
                    {
                        NavMan.NavigateTo("access-denied");
                        return;
                    }

                    Principal = (await AuthenticationStateTask).User;
                    User = await IUser.GetUserByUsername(Principal.Identity.Name);

                    Vendors = await IVendor.Get();
                    Projects = (User.Vendor.Name == "MTN Nigeria") ? await IProjects.Get(x => x.VendorId == User.VendorId) : await IProjects.Get(x => x.VendorId == User.VendorId);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error loading vendor setup data", new { }, ex);
                }
            }
        }

        public async Task ActionBegin<T>(ActionEventArgs<T> args, string model = null) where T : class
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {

            }
            else if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add)
            {

            }
            else if (args.RequestType == Syncfusion.Blazor.Grids.Action.Cancel)
            {
                await DoReset(args.Data, model);
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

            }
            else if (args.RequestType == Syncfusion.Blazor.Grids.Action.Delete)
            {
                await DoDeleteFromGrid(args.RowIndex, model);
            }

            StateHasChanged();
        }

        public void Dispose()
        {
            Logger.LogInformation("Executing Vendor FormSetup Disposal", new { });

            cts.Cancel();
            cts.Dispose();

            GC.SuppressFinalize(this);

            Logger.LogInformation("Vendor Manage Access Disposed", new { });
        }
    }
}
