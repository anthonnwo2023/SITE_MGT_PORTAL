using Project.V1.DLL.Helpers;
using Project.V1.DLL.Services.Interfaces;
using Project.V1.Lib.Helpers;
using Project.V1.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Project.V1.DLL.RequestActions
{
    public class PendingBulkState<T> : RequestStateBase<T> where T : RequestViewModel, IDisposable
    {
        public override async Task<bool> EnterState(IRequestAction<T> _request, List<T> requests, Dictionary<string, object> variables)
        {
            try
            {
                string username = variables["User"] as string;
                string application = variables["App"] as string;

                await SendEmail(username, application, requests);

                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, ex.Message);
                return false;
            }
        }

        public async Task SendEmail(string username, string application, List<T> requests)
        {
            ApplicationUser user = await LoginObject.User.GetUserByUsername(username);

            var regionEngineers = (await LoginObject.UserManager.GetUsersInRoleAsync("Engineer")).Where(x => x.Regions.Select(x => x.Id).Contains(requests.First().RegionId)).Select(x => new SenderBody
            {
                Name = x.Fullname,
                Address = x.Email
            });

            SendEmailActionObj emailObj = await GenerateMailBody("Requester", requests, user, application, regionEngineers);
            await SendNotification(requests, emailObj, "", "Site Acceptance");

            emailObj = await GenerateMailBody("RFTeam", requests, user, application, regionEngineers);
            await SendNotification(requests, emailObj, "RFTeam", "Site Acceptance");
        }

        private static async Task<SendEmailActionObj> GenerateMailBody(string mailType, List<T> requests, ApplicationUser user, string application, IEnumerable<SenderBody> regionEngineers)
        {
            var vendorMailList = (user.VendorId != null) ? (await LoginObject.Vendor.Get()).FirstOrDefault(x => x.Id == user.VendorId)?.MailList : null;
            string bulkAttach = Path.Combine(Directory.GetCurrentDirectory(), $"Documents\\Bulk\\{requests.First().BulkuploadPath}");

            var requestObjs = ((dynamic)requests) as List<RequestViewModel>;
            var request = requestObjs.FirstOrDefault();

            Dictionary<string, Func<SendEmailActionObj>> processMailBody = new()
            {
                ["Requester"] = () =>
                {
                    var emailObj = new SendEmailActionObj
                    {
                        Name = "Hello " + user.Fullname,
                        Title = "Notification of New Request - See Below Request Details",
                        Greetings = $"Site Acceptance Request : <font color='blue'><b>New Bulk Request</b></font> - See Details below:",
                        Comment = "",
                        Subject = ($"Site Acceptance Request ({(request as dynamic).Region.Name}) - {(request as dynamic).BulkBatchNumber} Notice"),
                        BodyType = "",
                        M2Uname = user.UserName.ToLower().Trim(),
                        Link = $"https://ojtssapp1/smp/Identity/Account/Login?ReturnUrl={application}/worklist",
                        To = new List<SenderBody> {
                            new SenderBody { Name = user.Fullname, Address = user.Email },
                        },
                        CC = new List<SenderBody> {
                            new SenderBody {Name = "Anthony Nwosu", Address = "Anthony.Nwosu@mtn.com" },
                        },
                        Attachment = bulkAttach
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
                        Greetings = $"Site Acceptance Request : <font color='blue'><b>New Bulk Request</b></font> - See Details below:",
                        Comment = "",
                        Subject = ($"Site Acceptance Request ({(request as dynamic).Region.Name}) - {(request as dynamic).BulkBatchNumber} RFTeam Notice"),
                        BodyType = "",
                        M2Uname = "",// requests.Manager.Username.ToLower().Trim(),
                        Link = $"https://ojtssapp1/smp/Identity/Account/Login?ReturnUrl={application}/engineer/worklist",
                        To = regionEngineers.ToList(),
                        CC = new List<SenderBody> {
                            new SenderBody {Name = "Anthony Nwosu", Address = "Anthony.Nwosu@mtn.com" },
                        },
                        Attachment = bulkAttach
                    };

                    return emailObj;
                }
            };

            return processMailBody[mailType].Invoke();
        }
    }
}
