using Project.V1.DLL.Helpers;

namespace Project.V1.DLL.RequestActions
{
    public class ReworkedState<T> : RequestStateBase<T> where T : RequestViewModel, IDisposable
    {
        public override bool Accept(IRequestAction<T> request, T requests, Dictionary<string, object> variables)
        {
            return request.TransitionState(new AcceptedState<T>(), requests, variables, null);
        }

        public override bool Reject(IRequestAction<T> request, T requests, Dictionary<string, object> variables, string reason)
        {
            return request.TransitionState(new RejectedState<T>(), requests, variables, null);
        }

        public override async Task<bool> EnterState(IRequestAction<T> _request, T request, Dictionary<string, object> variables, RequestApproverModel ActionedBy)
        {
            try
            {
                string application = variables["App"] as string;

                if (await _request.UpdateRequest(request, x => x.Id == request.Id, request.Navigations))
                {
                    await SendEmail(application, request);
                }

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
            ApplicationUser user = await LoginObject.User.GetUserByUsername(request.Requester.Username);

            var engineers = await LoginObject.UserManager.GetUsersInRoleAsync("Engineer");

            var regionEngineers = engineers.Where(x => x.Regions.Select(x => x.Id).Contains(request.RegionId)).Select(x => new SenderBody
            {
                Name = x.Fullname,
                Address = x.Email
            });

            SendEmailActionObj emailObj = await GenerateMailBody("Requester", request, user, application, regionEngineers);
            await SendNotification(request, emailObj, "");

            emailObj = await GenerateMailBody("RF Team", request, user, application, regionEngineers);
            await SendNotification(request, emailObj, "Engineer");
        }

        private static async Task<SendEmailActionObj> GenerateMailBody(string mailType, T request, ApplicationUser user, string application, IEnumerable<SenderBody> regionEngineers)
        {
            var vendorMailList = (user.VendorId != null) ? (await LoginObject.Vendor.Get()).FirstOrDefault(x => x.Id == user.VendorId)?.MailList : null;

            Dictionary<string, Func<SendEmailActionObj>> processMailBody = new()
            {
                ["Requester"] = () =>
                {
                    var emailObj = new SendEmailActionObj
                    {
                        Name = "Hello " + request.Requester.Name,
                        Title = "Update Notification on Request - See Below Request Details",
                        Greetings = $"Site Acceptance Request : <font color='blue'><b>Rework Request</b></font> - See Details below:",
                        Comment = "",
                        Subject = ($"Site Acceptance Request ({(request as dynamic).Region.Name}) - {(request as dynamic).UniqueId} Notice").Replace("  ", " "),
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
                    emailObj.CC.AddRange(HelperFunctions.ConvertMailStringToList(vendorMailList));

                    return emailObj;
                },

                ["RF Team"] = () =>
                {
                    var emailObj = new SendEmailActionObj
                    {
                        Name = "Hello RF Team",
                        Title = "Update Notification on Request - See Below Request Details",
                        Greetings = $"Site Acceptance Request : <font color='blue'><b>Rework Request</b></font> - See Details below:",
                        Comment = "",
                        Subject = ($"Site Acceptance Request ({(request as dynamic).Region.Name}) - {(request as dynamic).UniqueId} RF Team Notice").Replace("  ", " "),
                        BodyType = "",
                        M2Uname = "", // requests.Manager.Username.ToLower().Trim(),
                        Link = $"https://ojtssapp1/smp/Identity/Account/Login?ReturnUrl={application}/engineer/worklist/{request.Id}",
                        To = regionEngineers.ToList(),
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
