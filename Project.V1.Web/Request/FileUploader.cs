namespace Project.V1.Web.Request
{
    public class BoolDropDown
    {
        public string Name { get; set; }
    }

    public class NigerianState
    {
        public string Name { get; set; }
    }

    public static class FileUploader
    {
        private static List<AntennaMakeModel> AntennaMakes { get; set; }
        private static List<AntennaTypeModel> AntennaTypes { get; set; }
        private static List<ProjectModel> Projects { get; set; }
        private static List<ProjectTypeModel> ProjectTypes { get; set; }
        private static List<RegionViewModel> Regions { get; set; }
        private static List<SummerConfigModel> SummerConfigs { get; set; }
        private static List<SpectrumViewModel> Spectrums { get; set; }
        private static List<TechTypeModel> TechTypes { get; set; }
        private static ApplicationUser User { get; set; }
        private static ICLogger Logger { get; set; }

        private static readonly string[] States = new string[] {
            "Abia", "Adamawa", "Akwa Ibom", "Anambra", "Bauchi", "Bayelsa", "Benue", "Borno", "Cross River", "Delta", "Ebonyi", "Edo", "Ekiti",
            "Enugu", "FCT - Abuja", "Gombe", "Imo", "Jigawa", "Kaduna", "Kano", "Katsina", "Kebbi", "Kogi", "Kwara","Lagos", "Nasarawa", "Niger",
            "Ogun", "Ondo", "Osun", "Oyo", "Plateau", "Rivers", "Sokoto", "Taraba", "Yobe", "Zamfara"
        };

        private static List<BoolDropDown> BoolDrops { get; set; } = new()
        {
            new BoolDropDown { Name = "Yes" },
            new BoolDropDown { Name = "No" }
        };

        private static List<NigerianState> NigerianStates { get; set; } = States.Select(x => new NigerianState { Name = x }).ToList();

        public static void Initialize(IRequestListObject requestListObject, ICLogger logger)
        {
            AntennaMakes = requestListObject.AntennaMakes;
            AntennaTypes = requestListObject.AntennaTypes;
            Projects = requestListObject.Projects;
            ProjectTypes = requestListObject.ProjectTypes;
            Regions = requestListObject.Regions;
            SummerConfigs = requestListObject.SummerConfigs;
            Spectrums = requestListObject.Spectrums;
            TechTypes = requestListObject.TechTypes;
            User = requestListObject.User;
            Logger = logger;
        }

        public static async Task<(string uploadResp, string filePath, string uploadError)> StartUpload(bool allowedExtension, FilesManager file, bool toClose)
        {
            switch (allowedExtension)
            {
                case true:
                    {
                        var (error, path) = UploadFile(file, toClose);

                        if (path.Length > 0)
                        {
                            return ("", path, error);
                        }

                        await Task.CompletedTask;

                        return ($"An Error occurred, could not save specified file", "", error);
                    }

                default:
                    return ("Invalid file type. Upload only excel documents. (.xls and .xlsx only)", "", "");
            }
        }

        private static (string error, string path) UploadFile(FilesManager bufile, bool toClose)
        {
            try
            {
                bufile.UploadFile.Stream.WriteTo(bufile.Filestream);

                if (toClose)
                {
                    bufile.Filestream.Close();
                    bufile.UploadFile.Stream.Close();
                }

                return ("", bufile.UploadPath); ;
            }
            catch (Exception ex)
            {
                if (File.Exists(bufile.UploadPath))
                    File.Delete(bufile.UploadPath);

                return (ex.Message, "");
            }
        }


        public static async Task<(string error, List<RequestViewModel>)> ProcessUpload(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    ExcelRequestObj excelRequestObj = new()
                    {
                        Headers = new List<string>
                        {
                            "Technology", "Site Id", "Site Name", "RNC/BSC", "Region", "Spectrum", "Bandwidth (MHz)", "Latitude", "Longitude",
                            "Antenna Make", "Antenna Type", "Antenna Height", "Tower Height - (M)", "Antenna Azimuth", "M Tilt", "E Tilt", "Baseband", "RRU TYPE", "Power - (w)",
                            "Project Type", "Project Year", "Summer Config", "Software", "RRU Power - (w)", "CSFB Status GSM", "CSFB Status WCDMA",
                            "Integrated Date", "RET Configured", "Carrier Aggregation", "State", "Project Name", "Comment"
                        }
                    };

                    (DataTable dt, ExcelTransactionError error) = ExcelProcessor.ToDataTable(excelRequestObj, path, User.Fullname);

                    if (error.ErrorType.Length == 0)
                    {
                        var (errorRequest, validRequests) = await GetRequestsFromDataTable(dt, error);
                        List<RequestViewModel> requests = validRequests;
                        return (errorRequest, requests);
                    }

                    Logger.LogError($"{error.ErrorDesc} : {error.ErrorType}", new { Created = error.CreatedBy, error.DateCreated });

                    return (error.ErrorType, new List<RequestViewModel>());
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.Message, new { }, ex);
                    return (ex.Message, new List<RequestViewModel>());
                }

            }

            return ("File does not exist.", new List<RequestViewModel>());
        }

        private static async Task<(string errorRequest, List<RequestViewModel> validRequests)> GetRequestsFromDataTable(DataTable dt, ExcelTransactionError error)
        {
            List<RequestViewModel> requests = new();

            switch (error.ErrorType.Length)
            {
                case 0:
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            RequestViewModel request = ExcelProcessor.CreateItemFromRowMapper<RequestViewModel>(row);

                            (string column, bool valid) = ValidateRow(request);

                            if (valid == false)
                            {
                                var errorMsg = $"Invalid Excel Template uploaded. Error on row:  {dt.Rows.IndexOf(row) + 1}, column: {column}";
                                return (errorMsg, new());
                            }

                            request.AntennaMakeId = (request.AntennaMakeId != null) ? AntennaMakes.FirstOrDefault(x => x.Name.ToUpper().Trim() == request.AntennaMakeId.ToUpper())?.Id : request.AntennaMakeId;
                            request.AntennaTypeId = (request.AntennaTypeId != null) ? AntennaTypes.FirstOrDefault(x => x.Name.ToUpper().Trim() == request.AntennaTypeId.ToUpper())?.Id : request.AntennaTypeId;
                            request.RegionId = (request.RegionId != null) ? Regions.FirstOrDefault(x => x.Name.ToUpper().Trim() == request.RegionId.ToUpper())?.Id : request.RegionId;
                            request.SummerConfigId = (request.SummerConfigId != null) ? SummerConfigs.FirstOrDefault(x => x.Name.ToUpper().Trim() == request.SummerConfigId.ToUpper())?.Id : request.SummerConfigId;
                            request.TechTypeId = (request.TechTypeId != null) ? TechTypes.FirstOrDefault(x => x.Name.ToUpper().Trim() == request.TechTypeId.ToUpper())?.Id : request.TechTypeId;
                            request.SpectrumId = (request.SpectrumId != null) ? Spectrums.FirstOrDefault(x => x.Name.ToUpper().Trim() == request.SpectrumId.ToUpper() && x.TechTypeId == request.TechTypeId)?.Id : request.SpectrumId;
                            request.ProjectTypeId = (request.ProjectTypeId != null) ? ProjectTypes.FirstOrDefault(x => x.Name.ToUpper().Trim() == request.ProjectTypeId.ToUpper() && x.SpectrumId == request.SpectrumId)?.Id : request.ProjectTypeId;
                            request.ProjectNameId = (request.ProjectNameId != null) ? Projects.FirstOrDefault(x => x.Name.ToUpper().Trim() == request.ProjectNameId.ToUpper())?.Id : request.ProjectNameId;

                            requests.Add(request);
                        }

                        return (string.Empty, await Task.FromResult(requests));
                    }

                default:
                    return (error.ErrorDesc, requests);
            }
        }

        private static (string column, bool valid) ValidateRow(RequestViewModel request)
        {
            var (error, valid) = IsFKValid(x => x.Name.ToUpper().Trim() == request.TechTypeId.ToUpper(), TechTypes);
            if (!valid) return ("Technology", valid);
            (error, valid) = IsFKValid(x => x.Name.ToUpper().Trim() == request.RegionId.ToUpper(), Regions);
            if (!valid) return (error, valid);
            (error, valid) = IsFKValid(x => x.Name.ToUpper().Trim() == request.SpectrumId.ToUpper() && x.TechType.Name.ToUpper() == request.TechTypeId.ToUpper(), Spectrums);
            if (!valid) return (error, valid);
            (error, valid) = IsFKValid(x => x.Name.ToUpper().Trim() == request.State.ToUpper(), NigerianStates);
            if (!valid) return ("State", valid);
            (error, valid) = IsFKValid(x => x.Name.ToUpper().Trim() == request.ProjectNameId.ToUpper(), Projects);
            if (!valid) return ("Project Name", valid);
            (error, valid) = IsFKValid(x => x.Name.ToUpper().Trim() == request.ProjectTypeId.ToUpper() && x.Spectrum.Name.ToUpper() == request.SpectrumId.ToUpper(), ProjectTypes);
            if (!valid) return ("Project Type", valid);

            if (string.IsNullOrEmpty(request.SiteName))
            {
                error = "Site Name";
                valid = false;
            }

            if (string.IsNullOrEmpty(request.RRUType))
            {
                error = "RRU Type";
                valid = false;
            }
            if (!string.IsNullOrEmpty(request.SummerConfigId))
            {
                (error, valid) = IsFKValid(x => x.Name.ToUpper().Trim() == request.SummerConfigId.ToUpper(), SummerConfigs);
                if (!valid) return ("Summer Config", valid);
            }

            if (!string.IsNullOrEmpty(request.AntennaMakeId))
            {
                (error, valid) = IsFKValid(x => x.Name.ToUpper().Trim() == request.AntennaMakeId.ToUpper(), AntennaMakes);
                if (!valid) return ("Antenna Make", valid);
            }

            if (!string.IsNullOrEmpty(request.RETConfigured))
            {
                (error, valid) = IsFKValid(x => x.Name.ToUpper().Trim() == request.RETConfigured.ToUpper(), BoolDrops);
                if (!valid) return ("RET Configured", valid);
            }

            if (!string.IsNullOrEmpty(request.AntennaTypeId))
            {
                (error, valid) = IsFKValid(x => x.Name.ToUpper().Trim() == request.AntennaTypeId.ToUpper(), AntennaTypes);
                if (!valid) return ("Antenna Type", valid);
            }

            if (request.TechTypeId != TechTypes.FirstOrDefault(x => x.Name == "4G")?.Id)
            {
                if (!string.IsNullOrEmpty(request.CarrierAggregation))
                {
                    (error, valid) = IsFKValid(x => x.Name.ToUpper().Trim() == request.CarrierAggregation.ToUpper(), BoolDrops);
                    if (!valid) return ("Carrier Aggregation", valid);
                }
            }

            return (error, valid);
        }

        private static (string error, bool valid) IsFKValid<T>(Func<T, bool> whereExpression, List<T> collection) where T : class
        {
            var isFound = collection.Any(whereExpression);
            var error = string.Empty;

            if (!isFound)
            {
                error = typeof(T).Name.Replace("Model", "").Replace("View", "");
            }

            return (error, isFound);
        }
    }
}
