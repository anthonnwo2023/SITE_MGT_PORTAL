namespace Project.V1.Web.Requests;

public class SingleRequest : RequestBase
{
    private readonly IRequest _request;

    public SingleRequest(RequestViewModel Request, FilesManager ssvReport, IRequest request) :
        base(Request, ssvReport)
    {
        _request = request;
    }


    public override async Task<(bool Saved, string Error)> Save(AcceptanceRequestObject requestObject, bool toClose)
    {
        User = requestObject.User;
        ProjectTypes = requestObject.ProjectTypes;
        Spectrums = requestObject.Spectrums;
        TechTypes = requestObject.TechTypes;
        Variables = new() { { "User", requestObject.User.UserName }, { "App", "acceptance" } };
        IsWaiver = requestObject.IsWaiver;

        var spectrumName = Spectrums.FirstOrDefault(x => x.Id == Request.SpectrumId).Name;
        var TechTypeName = TechTypes.FirstOrDefault(x => x.Id == Request.TechTypeId).Name;
        var projectType = ProjectTypes.FirstOrDefault(x => x.Id == Request.ProjectTypeId).Name;

        var checkName = (spectrumName.Contains("RRU"))
            ? $"{Request.SiteId.ToUpper()}_{TechTypeName}_{spectrumName}"
            : $"{Request.SiteId.ToUpper()}_{spectrumName.ToUpper().RemoveSpecialCharacters()}";

        if (string.IsNullOrWhiteSpace(Request.BulkBatchNumber) && SSVReport?.Filename != null && !SSVReport.Filename.ToUpper().Contains(checkName.ToUpper()))
        {
            return (false, "Site Id does not match the uploaded SSV document");
        }

        if (SSVReport?.Filename == null && Helpers.ShouldRequireSSV(requestObject.ProjectTypes, spectrumName, TechTypeName, Request.ProjectTypeId))
        {
            return (false, $"Missing SSV Report! Please upload SSV document - {TechTypeName} {Request.SiteId} {spectrumName} {projectType}");
        }

        var (Saved, Error) = await SaveSingleRequest(SSVReport, toClose);

        if (!Saved)
        {
            return (false, "Unable to Save Request");
        }
        return (true, Error);
    }

    public override async Task SetCreateState()
    {
        await _request.SetCreateState(Request, Variables, null, null);
    }

    private async Task<(bool Saved, string Error)> SaveSingleRequest(FilesManager file, bool toClose)
    {
        try
        {
            if (file?.UploadFile != null)
            {
                string ext = Path.GetExtension(file.UploadFile.FileInfo.Name);
                bool allowedExtension = ExcelProcessor.IsAllowedExt(ext, Request.SSVReportIsWaiver);

                (string uploadResp, string filePath, string uploadError) = await FileUploader.StartUpload(allowedExtension, file, toClose);

                Request.SSVReport = Path.GetFileName(filePath);

                if (uploadResp.Length != 0 || uploadError.Length != 0)
                {
                    return (false, $"An error has occurred. SSV Report/Waiver could not be uploaded.");
                }
            }

            var (Saved, Error) = await SaveRequest();

            if (Saved)
            {
                return (true, Error);
            }

            return (false, Error);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    private async Task<(bool Saved, string Error)> SaveRequest()
    {
        try
        {
            Request.Id = Guid.NewGuid().ToString();
            Request.SiteId = Request.SiteId.ToUpper();
            Request.DateCreated = DateTime.Now;
            Request.DateSubmitted = DateTime.Now;
            Request.Status = "Pending";
            Request.RequestType = "SA";
            Request.DateUserActioned = DateTime.Now;
            //Request.SSVReportIsWaiver = WaiverUploadSelected;

            Request.EngineerAssigned = HelperFunctions.ModelApprover("EA", Request.Id);

            Request.Requester = GenerateRequestData();

            var (isCreated, errorMsg) = await _request.CreateRequest(Request);

            if (isCreated)
            {
                return (true, $"Request Submitted successfully {Request.SiteId} - {Spectrums.FirstOrDefault(x => x.Id == Request.SpectrumId)?.Name}.");
            }

            return (false, $"Request could not be created. {errorMsg}");
        }
        catch (Exception ex)
        {
            return (false, (ex.InnerException.Message.Contains("unique")) ? "Duplicate entry found" : $"An error has occurred. {ex.Message}");
        }
    }

    private RequesterData GenerateRequestData()
    {
        return new RequesterData
        {
            Id = Guid.NewGuid().ToString(),
            Name = User?.Fullname,
            Department = User?.Department,
            Email = User?.Email,
            Phone = User?.PhoneNumber,
            Title = User?.JobTitle,
            Username = User?.UserName,
            VendorId = User?.VendorId
        };
    }

}
