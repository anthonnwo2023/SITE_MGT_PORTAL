﻿using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Project.V1.DLL.Interface;
using Project.V1.DLL.Services.Interfaces.FormSetup;
using Project.V1.Lib.Interfaces;
using Project.V1.Models;
using Syncfusion.Blazor.Grids;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Linq;
using Project.V1.DLL.Services.Interfaces;
using Project.V1.Lib.Services;
using Project.V1.Web.Shared;
using Project.V1.Lib.Extensions;

namespace Project.V1.Web.Pages.Acceptance
{
    public partial class ReportTechPerSite
    {
        public List<PathInfo> Paths { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }
        [Inject] protected NavigationManager NavMan { get; set; }
        [Inject] public IHttpContextAccessor Context { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] protected IRequest IRequest { get; set; }
        [Inject] protected IRegion IRegion { get; set; }
        [Inject] protected IUser IUser { get; set; }
        [Inject] protected IVendor IVendor { get; set; }
        [Inject] protected ITechType ITechType { get; set; }
        [Inject] protected ISpectrum ISpectrum { get; set; }
        [Inject] protected IAntennaMake IAntennaMake { get; set; }
        [Inject] protected IAntennaType IAntennaType { get; set; }
        [Inject] protected IBaseBand IBaseband { get; set; }
        [Inject] protected IRRUType IRRUType { get; set; }
        [Inject] protected IProjectType IProjectType { get; set; }
        [Inject] protected ISummerConfig ISummerConfig { get; set; }

        List<RequestViewModel> Requests { get; set; }
        List<RequestViewModel> RequestsGroup { get; set; }
        List<RegionViewModel> Regions { get; set; }
        List<TechTypeModel> TechTypes { get; set; }
        List<SpectrumViewModel> Spectrums { get; set; }
        List<AntennaMakeModel> AntennaMakes { get; set; }
        List<AntennaTypeModel> AntennaTypes { get; set; }
        List<BaseBandModel> Basebands { get; set; }
        List<RRUTypeModel> RRUTypes { get; set; }
        List<ProjectTypeModel> ProjectTypes { get; set; }
        List<SummerConfigModel> SummerConfigs { get; set; }
        public ClaimsPrincipal Principal { get; set; }
        public ApplicationUser User { get; set; }
        public VendorModel Vendor { get; set; }

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }
        protected SfGrid<RequestViewModel> Grid_Request { get; set; }
        protected SfGrid<RequestViewModel> Grid_RequestGroup { get; set; }

        public List<string> ToolbarItems = new() { "Search", "ExcelExport", "ColumnChooser" };

        protected override void OnInitialized()
        {
            Paths = new()
            {
                new PathInfo { Name = $"Technology Per Site Report", Link = "acceptance/reports/site-technology" },
                new PathInfo { Name = $"Acceptance", Link = "acceptance" },
            };
        }

        public async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "ReportGroupTable_excelexport") //Id is combination of Grid's ID and itemname
            {
                //var hiddenCols = new string[] {
                //    "RNC/BSC",
                //    "Spectrum",
                //    "Bandwidth",
                //    "Latitude",
                //    "Longitude",
                //    "Antenna Make",
                //    "Antenna Type",
                //    "Antenna Height",
                //    "M Tilt",
                //    "E Tilt",
                //    "Baseband",
                //    "RRU Type",
                //    "Power - (w)",
                //    "Project Type",
                //    "Project Year",
                //    "Summer Config",
                //    "Software Version",
                //    "RRU Power - (w)",
                //    "CSFB Status GSM",
                //    "CSFB Status WCDMA",
                //    "RET Configured",
                //    "Carrier Aggregation",
                //    "Date Integrated",
                //    "Date Submitted",
                //};

                //await Grid_RequestGroup.ShowColumnsAsync(hiddenCols);
                await Grid_RequestGroup.ExcelExport();
                //await Grid_RequestGroup.HideColumnsAsync(hiddenCols);
            }
        }
        protected async Task AuthenticationCheck(bool isAuthenticated)
        {
            if (isAuthenticated)
            {
                try
                {
                    Principal = (await AuthenticationStateTask).User;
                    User = await IUser.GetUserByUsername(Principal.Identity.Name);
                    Vendor = await IVendor.GetById(x => x.Id == User.VendorId);

                    if (!await UserAuth.IsAutorizedForAsync("Can:ViewReportTPS") || Vendor.Name != "MTN Nigeria")
                    {
                        NavMan.NavigateTo("access-denied");
                        return;
                    }

                    //TechTypes = await ITechType.Get(x => x.IsActive);
                    //Regions = await IRegion.Get(x => x.IsActive);
                    //Spectrums = await ISpectrum.Get(x => x.IsActive);
                    //AntennaMakes = await IAntennaMake.Get(x => x.IsActive);
                    //AntennaTypes = await IAntennaType.Get(x => x.IsActive);
                    //Basebands = await IBaseband.Get(x => x.IsActive);
                    //RRUTypes = await IRRUType.Get(x => x.IsActive);
                    //ProjectTypes = await IProjectType.Get(x => x.IsActive);
                    //SummerConfigs = await ISummerConfig.Get(x => x.IsActive);

                    RequestsGroup = (await IRequest.Get(null, x => x.OrderByDescending(y => y.DateCreated))).GroupBy(x => x.SiteId)
                        .Select(x => new RequestViewModel
                        {
                            SiteId = x.Key,
                            SiteName = x.Select(x => x.SiteName).First(),
                            RegionId = x.Select(x => x.Region.Name).First(),
                            SpectrumId = string.Join(", ", x.Select(x => x.Spectrum.Name).Distinct()),
                            TechTypeId = string.Join(", ", x.Select(x => x.TechType.Name).Distinct()),
                            BasebandId = string.Join(", ", x.Select(y => y.Requester.Vendor.Name).Distinct()),
                        }).ToList();
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error loading rejected requests", new { }, ex);
                }
            }
        }
    }
}
