namespace Project.V1.DLL.RequestActions.SiteHalt
{
    public class FAApprovedState<T> : RequestStateBase<T> where T : SiteHUDRequestModel, IDisposable
    {
        public override bool Approve(IRequestAction<T> request, T requests, Dictionary<string, object> variables)
        {
            return request.TransitionState(new SAApprovedState<T>(), requests, variables, null);
        }

        public override bool Update(IRequestAction<T> request, T requests, Dictionary<string, object> variables)
        {
            return request.TransitionState(new ToUpdateRequest<T>(), requests, variables, null);
        }

        public override bool Disapprove(IRequestAction<T> request, T requests, Dictionary<string, object> variables, RequestApproverModel ActionedBy)
        {
            return request.TransitionState(new DisapprovedState<T>(), requests, variables, ActionedBy);
        }

        public override async Task<bool> EnterState(IRequestAction<T> _request, T request, Dictionary<string, object> variables, RequestApproverModel ActionedBy)
        {
            try
            {
                string application = variables["App"] as string;

                request.Status = "FAApproved";

                bool isSaved = await _request.UpdateRequest(request, x => x.Id == request.Id, request.Navigations);

                if (isSaved)
                {
                    await SendEmail(application, request);
                }

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

            emailObj = GenerateMailBody("FAApprover", request, application);
            await SendNotification(request, emailObj, "FAApprover");

            emailObj = GenerateMailBody("SAApprover", request, application);
            await SendNotification(request, emailObj, "SAApprover");
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
                        Greetings = $"HUD {(request as dynamic).RequestAction} Request : <font color='orange'><b>Request Approved by RF Senior Manager ({(request as dynamic).FirstApprover.Fullname})</b></font>, awaiting next approval - See Details below:",
                        Comment = (request as dynamic).FirstApprover.ApproverComment,
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

                ["FAApprover"] = () =>
                {
                    return new SendEmailActionObj
                    {
                        Name = "Hello " + (request as dynamic).FirstApprover.Fullname.Trim(),
                        Title = "Update Notification on Request - See Below Request Details",
                        Greetings = $"HUD {(request as dynamic).RequestAction} Request : <font color='orange'><b>Request Approved</b></font> - See Details below:",
                        Comment = (request as dynamic).FirstApprover.ApproverComment,
                        Subject = ($"{(request as dynamic).RequestAction} Request: {((dynamic)request).UniqueId} Update Notice"),
                        BodyType = "",
                        M2Uname = (request as dynamic).FirstApprover.Username.ToLower().Trim(),
                        Link = $"https://ojtssapp1/smp/Identity/Account/Login?ReturnUrl={application}/report/{(request as dynamic).Id}",
                        To = new List<SenderBody> {
                            new SenderBody { Name = (request as dynamic).FirstApprover.Fullname, Address = (request as dynamic).FirstApprover.Email },
                        },
                        CC = new List<SenderBody> {
                            new SenderBody { Name = "Adekunle Adeyemi", Address = "Adekunle.Adeyemi@mtn.com" },
                        }
                    };
                },

                ["SAApprover"] = () =>
                {
                    return new SendEmailActionObj
                    {
                        Name = "Hello " + (request as dynamic).SecondApprover.Fullname.Trim(),
                        Title = "Update Notification on Request - See Below Request Details",
                        Greetings = $"HUD {(request as dynamic).RequestAction} Request : <font color='orange'><b>Request Approved by RF Senior Manager ({(request as dynamic).FirstApprover.Fullname})</b></font>, but awaiting your approval - See Details below:",
                        Comment = (request as dynamic).FirstApprover.ApproverComment,
                        Subject = ($"{(request as dynamic).RequestAction} Request: {((dynamic)request).UniqueId} Action Notice"),
                        BodyType = "",
                        M2Uname = (request as dynamic).SecondApprover.Username.ToLower().Trim(),
                        Link = $"https://ojtssapp1/smp/Identity/Account/Login?ReturnUrl={application}/engineer/worklist/detail/{(request as dynamic).Id}",
                        To = new List<SenderBody> {
                            new SenderBody { Name = (request as dynamic).SecondApprover.Fullname, Address = (request as dynamic).SecondApprover.Email },
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
