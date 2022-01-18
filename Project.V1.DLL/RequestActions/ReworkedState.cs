using Project.V1.DLL.Helpers;
using Project.V1.DLL.Services.Interfaces;
using Project.V1.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.V1.DLL.RequestActions
{
    public class ReworkedState<T> : RequestStateBase<T> where T : RequestViewModel, IDisposable
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

                if (await _request.UpdateRequest(request, x => x.Id == request.Id))
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

            var regionEngineers = (await LoginObject.UserManager.GetUsersInRoleAsync("Engineer")).Where(x => x.Regions.Select(x => x.Id).Contains(request.RegionId)).Select(x => new SenderBody
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
            var vendorMailList = (user.VendorId != null) ? (await LoginObject.Vendor.GetById(x => x.Id == user.VendorId))?.MailList : null;

            Dictionary<string, Func<SendEmailActionObj>> processMailBody = new()
            {
                ["Requester"] = () =>
                {
                    return new SendEmailActionObj
                    {
                        Name = "Hello " + request.Requester.Name,
                        Title = "Update Notification on Request - See Below Request Details",
                        Greetings = $"Site Acceptance Request : <font color='blue'><b>Rework Request</b></font> - See Details below:",
                        Comment = "",
                        BodyType = "",
                        M2Uname = request.Requester.Username.ToLower().Trim(),
                        Link = $"https://ojtssapp1/spm/Identity/Account/Login?ReturnUrl=%2Fspm/{application}/worklist/{request.Id}",
                        To = new List<SenderBody> {
                            new SenderBody { Name = request.Requester.Name, Address = request.Requester.Email },
                        },
                        CC = new List<SenderBody> {
                            new SenderBody { Name = "Adekunle Adeyemi", Address = "Adekunle.Adeyemi@mtn.com" },
                        }
                    };
                },

                ["RF Team"] = () =>
                {
                    var emailObj = new SendEmailActionObj
                    {
                        Name = "Hello RF Team",
                        Title = "Update Notification on Request - See Below Request Details",
                        Greetings = $"Site Acceptance Request : <font color='blue'><b>Rework Request</b></font> - See Details below:",
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
