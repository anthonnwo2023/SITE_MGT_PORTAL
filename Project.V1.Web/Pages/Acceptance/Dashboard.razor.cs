﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Project.V1.DLL.Interface;
using Project.V1.DLL.Services.Interfaces;
using Project.V1.DLL.Services.Interfaces.FormSetup;
using Project.V1.Lib.Interfaces;
using Project.V1.Models;
using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.PivotView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.V1.Web.Pages.Acceptance
{
    public partial class Dashboard
    {
        public List<PathInfo> Paths { get; set; }
        [Inject] public IHttpContextAccessor Context { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] protected IRequest IRequest { get; set; }
        [Inject] protected IVendor IVendor { get; set; }
        [Inject] protected IProjectType IProjectType { get; set; }

        public List<RequestViewModel> VendorRequests { get; set; }
        public List<AcceptanceDTO> DailyRequests { get; set; }
        public List<AcceptanceDTO> MonthlyProjectTypeRequests { get; set; }
        public List<AcceptanceDTO> MonthlyRequests { get; set; }

        public DateTime DateData { get; set; } = DateTime.Now;
        public DateTime PrevDate { get; set; } = DateTime.MinValue;
        public DateTime MinDateTime { get; set; }
        public DateTime MaxDateTime { get; set; }
        public bool DateIsToday { get; set; } = true;
        public bool DateWthMth { get; set; } = true;

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        //public List<string> ToolbarItems = new() { "Search" };

        public string total3GPS = "\"(" + "\"" + "Spectrum" + "\"" + "==" + "\"" + "3G" + "\"" + "||" + "\"" + "Spectrum" + "\"" + "==" + "\"" + "U900" + "\"" + "||" + "\"" + "Spectrum" + "\"" + "==" + "\"" + "Multi Sector" + "\"" + ") ? 1 : 0";
        public string total4GPS = "\"(" + "\"" + "Spectrum" + "\"" + "==" + "\"" + "L700" + "\"" + "||" + "\"" + "Spectrum" + "\"" + "==" + "\"" + "L800" + "\"" + "||" + "\"" + "Spectrum" + "\"" + "==" + "\"" + "L1800" + "||" + "\"" + "Spectrum" + "\"" + "==" + "\"" + "L2600" + "\"" + ") ? 1 : 0";

        public SfPivotView<AcceptanceDTO> pivot;

        public record AcceptanceDTO
        {
            public string Vendor { get; set; }
            public string TechType { get; set; }
            public string Spectrum { get; set; }
            public int AcceptanceCount { get; set; }
            public int UMTSPhyCount { get; set; }
            public int LTEPhyCount { get; set; }
            public string ProjectType { get; set; }
        }

        public List<ToolbarItems> toolbar = new()
        {
            ToolbarItems.New,
            ToolbarItems.Load,
            ToolbarItems.Remove,
            ToolbarItems.Rename,
            ToolbarItems.SaveAs,
            ToolbarItems.Save,
            ToolbarItems.Grid,
            ToolbarItems.Chart,
            ToolbarItems.Export,
            ToolbarItems.SubTotal,
            ToolbarItems.GrandTotal,
            ToolbarItems.ConditionalFormatting,
            ToolbarItems.FieldList
        };


        protected override void OnInitialized()
        {
            Paths = new()
            {
                new PathInfo { Name = $"Acceptance", Link = "acceptance" },
            };
        }

        private static string GetProjectTypeName(string projectType)
        {
            return projectType.ToUpper() switch
            {
                "BTS" => "B-T-S Accepted",
                "REACTIVATION" => "Reactivation/Mobile/DAS/BURNT Accepted",
                "BURNT SITE REACTIVATION" => "Reactivation/Mobile/DAS/BURNT Accepted",
                "COW" => "Reactivation/Mobile/DAS/BURNT Accepted",
                "RELOCATION" => "Reactivation/Mobile/DAS/BURNT Accepted",
                "DAS INDOOR" => "Reactivation/Mobile/DAS/BURNT Accepted",
                "DAS OUTDOOR" => "Reactivation/Mobile/DAS/BURNT Accepted",
                "COLO" => "COLO Accepted",
                //"RT LEGACY" => "Rural Telephony",
                //"RT REVENUE SHARE" => "Rural Telephony",
                //"RT CAPEX" => "Rural Telephony",
                "UPGRADE" => "UPGRADE Accepted",
                var pType when pType.Contains("UPGRADE") => "UPGRADE Accepted",
                var pType when pType.StartsWith("RT ") => "Rural Telephony",
                _ => projectType
            };
        }

        private static string GetSpectrumName(string spectrum)
        {
            return spectrum.ToUpper() switch
            {
                "2G RT" => "2G",
                "G900" => "2G",
                "G1800" => "2G",
                "3G RT" => "3G",
                "U2100" => "3G",
                "L700" => "700M",
                "L800" => "800M",
                "L1800" => "1800M",
                "L2600" => "2600M",
                _ => spectrum
            };
        }

        protected async Task SetMyDate(ChangedEventArgs<DateTime> value)
        {
            DateData = value.Value;

            DailyRequests = await GetVendorRequests("Day");

            int lastDayOfMth = DateTime.DaysInMonth(DateData.Year, DateData.Month);

            MinDateTime = new DateTime(DateData.Year, DateData.Month, 1).Date;
            MaxDateTime = new DateTime(DateData.Year, DateData.Month, lastDayOfMth).AddDays(1).Date;

            var shouldNotReload = PrevDate.Date >= MinDateTime && PrevDate.Date < MaxDateTime;

            if (!shouldNotReload)
            {
                MonthlyProjectTypeRequests = await GetProjectTypeRequests("Month");
                MonthlyRequests = await GetVendorRequests("Month");

                PrevDate = DateData;
            }
        }

        private async Task<List<AcceptanceDTO>> GetProjectTypeRequests(string requestType)
        {
            var requests = new List<AcceptanceDTO>();

            var projectTypes = (await IProjectType.Get(x => x.IsActive && x.Name != "Layer Expansion" && x.Name != "Small Cell")).Select(x => GetProjectTypeName(x.Name)).ToList();

            projectTypes = projectTypes.Distinct().ToList();

            foreach (var projectType in projectTypes)
            {
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "2G", ProjectType = projectType });
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "3G", ProjectType = projectType });
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "U900", ProjectType = projectType });
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "Multi Sector", ProjectType = projectType });
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "700M", ProjectType = projectType });
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "800M", ProjectType = projectType });
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "1800M", ProjectType = projectType });
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "2600M", ProjectType = projectType });
                requests.Add(new AcceptanceDTO { UMTSPhyCount = 0, Spectrum = "3G PHY", ProjectType = projectType });
                requests.Add(new AcceptanceDTO { LTEPhyCount = 0, Spectrum = "4G PHY", ProjectType = projectType });
            }

            int firstDatOfMth = 1;
            int lastDayOfMth = DateTime.DaysInMonth(DateData.Year, DateData.Month);

            if (requestType == "Day")
            {
                lastDayOfMth = DateData.Day;
                firstDatOfMth = lastDayOfMth;
                DateIsToday = DateData.Date == DateTime.Now.Date;
            }

            var MinDateTime = new DateTime(DateData.Year, DateData.Month, firstDatOfMth).Date;
            var MaxDateTime = new DateTime(DateData.Year, DateData.Month, lastDayOfMth).AddDays(1).Date;

            DateWthMth = DateData.Date >= MinDateTime && DateData.Date < MaxDateTime;

            requests.AddRange((await IRequest.Get(x => !string.IsNullOrEmpty(x.EngineerAssigned.Fullname.Trim())
                                && x.EngineerAssigned.DateApproved.Date >= MinDateTime && x.EngineerAssigned.DateApproved.Date < MaxDateTime
                                && !x.Spectrum.Name.Contains("RRU")
                                && x.ProjectType.Name != "Layer Expansion" && x.ProjectType.Name != "Small Cell"))
                                .Select(x => new AcceptanceDTO
                                {
                                    Vendor = x.Requester.Vendor.Name,
                                    ProjectType = GetProjectTypeName(x.ProjectType.Name),
                                    TechType = x.TechType.Name,
                                    Spectrum = GetSpectrumName(x.Spectrum.Name),
                                    AcceptanceCount = 1,
                                    UMTSPhyCount = GetPhysicalTechCount("3G", x.Spectrum.Name),
                                    LTEPhyCount = GetPhysicalTechCount("4G", x.Spectrum.Name),
                                }).ToList());

            return requests;
        }

        private async Task<List<AcceptanceDTO>> GetVendorRequests(string requestType)
        {
            var requests = new List<AcceptanceDTO>();
            var vendors = await IVendor.Get(x => x.IsActive && x.Name != "MTN Nigeria");

            foreach (var vendor in vendors)
            {
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "2G", Vendor = vendor.Name });
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "3G", Vendor = vendor.Name });
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "U900", Vendor = vendor.Name });
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "Multi Sector", Vendor = vendor.Name });
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "700M", Vendor = vendor.Name });
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "800M", Vendor = vendor.Name });
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "1800M", Vendor = vendor.Name });
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "2600M", Vendor = vendor.Name });
                requests.Add(new AcceptanceDTO { UMTSPhyCount = 0, Spectrum = "3G PHY", Vendor = vendor.Name });
                requests.Add(new AcceptanceDTO { LTEPhyCount = 0, Spectrum = "4G PHY", Vendor = vendor.Name });
            }

            int firstDatOfMth = 1;
            int lastDayOfMth = DateTime.DaysInMonth(DateData.Year, DateData.Month);

            if (requestType == "Day")
            {
                lastDayOfMth = DateData.Day;
                firstDatOfMth = lastDayOfMth;
                DateIsToday = DateData.Date == DateTime.Now.Date;
            }

            MinDateTime = new DateTime(DateData.Year, DateData.Month, firstDatOfMth).Date;
            MaxDateTime = new DateTime(DateData.Year, DateData.Month, lastDayOfMth).AddDays(1).Date;

            DateWthMth = DateData.Date >= MinDateTime && DateData.Date < MaxDateTime;

            await GetRequests(MinDateTime, MaxDateTime);

            requests.AddRange(VendorRequests.Select(x => new AcceptanceDTO
            {
                Vendor = x.Requester.Vendor.Name,
                ProjectType = GetProjectTypeName(x.ProjectType.Name),
                TechType = x.TechType.Name,
                Spectrum = GetSpectrumName(x.Spectrum.Name),
                AcceptanceCount = 1,
                UMTSPhyCount = GetPhysicalTechCount("3G", x.Spectrum.Name),
                LTEPhyCount = GetPhysicalTechCount("4G", x.Spectrum.Name),
            }).ToList());

            return requests;
        }

        private static int GetPhysicalTechCount(string techType, string spectrum)
        {
            if (techType == "3G")
                return spectrum switch
                {
                    ("U2100" or "U900" or "Multi Site") => 1,
                    _ => 0
                };

            if (techType == "4G")
                return spectrum switch
                {
                    ("L700" or "L800" or "L1800" or "L2600") => 1,
                    _ => 0
                };

            return 0;
        }

        private async Task GetRequests(DateTime MinDateTime, DateTime MaxDateTime)
        {
            VendorRequests = await IRequest.Get(x => !string.IsNullOrEmpty(x.EngineerAssigned.Fullname.Trim())
                                && x.EngineerAssigned.DateApproved.Date >= MinDateTime && x.EngineerAssigned.DateApproved.Date < MaxDateTime
                                //&& !x.Spectrum.Name.Contains("RT")
                                && !x.Spectrum.Name.Contains("RRU")
                                );
        }

        protected async Task AuthenticationCheck(bool isAuthenticated)
        {
            if (isAuthenticated)
            {
                try
                {
                    //    Principal = (await AuthenticationStateTask).User;
                    //    User = await IUser.GetUserByUsername(Principal.Identity.Name);
                    //    Vendor = await IVendor.GetById(x => x.Id == User.VendorId);

                    //Requests = (await IRequest.Get(x => !string.IsNullOrEmpty(x.EngineerAssigned.Fullname.Trim())
                    //            && x.EngineerAssigned.DateApproved.Date.ToString("MM-yyyy") == DateData.Date.ToString("MM-yyyy")))
                    //    .OrderByDescending(x => x.DateCreated).ToList();

                    //EnableSpecial = (dt.Day == DateTime.DaysInMonth(dt.Year, dt.Month));

                    MonthlyRequests = await GetVendorRequests("Month");
                    DailyRequests = await GetVendorRequests("Day");
                    MonthlyProjectTypeRequests = await GetProjectTypeRequests("Month");

                    //TechTypes = await ITechType.Get(x => x.IsActive);
                    //Regions = await IRegion.Get(x => x.IsActive);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error loading rejected requests", new { }, ex);
                }
            }
        }
    }
}
