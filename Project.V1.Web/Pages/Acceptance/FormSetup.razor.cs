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
    public partial class FormSetup : IDisposable
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
        [Inject] protected IRRUType IRRUType { get; set; }
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
        public List<RRUTypeModel> RRUTypes { get; set; }
        public List<TechTypeModel> TechTypes { get; set; }
        public List<BaseBandModel> BaseBands { get; set; }

        protected SfGrid<RegionViewModel> Grid_Region { get; set; }
        protected SfGrid<SpectrumViewModel> Grid_Spectrum { get; set; }
        protected SfGrid<AntennaMakeModel> Grid_AntennaMake { get; set; }
        protected SfGrid<AntennaTypeModel> Grid_AntennaType { get; set; }
        protected SfGrid<SummerConfigModel> Grid_SummerConfig { get; set; }
        protected SfGrid<ProjectTypeModel> Grid_ProjectType { get; set; }
        protected SfGrid<TechTypeModel> Grid_TechType { get; set; }
        protected SfGrid<RRUTypeModel> Grid_RRUType { get; set; }
        protected SfGrid<BaseBandModel> Grid_BaseBand { get; set; }

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
                new PathInfo { Name = $"Request Setup", Link = "acceptance/setup" },
                new PathInfo { Name = $"Acceptance", Link = "acceptance" },
            };
        }

        private async Task DoReset<T>(T data, string model)
        {
            Dictionary<string, Action<T>> processor = new()
            {
                ["RRUTypeModel"] = async (data) =>
                {
                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();

                        if (((dynamic)data).Id != null)
                        {
                            string Id = ((dynamic)data).Id;
                            RRUTypeModel DataToReset = await IRRUType.GetById(x => x.Id == Id);

                            RRUTypes.ForEach(x =>
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
                ["SpectrumViewModel"] = async (data) =>
                {
                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();

                        if (((dynamic)data).Id != null)
                        {
                            string Id = ((dynamic)data).Id;
                            SpectrumViewModel DataToReset = await ISpectrum.GetById(x => x.Id == Id);

                            Spectrums.ForEach(x =>
                            {
                                if (x.Id == ((dynamic)data).Id)
                                {
                                    x.Id = DataToReset.Id;
                                    x.Name = DataToReset.Name;
                                    x.DateCreated = DataToReset.DateCreated;
                                    x.IsActive = DataToReset.IsActive;
                                    x.TechTypeId = DataToReset.TechTypeId;
                                }
                            });
                        }
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
                        cts.Token.ThrowIfCancellationRequested();

                        if (((dynamic)data).Id != null)
                        {
                            string Id = ((dynamic)data).Id;
                            RegionViewModel DataToReset = await IRegion.GetById(x => x.Id == Id);

                            Regions.ForEach(x =>
                            {
                                if (x.Id == ((dynamic)data).Id)
                                {
                                    x.Id = DataToReset.Id;
                                    x.Name = DataToReset.Name;
                                    x.DateCreated = DataToReset.DateCreated;
                                    x.IsActive = DataToReset.IsActive;
                                    x.Abbr = DataToReset.Abbr;
                                }
                            });
                        }
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
                        cts.Token.ThrowIfCancellationRequested();

                        if (((dynamic)data).Id != null)
                        {
                            string Id = ((dynamic)data).Id;
                            AntennaMakeModel DataToReset = await IAntennaMake.GetById(x => x.Id == Id);

                            AntennaMakes.ForEach(x =>
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
                ["AntennaTypeModel"] = async (data) =>
                {
                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();

                        if (((dynamic)data).Id != null)
                        {
                            string Id = ((dynamic)data).Id;
                            AntennaTypeModel DataToReset = await IAntennaType.GetById(x => x.Id == Id);

                            AntennaTypes.ForEach(x =>
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
                ["SummerConfigModel"] = async (data) =>
                {
                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();

                        if (((dynamic)data).Id != null)
                        {
                            string Id = ((dynamic)data).Id;
                            SummerConfigModel DataToReset = await ISummerConfig.GetById(x => x.Id == Id);

                            SummerConfigs.ForEach(x =>
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
                ["ProjectTypeModel"] = async (data) =>
                {
                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();

                        if (((dynamic)data).Id != null)
                        {
                            string Id = ((dynamic)data).Id;
                            ProjectTypeModel DataToReset = await IProjectType.GetById(x => x.Id == Id);

                            ProjectTypes.ForEach(x =>
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
                ["TechTypeModel"] = async (data) =>
                {
                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();

                        if (((dynamic)data).Id != null)
                        {
                            string Id = ((dynamic)data).Id;
                            TechTypeModel DataToReset = await ITechType.GetById(x => x.Id == Id);

                            TechTypes.ForEach(x =>
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
                ["BaseBandModel"] = async (data) =>
                {
                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();

                        if (((dynamic)data).Id != null)
                        {
                            string Id = ((dynamic)data).Id;
                            BaseBandModel DataToReset = await IBaseBand.GetById(x => x.Id == Id);

                            BaseBands.ForEach(x =>
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
            };

            await Task.Run(() => processor[model](data));
        }

        private async Task<bool> DoDeleteFromGrid(double Id, string model)
        {
            Dictionary<string, Func<double, bool>> processor = new()
            {
                ["RRUTypeModel"] = (Id) =>
                {
                    try
                    {
                        Grid_RRUType.DeleteRow(Id);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
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

        private async Task<bool> DoGridUpdate<T>(double Id, T data, string model)
        {
            Dictionary<string, Func<T, bool>> processor = new()
            {
                ["RRUTypeModel"] = (data) =>
                {
                    try
                    {
                        Grid_RRUType.UpdateRow(Id, data as RRUTypeModel);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["SpectrumViewModel"] = (data) =>
                {
                    try
                    {
                        Grid_Spectrum.UpdateRow(Id, data as SpectrumViewModel);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["RegionViewModel"] = (data) =>
                {
                    try
                    {
                        Grid_Region.UpdateRow(Id, data as RegionViewModel);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["AntennaMakeModel"] = (data) =>
                {
                    try
                    {
                        Grid_AntennaMake.UpdateRow(Id, data as AntennaMakeModel);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["AntennaTypeModel"] = (data) =>
                {
                    try
                    {
                        Grid_AntennaType.UpdateRow(Id, data as AntennaTypeModel);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["SummerConfigModel"] = (data) =>
                {
                    try
                    {
                        Grid_SummerConfig.UpdateRow(Id, data as SummerConfigModel);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["ProjectTypeModel"] = (data) =>
                {
                    try
                    {
                        Grid_ProjectType.UpdateRow(Id, data as ProjectTypeModel);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["TechTypeModel"] = (data) =>
                {
                    try
                    {
                        Grid_TechType.UpdateRow(Id, data as TechTypeModel);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["BaseBandModel"] = (data) =>
                {
                    try
                    {
                        Grid_BaseBand.UpdateRow(Id, data as BaseBandModel);
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
                ["RRUTypeModel"] = (data) =>
                {
                    try
                    {
                        //Grid_RRUType.AddRecord(data as RRUTypeModel);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["SpectrumViewModel"] = (data) =>
                {
                    try
                    {
                        //Grid_Spectrum.AddRecord(data as SpectrumViewModel);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["RegionViewModel"] = (data) =>
                {
                    try
                    {
                        //Grid_Project.AddRecord(data as RegionViewModel);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["AntennaMakeModel"] = (data) =>
                {
                    try
                    {
                        //Grid_Project.AddRecord(data as AntennaMakeModel);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["AntennaTypeModel"] = (data) =>
                {
                    try
                    {
                        //Grid_Project.AddRecord(data as AntennaTypeModel);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["SummerConfigModel"] = (data) =>
                {
                    try
                    {
                        //Grid_Project.AddRecord(data as SummerConfigModel);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["ProjectTypeModel"] = (data) =>
                {
                    try
                    {
                        //Grid_Project.AddRecord(data as ProjectTypeModel);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["TechTypeModel"] = (data) =>
                {
                    try
                    {
                        //Grid_Tech.AddRecord(data as TechTypeModel);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ["BaseBandModel"] = (data) =>
                {
                    try
                    {
                        //Grid_Project.AddRecord(data as BaseBandModel);
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
                ["RRUTypeModel"] = async (data) =>
                {
                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();

                        RRUTypeModel result = await IRRUType.Update(data as RRUTypeModel, x => x.Id == Id);
                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["SpectrumViewModel"] = async (data) =>
                {
                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();

                        SpectrumViewModel result = await ISpectrum.Update(data as SpectrumViewModel, x => x.Id == Id);
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
                        cts.Token.ThrowIfCancellationRequested();

                        RegionViewModel result = await IRegion.Update(data as RegionViewModel, x => x.Id == Id);
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
                        cts.Token.ThrowIfCancellationRequested();

                        AntennaMakeModel result = await IAntennaMake.Update(data as AntennaMakeModel, x => x.Id == Id);
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
                        cts.Token.ThrowIfCancellationRequested();

                        AntennaTypeModel result = await IAntennaType.Update(data as AntennaTypeModel, x => x.Id == Id);
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
                        cts.Token.ThrowIfCancellationRequested();

                        SummerConfigModel result = await ISummerConfig.Update(data as SummerConfigModel, x => x.Id == Id);
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
                        cts.Token.ThrowIfCancellationRequested();

                        ProjectTypeModel result = await IProjectType.Update(data as ProjectTypeModel, x => x.Id == Id);
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
                        cts.Token.ThrowIfCancellationRequested();

                        TechTypeModel result = await ITechType.Update(data as TechTypeModel, x => x.Id == Id);
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
                        cts.Token.ThrowIfCancellationRequested();

                        BaseBandModel result = await IBaseBand.Update(data as BaseBandModel, x => x.Id == Id);
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
                ["RRUTypeModel"] = async (data) =>
                {
                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();

                        string spectrumId = (data as RRUTypeModel).Id;
                        RRUTypeModel result = await IRRUType.Delete(data as RRUTypeModel, x => x.Id == spectrumId);

                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["SpectrumViewModel"] = async (data) =>
                {
                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();

                        string spectrumId = (data as SpectrumViewModel).Id;
                        SpectrumViewModel result = await ISpectrum.Delete(data as SpectrumViewModel, x => x.Id == spectrumId);

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
                        cts.Token.ThrowIfCancellationRequested();

                        string regionId = (data as RegionViewModel).Id;
                        RegionViewModel result = await IRegion.Delete(data as RegionViewModel, x => x.Id == regionId);

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
                        cts.Token.ThrowIfCancellationRequested();

                        string antennaId = (data as AntennaMakeModel).Id;
                        AntennaMakeModel result = await IAntennaMake.Delete(data as AntennaMakeModel, x => x.Id == antennaId);

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
                        cts.Token.ThrowIfCancellationRequested();

                        string antennaId = (data as AntennaTypeModel).Id;
                        AntennaTypeModel result = await IAntennaType.Delete(data as AntennaTypeModel, x => x.Id == antennaId);

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
                        cts.Token.ThrowIfCancellationRequested();

                        string configId = (data as SummerConfigModel).Id;
                        SummerConfigModel result = await ISummerConfig.Delete(data as SummerConfigModel, x => x.Id == configId);

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
                        cts.Token.ThrowIfCancellationRequested();

                        string projectId = (data as ProjectTypeModel).Id;
                        ProjectTypeModel result = await IProjectType.Delete(data as ProjectTypeModel, x => x.Id == projectId);

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
                        cts.Token.ThrowIfCancellationRequested();

                        string techId = (data as TechTypeModel).Id;
                        TechTypeModel result = await ITechType.Delete(data as TechTypeModel, x => x.Id == techId);

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
                        cts.Token.ThrowIfCancellationRequested();

                        string basebandId = (data as BaseBandModel).Id;
                        BaseBandModel result = await IBaseBand.Delete(data as BaseBandModel, x => x.Id == basebandId);

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
                ["RRUTypeModel"] = async (data) =>
                {
                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();
                        RRUTypeModel result = await IRRUType.Create(data as RRUTypeModel);
                        return result as T;
                    }
                    catch
                    {
                        return null;
                    }
                },
                ["SpectrumViewModel"] = async (data) =>
                {
                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();
                        SpectrumViewModel result = await ISpectrum.Create(data as SpectrumViewModel);
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
                        cts.Token.ThrowIfCancellationRequested();
                        RegionViewModel result = await IRegion.Create(data as RegionViewModel);
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
                        cts.Token.ThrowIfCancellationRequested();
                        AntennaMakeModel result = await IAntennaMake.Create(data as AntennaMakeModel);
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
                        cts.Token.ThrowIfCancellationRequested();
                        AntennaTypeModel result = await IAntennaType.Create(data as AntennaTypeModel);
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
                        cts.Token.ThrowIfCancellationRequested();
                        SummerConfigModel result = await ISummerConfig.Create(data as SummerConfigModel);
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
                        cts.Token.ThrowIfCancellationRequested();
                        ProjectTypeModel result = await IProjectType.Create(data as ProjectTypeModel);
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
                        cts.Token.ThrowIfCancellationRequested();
                        TechTypeModel result = await ITechType.Create(data as TechTypeModel);
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
                        cts.Token.ThrowIfCancellationRequested();
                        BaseBandModel result = await IBaseBand.Create(data as BaseBandModel);
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

                    Vendors = await IVendor.Get();
                    Regions = await IRegion.Get();
                    AntennaTypes = await IAntennaType.Get();
                    AntennaMakes = await IAntennaMake.Get();
                    ProjectTypes = await IProjectType.Get();
                    RRUTypes = await IRRUType.Get();
                    TechTypes = await ITechType.Get();
                    BaseBands = await IBaseBand.Get();
                    SummerConfigs = await ISummerConfig.Get();
                    Spectrums = await ISpectrum.Get();
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error loading setup data", new { }, ex);
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
            Logger.LogInformation("Executing FormSetup Disposal", new { });

            cts.Cancel();
            cts.Dispose();
            //IRegion?.Dispose();
            //IAntennaType?.Dispose();
            //IProjectType?.Dispose();
            //IBaseBand?.Dispose();
            //ISummerConfig?.Dispose();
            //ISpectrum?.Dispose();

            GC.SuppressFinalize(this);

            Logger.LogInformation("Manage Access Disposed", new { });
        }
    }
}
