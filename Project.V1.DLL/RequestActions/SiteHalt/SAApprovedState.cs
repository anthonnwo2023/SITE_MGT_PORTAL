namespace Project.V1.DLL.RequestActions.SiteHalt
{
    public class SAApprovedState<T> : RequestStateBase<T> where T : SiteHUDRequestModel, IDisposable
    {
        public override bool Approve(IRequestAction<T> request, T requests, Dictionary<string, object> variables)
        {
            return request.TransitionState(new TAApprovedState<T>(), requests, variables, null);
        }

        public override bool Update(IRequestAction<T> request, T requests, Dictionary<string, object> variables)
        {
            return request.TransitionState(new ToUpdateRequest<T>(), requests, variables, null);
        }

        public override bool Complete(IRequestAction<T> request, T requests, Dictionary<string, object> variables)
        {
            if (requests.IsForceMajeure)
                return request.TransitionState(new CompletedState<T>(), requests, variables, null);

            return false;
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

                request.Status = "SAApproved";

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

            emailObj = GenerateMailBody("SAApprover", request, application);
            await SendNotification(request, emailObj, "SAApprover");

            if (request.ThirdApprover != null)
            {
                emailObj = GenerateMailBody("TAApprover", request, application);
                await SendNotification(request, emailObj, "TAApprover");
            }
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
                        Greetings = $"HUD {(request as dynamic).RequestAction} Request : <font color='orange'><b>Request Approved by ({(request as dynamic).SecondApprover.Fullname})</b></font>, awaiting next approval - See Details below:",
                        Comment = (request as dynamic).SecondApprover.ApproverComment,
                        Subject = ($"{(request as dynamic).RequestAction} Request: {((dynamic)request).UniqueId} Update Notice"),
                        Body = (request.RequestAction != "UnHalt") ? $"<p> Approver 1 : <b>{request.FirstApprover.Fullname} </b> <font color='green'><b>Approved</b></font> </p><p> Approver 2 : <b>{request.SecondApprover.Fullname} </b> <font color='green'><b>Approved</b></font></p>" : "",
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

                ["SAApprover"] = () =>
                {
                    return new SendEmailActionObj
                    {
                        Name = "Hello " + (request as dynamic).SecondApprover.Fullname.Trim(),
                        Title = "Update Notification on Request - See Below Request Details",
                        Greetings = $"HUD {(request as dynamic).RequestAction} Request : <font color='orange'><b>Request Approved</b></font> - See Details below:",
                        Comment = (request as dynamic).SecondApprover.ApproverComment,
                        Subject = ($"{(request as dynamic).RequestAction} Request: {((dynamic)request).UniqueId} Update Notice"),
                        Body = (request.RequestAction != "UnHalt") ? $"<p> Approver 1 : <b>{request.FirstApprover.Fullname} </b> <font color='green'><b>Approved</b></font> </p><p> Approver 2 : <b>{request.SecondApprover.Fullname} </b> <font color='green'><b>Approved</b></font></p>" : "",
                        BodyType = "",
                        M2Uname = (request as dynamic).SecondApprover.Username.ToLower().Trim(),
                        Link = $"https://ojtssapp1/smp/Identity/Account/Login?ReturnUrl={application}/report/{(request as dynamic).Id}",
                        To = new List<SenderBody> {
                            new SenderBody { Name = (request as dynamic).SecondApprover.Fullname, Address = (request as dynamic).SecondApprover.Email },
                        },
                        CC = new List<SenderBody> {
                            new SenderBody { Name = "Adekunle Adeyemi", Address = "Adekunle.Adeyemi@mtn.com" },
                        }
                    };
                },

                ["TAApprover"] = () =>
                {
                    return new SendEmailActionObj
                    {
                        Name = "Hello " + (request as dynamic).ThirdApprover.Fullname.Trim(),
                        Title = "Update Notification on Request - See Below Request Details",
                        Greetings = $"HUD {(request as dynamic).RequestAction} Request : <font color='orange'><b>Request Approved by ({(request as dynamic).SecondApprover.Fullname})</b></font>, but awaiting your approval - See Details below:",
                        Comment = (request as dynamic).SecondApprover.ApproverComment,
                        Subject = ($"{(request as dynamic).RequestAction} Request: {((dynamic)request).UniqueId} Action Notice"),
                        Body = $"<p> Approver 1 : <b>{(request as dynamic).FirstApprover.Fullname} </b> <font color='green'><b>Approved</b></font></p>",
                        BodyType = "",
                        M2Uname = (request as dynamic).ThirdApprover.Username.ToLower().Trim(),
                        Link = $"https://ojtssapp1/smp/Identity/Account/Login?ReturnUrl={application}/engineer/worklist/detail/{(request as dynamic).Id}",
                        To = new List<SenderBody> {
                            new SenderBody { Name = (request as dynamic).ThirdApprover.Fullname, Address = (request as dynamic).ThirdApprover.Email },
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
