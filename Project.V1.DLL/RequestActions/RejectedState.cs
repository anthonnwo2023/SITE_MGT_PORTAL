﻿using Project.V1.DLL.Helpers;
using System.IO;

namespace Project.V1.DLL.RequestActions
{
    public class RejectedState<T> : RequestStateBase<T> where T : RequestViewModel, IDisposable
    {
        public override bool Cancel(IRequestAction<T> request, T requests, Dictionary<string, object> variables)
        {
            return request.TransitionState(new CancelledState<T>(), requests, variables, null);
        }

        public override bool Rework(IRequestAction<T> request, T requests, Dictionary<string, object> variables)
        {
            return request.TransitionState(new ReworkedState<T>(), requests, variables, null);
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
            SendEmailActionObj emailObj = GenerateMailBody("Requester", request, application);
            await SendNotification(request, emailObj, "");

            emailObj = GenerateMailBody("Engineer", request, application);
            await SendNotification(request, emailObj, "Engineer");
        }

        private static SendEmailActionObj GenerateMailBody(string mailType, T request, string application)
        {           
            ApplicationUser user = LoginObject.User.GetUserByUsername(request.Requester.Username).GetAwaiter().GetResult();
            var vendorMailList = (user.VendorId != null) ? (LoginObject.Vendor.Get()).GetAwaiter().GetResult().FirstOrDefault(x => x.Id == user.VendorId)?.MailList : null;


            string bulkAttach = Path.Combine(Directory.GetCurrentDirectory(), $"Documents\\EReport\\{request.BulkuploadPath}");

            Dictionary<string, Func<SendEmailActionObj>> processMailBody = new()
            {
                ["Requester"] = () =>
                {
                    //return new SendEmailActionObj
                    //{
                    //    Name = "Hello " + request.Requester.Name,
                    //    Title = "Update Notification on Request - See Below Request Details",
                    //    Greetings = $"Site Acceptance Request : <font color='red'><b>Request Rejected</b></font> - See Details below:",
                    //    Comment = request.EngineerAssigned.ApproverComment,
                    //    Subject = ($"Site Acceptance Request ({(request as dynamic).Region.Name}) - {(request as dynamic).UniqueId} Notice").Replace("  ", " "),
                    //    BodyType = "",
                    //    M2Uname = request.Requester.Username.ToLower().Trim(),
                    //    Link = $"https://ojtssapp1/smp/Identity/Account/Login?ReturnUrl={application}/worklist/{request.Id}",
                    //    To = new List<SenderBody> {
                    //        new SenderBody { Name = request.Requester.Name, Address = request.Requester.Email },
                    //    },
                    //    CC = new List<SenderBody> {
                    //        new SenderBody {Name = "Anthony Nwosu", Address = "Anthony.Nwosu@mtn.com" },
                    //    },
                    //    Attachment = bulkAttach
                    //};

                    var emailObj = new SendEmailActionObj
                    {
                        Name = "Hello " + request.Requester.Name,
                        Title = "Update Notification on Request - See Below Request Details",
                        Greetings = $"Site Acceptance Request : <font color='red'><b>Request Rejected</b></font> - See Details below:",
                        Comment = request.EngineerAssigned.ApproverComment,
                        Subject = ($"Site Acceptance Request ({(request as dynamic).Region.Name}) - {(request as dynamic).UniqueId} Notice").Replace("  ", " "),
                        BodyType = "",
                        M2Uname = request.Requester.Username.ToLower().Trim(),
                        Link = $"https://ojtssapp1/smp/Identity/Account/Login?ReturnUrl={application}/worklist/{request.Id}",
                        To = new List<SenderBody> {
                            new SenderBody { Name = request.Requester.Name, Address = request.Requester.Email },
                        },
                        CC = new List<SenderBody> {
                            new SenderBody {Name = "Anthony Nwosu", Address = "Anthony.Nwosu@mtn.com" },
                        },
                        Attachment = bulkAttach
                    };

                    emailObj.CC.AddRange(HelperFunctions.ConvertMailStringToList(vendorMailList));
                    return emailObj;

                },

                ["Engineer"] = () =>
                {
                    return new SendEmailActionObj
                    {
                        Name = "Hello " + request.EngineerAssigned.Fullname,
                        Title = "Update Notification on Request - See Below Request Details",
                        Greetings = $"Site Acceptance Request : <font color='red'><b>Request Rejected</b></font> - See Details below:",
                        Comment = request.EngineerAssigned.ApproverComment,
                        Subject = ($"Site Acceptance Request ({(request as dynamic).Region.Name}) - {(request as dynamic).UniqueId} Engineer Notice").Replace("  ", " "),
                        BodyType = "",
                        M2Uname = request.EngineerAssigned.Username.ToLower().Trim(),
                        Link = $"https://ojtssapp1/smp/Identity/Account/Login?ReturnUrl={application}/engineer/worklist/detail/{request.Id}",
                        To = new List<SenderBody>
                        {
                           new SenderBody { Name = request.EngineerAssigned.Fullname, Address = request.EngineerAssigned.Email },
                           //new SenderBody {Name = "Anthony Nwosu", Address = "Anthony.Nwosu@mtn.com" },
                        },
                        CC = new List<SenderBody> {
                            new SenderBody {Name = "Anthony Nwosu", Address = "Anthony.Nwosu@mtn.com" },
                        },
                        Attachment = bulkAttach
                    };
                }
            };

            return processMailBody[mailType].Invoke();
        }
    }
}
