using Project.V1.DLL.Helpers;
using Project.V1.DLL.Services.Interfaces;
using Project.V1.Lib.Helpers;
using Project.V1.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.V1.DLL.RequestActions
{
    public class RestartedState<T> : RequestStateBase<T> where T : RequestViewModel, IDisposable
    {
        public override bool Accept(IRequestAction<T> request, T requests, Dictionary<string, object> variables)
        {
            return request.TransitionState(new AcceptedState<T>(), requests, variables);
        }

        public override bool Reject(IRequestAction<T> request, T requests, Dictionary<string, object> variables, string reason)
        {
            return request.TransitionState(new RejectedState<T>(), requests, variables);
        }

        public override async Task<bool> EnterState(IRequestAction<T> _request, T request, Dictionary<string, object> variables)
        {
            try
            {
                string application = variables["App"] as string;

                await _request.UpdateRequest(request, x => x.Id == request.Id);

                await SendEmail(application, request);

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
                        Greetings = $"Site Acceptance Request : <font color='blue'><b>Request Restarted</b></font> - See Details below:",
                        Comment = "",
                        BodyType = "",
                        M2Uname = user.UserName.ToLower().Trim(),
                        Link = $"https://ojtssapp1/smp/Identity/Account/Login?ReturnUrl={application}/worklist/detail/{request.Id}",
                        To = new List<SenderBody> {
                            new SenderBody { Name = user.Fullname, Address = user.Email },
                        },
                        CC = new List<SenderBody> {
                            new SenderBody { Name = "Adekunle Adeyemi", Address = "Adekunle.Adeyemi@mtn.com" },
                        }
                    };
                    emailObj.CC.AddRange(HelperFunctions.ConvertMailStringToList(vendorMailList));

                    return emailObj;
                },

                ["RFTeam"] = () =>
                {
                    var emailObj = new SendEmailActionObj
                    {
                        Name = "Hello RF Team",
                        Title = "Notification of New Request for approval - See Below Request Details",
                        Greetings = $"Site Acceptance Request : <font color='blue'><b>Request Restarted</b></font> - See Details below:",
                        Comment = "",
                        BodyType = "",
                        M2Uname = "", // requests.Manager.Username.ToLower().Trim(),
                        To = regionEngineers.ToList(),
                        Link = $"https://ojtssapp1/smp/Identity/Account/Login?ReturnUrl={application}/engineer/worklist/{request.Id}",
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
