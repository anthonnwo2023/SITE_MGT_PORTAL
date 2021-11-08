using Project.V1.DLL.Helpers;
using Project.V1.DLL.Services.Interfaces;
using Project.V1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.V1.DLL.RequestActions
{
    public class PendingState<T> : RequestStateBase<T> where T : RequestViewModel, IDisposable
    {
        public override void Accept(IRequestAction<T> request, T requests, Dictionary<string, object> variables)
        {
            request.TransitionState(new AcceptedState<T>(), requests, variables);
        }

        public override void Reject(IRequestAction<T> request, T requests, Dictionary<string, object> variables, string reason)
        {
            request.TransitionState(new RejectedState<T>(), requests, variables);
        }

        public override async Task EnterState(IRequestAction<T> _request, T requests, Dictionary<string, object> variables)
        {
            string application = variables["App"] as string;

            await SendEmail(application, requests);
        }

        public async Task SendEmail(string application, T request)
        {
            LoginObject.InitObjects();

            ApplicationUser user = await LoginObject.User.GetUserByUsername(request.Requester.Username);

            var regionEngineers = (await LoginObject.UserManager.GetUsersInRoleAsync("Engineer")).Where(x => x.Regions.Select(x => x.Id).Contains(request.RegionId)).Select(x => new SenderBody
            {
                Name = x.Fullname,
                Address = x.Email
            });

            SendEmailActionObj emailObj = await GenerateMailBody("Requester", request, user, application, regionEngineers);
            await SendNotification(request, emailObj, "");

            emailObj = await GenerateMailBody("RFTeam", request, user, application, regionEngineers);
            await SendNotification(request, emailObj, "RFTeam");
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
                        Name = "Hello " + user.Fullname,
                        Title = "Notification of New Request - See Below Request Details",
                        Greetings = $"NWG NAPO Site Acceptance Request : <font color='blue'><b>New Request</b></font> - See Details below:",
                        Comment = "",
                        BodyType = "",
                        M2Uname = user.UserName.ToLower().Trim(),
                        Link = $"https://ojtssapp1/smp/Identity/Account/Login?ReturnUrl=%2Fsmp/{application}/worklist/detail/{request.Id}",
                        To = new List<SenderBody> {
                            new SenderBody { Name = user.Fullname, Address = user.Email },
                        },
                        CC = new List<SenderBody> {
                            new SenderBody { Name = "Adekunle Adeyemi", Address = "Adekunle.Adeyemi@mtn.com" },
                            new SenderBody { Name = "", Address = vendorMailList },
                        }
                    };

                    emailObj.To.AddRange(regionEngineers);

                    return emailObj;
                },

                ["RFTeam"] = () =>
                {
                    var emailObj = new SendEmailActionObj
                    {
                        Name = "Hello RF Team",
                        Title = "Notification of New Request for approval - See Below Request Details",
                        Greetings = $"NWG NAPO Site Acceptance Request : <font color='blue'><b>New Request</b></font> - See Details below:",
                        Comment = "",
                        BodyType = "",
                        M2Uname = "", // requests.Manager.Username.ToLower().Trim(),
                        Link = $"https://ojtssapp1/smp/Identity/Account/Login?ReturnUrl=%2Fsmp/{application}/engineer/worklist/{request.Id}",
                        To = new List<SenderBody>
                        {
                            new SenderBody { Name = "#RF Planning", Address = "#rfplanning.ng@mtn.com" },
                        },
                        CC = new List<SenderBody> {
                            new SenderBody { Name = "Adekunle Adeyemi", Address = "Adekunle.Adeyemi@mtn.com" },
                        }
                    };

                    emailObj.To.AddRange(regionEngineers);

                    return emailObj;
                }
            };

            return processMailBody[mailType].Invoke();
        }
    }
}
