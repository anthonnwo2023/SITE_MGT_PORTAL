using Project.V1.DLL.Services.Interfaces;
using Project.V1.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.V1.DLL.RequestActions
{
    public class AcceptedState<T> : RequestStateBase<T> where T : RequestViewModel, IDisposable
    {
        public override async Task<bool> EnterState(IRequestAction<T> _request, T request, Dictionary<string, object> variables, RequestApproverModel ActionedBy)
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
            SendEmailActionObj emailObj = GenerateMailBody("Requester", request, application);
            await SendNotification(request, emailObj, "");

            emailObj = GenerateMailBody("Engineer", request, application);
            await SendNotification(request, emailObj, "Engineer");
        }

        private static SendEmailActionObj GenerateMailBody(string mailType, T request, string application)
        {
            Dictionary<string, Func<SendEmailActionObj>> processMailBody = new()
            {
                ["Requester"] = () =>
                {
                    return new SendEmailActionObj
                    {
                        Name = "Hello " + request.Requester.Name,
                        Title = "Update Notification on Request - See Below Request Details",
                        Greetings = $"Site Acceptance Request : <font color='green'><b>Request Accepted</b></font> - See Details below:",
                        Comment = request.EngineerAssigned.ApproverComment,
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
                },

                ["Engineer"] = () =>
                {
                    return new SendEmailActionObj
                    {
                        Name = "Hello " + request.EngineerAssigned.Fullname,
                        Title = "Update Notification on Request - See Below Request Details",
                        Greetings = $"Site Acceptance Request : <font color='green'><b>Request Accepted</b></font> - See Details below:",
                        Comment = request.EngineerAssigned.ApproverComment,
                        Subject = ($"Site Acceptance Request ({(request as dynamic).Region.Name}) - {(request as dynamic).UniqueId} Engineer Notice").Replace("  ", " "),
                        BodyType = "",
                        M2Uname = request.EngineerAssigned.Username.ToLower().Trim(),
                        Link = $"https://ojtssapp1/smp/Identity/Account/Login?ReturnUrl={application}/engineer/worklist/detail/{request.Id}",
                        To = new List<SenderBody>
                        {
                            new SenderBody { Name = request.EngineerAssigned.Fullname, Address = request.EngineerAssigned.Email },
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
