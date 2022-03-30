namespace Project.V1.DLL.RequestActions.SiteHalt;

public class CompletedState<T> : RequestStateBase<T> where T : SiteHUDRequestModel, IDisposable
{
    public override async Task<bool> EnterState(IRequestAction<T> _request, T request, Dictionary<string, object> variables, RequestApproverModel ActionedBy)
    {
        try
        {
            string application = variables["App"] as string;
            string user = variables["User"] as string;

            request.Status = "Completed";

            bool isSaved = await _request.UpdateRequest(request, x => x.Id == request.Id);

            if (isSaved)
                await SendEmail(application, request);

            return true;
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, $"{ex.Message}, {ex.InnerException}");
            return false;
        }
    }

    public async Task SendEmail(string application, T request)
    {
        SendEmailActionObj emailObj = GenerateMailBody("Requester", request, application);
        await SendNotification(request, emailObj, "");

        emailObj = GenerateMailBody("Engineer", request, application);
        await SendNotification(request, emailObj, "Engineer");
    }

    private static SendEmailActionObj GenerateMailBody(string mailType, T request, string application)
    {
        Dictionary<string, Func<SendEmailActionObj>> processMailBody = new()
        {
            ["Requester"] = () =>
            {
                return new SendEmailActionObj
                {
                    Name = "Hello " + request.Requester.Name.Trim(),
                    Title = "Update Notification on Request - See Below Request Details",
                    Greetings = $"HUD {(request as dynamic).RequestAction} Request : <font color='orange'><b>Request Completed on TNIS by ({(request as dynamic).User.Fullname})</b></font> - See Details below:",
                    Comment = (request as dynamic).ThirdApprover.ApproverComment,
                    Body = $"<p> Approver 1 : <b>{(request as dynamic).FirstApprover.Fullname} </b> <font color='green'><b>Approved</b></font> </p><p> Approver 2 : <b>{(request as dynamic).SecondApprover.Fullname} </b> <font color='green'><b>Approved</b></font></p><p> Approver 3 : <b>{(request as dynamic).ThirdApprover.Fullname} </b> <font color='green'><b>Approved</b></font></p>",
                    Subject = ($"{(request as dynamic).RequestAction} Request: {((dynamic)request).UniqueId} Update Notice"),
                    BodyType = "",
                    M2Uname = request.Requester.Username.ToLower().Trim(),
                    Link = $"https://ojtssapp1/smp/Identity/Account/Login?ReturnUrl={application}/report/{(request as dynamic).Id}",
                    To = new List<SenderBody> {
                        new SenderBody { Name = request.Requester.Name, Address = request.Requester.Email },
                    },
                    CC = new List<SenderBody> {
                        new SenderBody { Name = "Adekunle Adeyemi", Address = "Adekunle.Adeyemi@mtn.com" },
                    }
                };
            },

            ["Engineer"] = () =>
            {
                return new SendEmailActionObj
                {
                    Name = "Hello " + request.User.Fullname,
                    Title = "Update Notification on Request - See Below Request Details",
                    Greetings = $"HUD {request.RequestAction} Request : <font color='orange'><b>Request Completed on TNIS</b></font> - See Details below:",
                    Comment = request.ThirdApprover.ApproverComment,
                    Body = $"<p> Approver 1 : <b>{request.FirstApprover.Fullname} </b> <font color='green'><b>Approved</b></font> </p><p> Approver 2 : <b>{request.SecondApprover.Fullname} </b> <font color='green'><b>Approved</b></font></p><p> Approver 3 : <b>{request.ThirdApprover.Fullname} </b> <font color='green'><b>Approved</b></font></p>",
                    Subject = ($"{request.RequestAction} Request: {request.UniqueId} Update Notice"),
                    BodyType = "",
                    M2Uname = request.Requester.Username.ToLower().Trim(),
                    Link = $"https://ojtssapp1/smp/Identity/Account/Login?ReturnUrl={application}/report/{(request as dynamic).Id}",
                    To = new List<SenderBody> {
                        new SenderBody { Name = request.User.Fullname, Address = request.User.Email },
                    },
                    CC = new List<SenderBody> {
                        new SenderBody { Name = "Adekunle Adeyemi", Address = "Adekunle.Adeyemi@mtn.com" },
                    }
                };
            }
        };

        return processMailBody[mailType].Invoke();
    }
}
