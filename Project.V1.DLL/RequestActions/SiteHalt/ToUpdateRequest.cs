namespace Project.V1.DLL.RequestActions.SiteHalt
{
    public class ToUpdateRequest<T> : RequestStateBase<T> where T : SiteHUDRequestModel, IDisposable
    {
        public override async Task<bool> EnterState(IRequestAction<T> _request, T request, Dictionary<string, object> variables, RequestApproverModel ActionedBy)
        {
            try
            {
                string application = variables["App"] as string;
                string user = variables["User"] as string;

                await _request.UpdateRequest(request, x => x.Id == request.Id, request.Navigations);

                //await SendEmail(application, request);

                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, ex.Message);
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
                        Name = "Hello " + request.Requester.Name,
                        Title = "Update Notification on Request - See Below Request Details",
                        Greetings = $"HUD {(request as dynamic).RequestAction} Request : <font color='orange'><b>Request Updated by ()</b></font> - See Details below:",
                        Comment = (request as dynamic).ThirdApprover.ApproverComment,
                        Subject = ($"{(request as dynamic).RequestAction} Request: {((dynamic)request).UniqueId} Update Notice"),
                        BodyType = "",
                        Body = $"<p> Approver 1 : <b>{(request as dynamic).FirstApprover?.Fullname} </b></p><p> Approver 2 : <b>{(request as dynamic).SecondApprover?.Fullname} </b></p><p> Approver 3 : <b>{(request as dynamic).ThirdApprover?.Fullname} </b></p>",
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
                        Body = $"<p> Approver 1 : <b>{(request as dynamic).FirstApprover?.Fullname} </b></p><p> Approver 2 : <b>{(request as dynamic).SecondApprover?.Fullname} </b></p><p> Approver 3 : <b>{(request as dynamic).ThirdApprover?.Fullname} </b></p>",
                    };
                }
            };

            return processMailBody[mailType].Invoke();
        }
    }
}
