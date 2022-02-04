namespace Project.V1.Web.Request;

public abstract class RequestBase : IDisposable
{
    public RequestViewModel Request { get; set; }

    public List<RequestViewModel> Requests { get; set; }

    public List<RequestViewModel> ValidRequests { get; set; } = new();

    public bool IsWaiver { get; set; }

    public FilesManager SSVReport { get; set; }

    public List<FilesManager> SSVReports { get; set; }

    public ApplicationUser User { get; set; }

    public List<ProjectTypeModel> ProjectTypes { get; set; }

    public List<SpectrumViewModel> Spectrums { get; set; }

    public List<TechTypeModel> TechTypes { get; set; }

    public Dictionary<string, object> Variables { get; set; }

    public RequestBase(RequestViewModel request, FilesManager ssvReport)
    {
        Request = request;
        SSVReport = ssvReport;
    }

    public RequestBase(List<RequestViewModel> requests, List<FilesManager> ssvReports)
    {
        Requests = requests;
        SSVReports = ssvReports;
    }

    public virtual Task<(bool Saved, string Error)> Save(SingleRequestObject requestObject, bool toClose)
        => Task.FromResult((false, "Method not allowed - Custom Internal Error"));

    public virtual Task<(string SaveStatus, List<string> Messages, List<RequestViewModel> ValidRequests)> Save(SingleRequestObject requestObject)
         => Task.Run<(string SaveStatus, List<string> Messages, List<RequestViewModel> ValidRequests)>(() =>
         {
             return ("Method not allowed", null, null);
         });

    public abstract Task SetCreateState();

    public void Dispose()
    {
    }
}
