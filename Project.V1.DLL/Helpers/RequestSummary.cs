﻿using Project.V1.DLL.Interface;
using Project.V1.DLL.Services.Interfaces.FormSetup;

namespace Project.V1.DLL.Helpers
{
    public record AcceptanceDTO
    {
        public string SiteId { get; set; }
        public string Region { get; set; }
        public string State { get; set; }
        public string Vendor { get; set; }
        public string TechType { get; set; }
        public string Spectrum { get; set; }
        public int AcceptanceCount { get; set; }
        public int UMTSPhyCount { get; set; }
        public int LTEPhyCount { get; set; }
        public string ProjectType { get; set; }
        public string ProjectTypeName { get; set; }
        public DateTimeOffset DateSubmitted { get; set; }
        public DateTimeOffset? DateAccepted { get; set; }
        public string EngineerComment { get; set; }
        public string Status { get; set; }
    }

    public static class RequestSummary
    {
        private static IProjectType _projectType;
        private static IVendor _vendor;
        private static IRequest _request;

        public static IProjectType ProjectType
        {
            get
            {
                return _projectType;
            }
        }

        public static IVendor Vendor
        {
            get
            {
                return _vendor;
            }
        }

        public static IRequest Request
        {
            get
            {
                return _request;
            }
        }

        public static void Initialize(IProjectType projectType, IVendor vendor, IRequest request)
        {
            _projectType = projectType;
            _vendor = vendor;
            _request = request;
        }

        public static string GetProjectTypeName(string projectType)
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
                var pType when pType.StartsWith("RT ") && pType.ToUpper() != "RT DONOR" => "Rural Telephony",
                _ => projectType
            };
        }

        public static string GetSpectrumName(string spectrum)
        {
            return spectrum.ToUpper().Trim() switch
            {
                "2G RT" => "2G",
                "G900" => "2G",
                "G1800" => "2G",
                "G900+G1800" => "2G",
                "3G RT" => "3G",
                "U2100" => "3G",
                "L700" => "700M",
                "L800" => "800M",
                "L1800" => "1800M",
                "L2600" => "2600M",
                "N3500" => "N3500",
                _ => spectrum
            };
        }

        public static List<AcceptanceDTO> GetProjectTypeRequests(DateTime MinDateTime, DateTime MaxDateTime)
        {
            var requests = new List<AcceptanceDTO>();

            var projectTypes = (ProjectType.Get(x =>
                                                    x.IsActive && x.Name != "Layer Expansion" &&
                                                    x.Name != "Small Cell" &&
                                                    !x.Name.Contains("MOD") &&
                                                    !x.Name.ToUpper().Contains(" UPGRADE")
                                ).GetAwaiter().GetResult()).Select(x => GetProjectTypeName(x.Name)).ToList();

            projectTypes = projectTypes.Distinct().ToList();

            foreach (var projectType in projectTypes)
            {
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "2G", ProjectType = projectType });
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "L900 RT", ProjectType = projectType });
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "3G", ProjectType = projectType });
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "U900", ProjectType = projectType });
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "Multi Sector", ProjectType = projectType });
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "700M", ProjectType = projectType });
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "800M", ProjectType = projectType });
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "1800M", ProjectType = projectType });
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "2600M", ProjectType = projectType });
                requests.Add(new AcceptanceDTO { UMTSPhyCount = 0, Spectrum = "3G PHY", ProjectType = projectType });
                requests.Add(new AcceptanceDTO { LTEPhyCount = 0, Spectrum = "4G PHY", ProjectType = projectType });
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "N3500", ProjectType = projectType });
            }

            requests.AddRange(Request.Get(x => !string.IsNullOrEmpty(x.EngineerAssigned.Fullname.Trim())
                                && x.EngineerAssigned.DateApproved.Date >= MinDateTime && x.EngineerAssigned.DateApproved.Date < MaxDateTime
                                && x.Requester.Vendor.ShouldSummerize
                                && !x.Spectrum.Name.Contains("MOD") && !x.ProjectType.Name.Contains("MOD") && x.Status == "Accepted"
                                && x.ProjectType.Name != "Layer Expansion" && x.ProjectType.Name != "Small Cell", null, "EngineerAssigned,Region,Requester.Vendor,ProjectType,TechType,Spectrum").GetAwaiter().GetResult()
                                .Select(x => new AcceptanceDTO
                                {
                                    Region = x.Region.Name,
                                    Vendor = x.Requester.Vendor.Name,
                                    ProjectType = GetProjectTypeName(x.ProjectType.Name),
                                    ProjectTypeName = x.ProjectType.Name,
                                    TechType = x.TechType.Name,
                                    Spectrum = GetSpectrumName(x.Spectrum.Name),
                                    AcceptanceCount = 1,
                                    UMTSPhyCount = GetPhysicalTechCount("3G", x.Spectrum.Name),
                                    LTEPhyCount = (x.ProjectType.Name.ToUpper().Equals("RT DONOR") == false) ? GetPhysicalTechCount("4G", x.Spectrum.Name) : 0,
                                }).ToList());

            return requests;
        }

        public static List<AcceptanceDTO> GetVendorRequests(DateTime MinDateTime, DateTime MaxDateTime)
        {
            var requests = new List<AcceptanceDTO>();
            var vendors = Vendor.Get(x => x.IsActive && x.ShouldSummerize).GetAwaiter().GetResult();

            foreach (var vendor in vendors)
            {
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "2G", Vendor = vendor.Name });
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "L900 RT", Vendor = vendor.Name });
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "3G", Vendor = vendor.Name });
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "U900", Vendor = vendor.Name });
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "Multi Sector", Vendor = vendor.Name });
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "700M", Vendor = vendor.Name });
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "800M", Vendor = vendor.Name });
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "1800M", Vendor = vendor.Name });
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "2600M", Vendor = vendor.Name });
                requests.Add(new AcceptanceDTO { UMTSPhyCount = 0, Spectrum = "3G PHY", Vendor = vendor.Name });
                requests.Add(new AcceptanceDTO { LTEPhyCount = 0, Spectrum = "4G PHY", Vendor = vendor.Name });
                requests.Add(new AcceptanceDTO { AcceptanceCount = 0, Spectrum = "N3500", Vendor = vendor.Name });
            }

            requests.AddRange(Request.Get(x => !string.IsNullOrEmpty(x.EngineerAssigned.Fullname.Trim())
                                && x.EngineerAssigned.DateApproved.Date >= MinDateTime && x.EngineerAssigned.DateApproved.Date < MaxDateTime
                                && x.Requester.Vendor.ShouldSummerize
                                //&& !x.Spectrum.Name.Contains("RT")
                                && !x.Spectrum.Name.Contains("MOD") && !x.ProjectType.Name.Contains("MOD") && x.Status == "Accepted"
                                && x.ProjectType.Name != "Layer Expansion" && x.ProjectType.Name != "Small Cell"
                                , null, "EngineerAssigned,Region,Requester.Vendor,ProjectType,TechType,Spectrum").GetAwaiter().GetResult()
                                .Select(x => new AcceptanceDTO
                                {
                                    Region = x.Region.Name,
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

        public static int GetPhysicalTechCount(string techType, string spectrum)
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
    }
}
