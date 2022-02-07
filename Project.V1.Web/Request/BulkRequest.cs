namespace Project.V1.Web.Request
{
    public class BulkRequest : RequestBase
    {
        private readonly IRequest _request;

        public BulkRequest(List<RequestViewModel> requests, List<FilesManager> ssvReports, IRequest request) :
            base(requests, ssvReports)
        {
            _request = request;
        }

        public override async Task<(string SaveStatus, List<string> Messages, List<RequestViewModel> ValidRequests)> Save(AcceptanceRequestObject requestObject)
        {
            User = requestObject.User;
            ProjectTypes = requestObject.ProjectTypes;
            Spectrums = requestObject.Spectrums;
            TechTypes = requestObject.TechTypes;
            Variables = new() { { "User", requestObject.User.UserName }, { "App", "acceptance" } };
            IsWaiver = requestObject.IsWaiver;

            var batchNumber = HelperFunctions.GenerateIDUnique("SA-BN");
            List<string> Errors = new();

            foreach (RequestViewModel request in Requests)
            {
                request.BulkuploadPath = Path.GetFileName(requestObject.BulkUploadPath);
                request.BulkBatchNumber = batchNumber;
                request.State = request.State.ToUpper();
                request.SSVReportIsWaiver = requestObject.IsWaiver;

                var toClose = true;

                if (request.SSVReportIsWaiver && ((Requests.IndexOf(request) + 1) != Requests.Count)) toClose = false;
                var ssvReport = GetSSVFileFromBulk(request);

                SingleRequest SingleRequest = new(request, ssvReport, _request);
                var (Saved, Error) = await SingleRequest.Save(requestObject, toClose);

                if (Saved)
                {
                    ValidRequests.Add(request);
                    Errors.Add(Error);
                }
                else
                {
                    Errors.Add(Error);
                }
            }
            var SaveStatus = GetSaveStatus(Requests.Count, ValidRequests.Count);

            return (SaveStatus, Errors, ValidRequests);
        }

        private static string GetSaveStatus(int requestCount, int validRequestCount)
        {
            if (validRequestCount == 0) return "All Failed";

            if (requestCount == validRequestCount) return "All Good";

            return "Some Failed";
        }

        public override async Task SetCreateState()
        {
            await _request.SetCreateState(ValidRequests, Variables);
        }

        private FilesManager GetSSVFileFromBulk(RequestViewModel request)
        {
            FilesManager uploadFile;

            var spectrumName = Spectrums.FirstOrDefault(x => x.Id == request.SpectrumId).Name;
            var TechTypeName = TechTypes.FirstOrDefault(x => x.Id == request.TechTypeId).Name;
            var projectType = ProjectTypes.FirstOrDefault(x => x.Id == request.ProjectTypeId).Name;

            var checkName = (spectrumName.Contains("RRU"))
                ? $"{request.SiteId.ToUpper()}_{TechTypeName}_{spectrumName}"
                : $"{request.SiteId.ToUpper()}_{spectrumName.ToUpper().RemoveSpecialCharacters()}";

            if (IsWaiver)
            {
                uploadFile = SSVReports.FirstOrDefault(x => x.UploadType == "Waiver");
            }
            else
            {
                uploadFile = SSVReports.FirstOrDefault(x => x.UploadType == "SSV" && x.Filename.Contains(checkName));
            }

            return uploadFile;
        }
    }
}
