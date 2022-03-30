using Project.V1.DLL.Helpers;

namespace Project.V1.DLL.RequestActions.SiteHalt
{
    public class PendingState<T> : RequestStateBase<T> where T : SiteHUDRequestModel, IDisposable
    {
        public override bool Approve(IRequestAction<T> request, T requests, Dictionary<string, object> variables)
        {
            return request.TransitionState(new RFSMApprovedState<T>(), requests, variables, null);
        }

        public override bool Update(IRequestAction<T> request, T requests, Dictionary<string, object> variables)
        {
            return request.TransitionState(new ToUpdateRequest<T>(), requests, variables, null);
        }

        public override bool Disapprove(IRequestAction<T> request, T requests, Dictionary<string, object> variables, RequestApproverModel ActionedBy)
        {
            return request.TransitionState(new DisapprovedState<T>(), requests, variables, ActionedBy);
        }

        public override async Task<bool> EnterState(IRequestAction<T> _request, T requests, Dictionary<string, object> variables, RequestApproverModel ActionedBy)
        {
            try
            {
                string application = variables["App"] as string;

                await SendEmail(application, requests);

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
            ApplicationUser user = await LoginObject.User.GetUserByUsername(request.Requester.Username);

            SendEmailActionObj emailObj = GenerateMailBody("Requester", request, user, application);
            await SendNotification(request, emailObj, "");

            emailObj = GenerateMailBody("FAApprover", request, user, application);
            await SendNotification(request, emailObj, "FAApprover");
        }

        private static SendEmailActionObj GenerateMailBody(string mailType, T request, ApplicationUser user, string application)
        {
            Dictionary<string, Func<SendEmailActionObj>> processMailBody = new()
            {
                ["Requester"] = () =>
                {
                    return new SendEmailActionObj
                    {
                        Name = "Hello " + user.Fullname.Trim(),
                        Title = "Notification of New Request - See Below Request Details",
                        Greetings = $"HUD {(request as dynamic).RequestAction} Request : <font color='blue'><b>New Request</b></font> - See Details below:",
                        Comment = $"{(request as dynamic).SiteIds}",
                        Subject = ($"{(request as dynamic).RequestAction} Request: {((dynamic)request).UniqueId} Notice"),
                        BodyType = "",
                        M2Uname = user.UserName.ToLower().Trim(),
                        Link = $"https://ojtssapp1/smp/Identity/Account/Login?ReturnUrl={application}/report/general/{(request as dynamic).Id}",
                        To = new List<SenderBody> {
                            new SenderBody { Name = user.Fullname, Address = user.Email },
                        },
                        CC = new List<SenderBody> {
                            new SenderBody { Name = "Adekunle Adeyemi", Address = "Adekunle.Adeyemi@mtn.com" },
                        }
                    };
                },

                ["FAApprover"] = () =>
                {
                    var emailObj = new SendEmailActionObj
                    {
                        Name = "Hello " + (request as dynamic).FirstApprover.Fullname.Trim(),
                        Title = "Notification of New Request - See Below Request Details",
                        Greetings = $"HUD {(request as dynamic).RequestAction} Request : <font color='blue'><b>New Request</b></font> - See Details below:",
                        Comment = $"{(request as dynamic).SiteIds}",
                        Subject = ($"{(request as dynamic).RequestAction} Request: {((dynamic)request).UniqueId} RF SM Action Notice"),
                        BodyType = "",
                        M2Uname = (request as dynamic).FirstApprover.Username?.ToLower().Trim(),
                        Link = $"https://ojtssapp1/smp/Identity/Account/Login?ReturnUrl={application}/engineer/worklist/{(request as dynamic).Id}",
                        To = new List<SenderBody> {
                            new SenderBody { Name = (request as dynamic).FirstApprover.Fullname, Address = (request as dynamic).FirstApprover.Email },
                        },
                        CC = new List<SenderBody> {
                            new SenderBody { Name = "Adekunle Adeyemi", Address = "Adekunle.Adeyemi@mtn.com" },
                        }
                    };

                    return emailObj;
                }
            };

            return processMailBody[mailType].Invoke();
        }
    }
}
