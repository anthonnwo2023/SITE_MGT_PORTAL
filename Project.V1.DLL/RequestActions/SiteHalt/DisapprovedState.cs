namespace Project.V1.DLL.RequestActions.SiteHalt
{
    public class DisapprovedState<T> : RequestStateBase<T> where T : SiteHUDRequestModel, IDisposable
    {
        public override bool Restart(IRequestAction<T> request, T requests, Dictionary<string, object> variables)
        {
            return request.TransitionState(new RestartedState<T>(), requests, variables, null);
        }

        public override async Task<bool> EnterState(IRequestAction<T> _request, T request, Dictionary<string, object> variables, RequestApproverModel ActionedBy)
        {
            try
            {
                string application = variables["App"] as string;

                bool isSaved = await _request.UpdateRequest(request, x => x.Id == request.Id);

                if (isSaved)
                    await SendEmail(application, request, ActionedBy);

                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, $"{ex.Message}, {ex.InnerException}");
                return false;
            }
        }

        public async Task SendEmail(string application, T request, RequestApproverModel ActionedBy)
        {
            SendEmailActionObj emailObj = GenerateMailBody("Requester", request, application, ActionedBy);
            await SendNotification(request, emailObj, "");

            emailObj = GenerateMailBody("Approver", request, application, ActionedBy);
            await SendNotification(request, emailObj, "Approver");
        }

        private static SendEmailActionObj GenerateMailBody(string mailType, T request, string application, RequestApproverModel ActionedBy)
        {
            Dictionary<string, Func<SendEmailActionObj>> processMailBody = new()
            {
                ["Requester"] = () =>
                {
                    return new SendEmailActionObj
                    {
                        Name = "Hello " + request.Requester.Name.Trim(),
                        Title = "Update Notification on Request - See Below Request Details",
                        Greetings = $"HUD {(request as dynamic).RequestAction} Request : <font color='red'><b>Request Disapproved by {ActionedBy.Fullname}</b></font> - See Details below:",
                        Comment = ActionedBy.ApproverComment,
                        Subject = ($"{(request as dynamic).RequestAction} Request: {((dynamic)request).UniqueId} Notice"),
                        BodyType = "",
                        M2Uname = request.Requester.Username.ToLower().Trim(),
                        Link = $"https://ojtssapp1/smp/Identity/Account/Login?ReturnUrl={application}/worklist/{request.Id}",
                        To = new List<SenderBody> {
                            new SenderBody { Name = request.Requester.Name, Address = request.Requester.Email },
                        },
                        CC = new List<SenderBody> {
                            new SenderBody { Name = "Adekunle Adeyemi", Address = "Adekunle.Adeyemi@mtn.com" },
                        }
                    };
                },

                ["Approver"] = () =>
                {
                    return new SendEmailActionObj
                    {
                        Name = "Hello " + ActionedBy.Fullname.Trim(),
                        Title = "Update Notification on Request - See Below Request Details",
                        Greetings = $"HUD {(request as dynamic).RequestAction} Request : <font color='red'><b>Request Disapproved</b></font> - See Details below:",
                        Comment = ActionedBy.ApproverComment,
                        Subject = ($"{(request as dynamic).RequestAction} Request: {((dynamic)request).UniqueId} Notice"),
                        BodyType = "",
                        M2Uname = ActionedBy.Username.ToLower().Trim(),
                        Link = $"https://ojtssapp1/smp/Identity/Account/Login?ReturnUrl={application}/report/genaral/{request.Id}",
                        To = new List<SenderBody>
                        {
                            new SenderBody { Name = ActionedBy.Fullname, Address = ActionedBy.Email },
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
}
