using Project.V1.DLL.Services.Interfaces;
using Project.V1.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project.V1.DLL.RequestActions
{
    public class CompletedState<T> : RequestStateBase<T> where T : RequestViewModel, IDisposable
    {
        public override async Task EnterState(IRequestAction<T> _request, T request, Dictionary<string, object> variables)
        {
            IUser _user = variables["IUser"] as IUser;
            ClaimsPrincipal User = variables["User"] as ClaimsPrincipal;
            ApplicationUser engineer = variables["engineer"] as ApplicationUser;

            await _request.UpdateRequest(request, x => x.Id == request.Id);

            await SendEmail(User, _user, request, engineer);
        }

        public async Task SendEmail(ClaimsPrincipal User, IUser _user, T requests, ApplicationUser engineer)
        {
            ApplicationUser user = await _user.GetUserByUsername(User.Identity.Name);

            SendEmailActionObj emailObj = GenerateMailBody("Requester", requests, user, engineer);
            await SendNotification<T>(requests, emailObj, "");

            emailObj = GenerateMailBody("", requests, user, engineer);
            await SendNotification(requests, emailObj, "");
        }

        private static SendEmailActionObj GenerateMailBody(string mailType, T requests, ApplicationUser user, ApplicationUser engineer)
        {
            Dictionary<string, Func<SendEmailActionObj>> processMailBody = new()
            {
                ["Requester"] = () =>
                {
                    return new SendEmailActionObj
                    {
                        Name = "Hello " + requests.Requester.Name,
                        Title = "Notification of Request Completion - See Below Request Details",
                        Greetings = $"NWG NAPO Site Acceptance Request : <font color='green'><b>Your Request has been completed</b></font> - See Details below:",
                        Comment = "", //requests.EngineerAssigned.ApproverComment,
                        BodyType = "",
                        M2Uname = engineer.UserName.ToLower().Trim(),
                        Link = "https://ojtssapp1/ssrequest/Identity/Account/Login?ReturnUrl=%2Fssrequest/Request/Details?id=" + requests.Id,
                        To = new List<SenderBody> {
                            new SenderBody { Name = requests.Requester.Name, Address = requests.Requester.Email },
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
                        Name = "Hello " + user.Fullname,
                        Title = "Notification of Request Completion - See Below Request Details",
                        Greetings = $"NWG NAPO Site Acceptance Request : <font color='green'><b>Request completed successfully</b></font> - See Details below:",
                        Comment = "", //requests.EngineerAssigned.ApproverComment,
                        BodyType = "",
                        M2Uname = user.UserName.ToLower().Trim(),
                        Link = "https://ojtssapp1/ssrequest/Identity/Account/Login?ReturnUrl=%2Fssrequest/Request/Engineer/Report/Details?id=" + requests.Id,
                        To = new List<SenderBody> {
                            new SenderBody { Name = user.Fullname, Address = user.Email },
                        },
                        CC = new List<SenderBody> {
                            //new SenderBody { Name = "#NWG IP Security", Address = "#switchsupport.ng@mtn.com" },
                            new SenderBody { Name = "Adekunle Adeyemi", Address = "Adekunle.Adeyemi@mtn.com" },
                        }
                    };
                }
            };

            return processMailBody[mailType].Invoke();
        }
    }
}
