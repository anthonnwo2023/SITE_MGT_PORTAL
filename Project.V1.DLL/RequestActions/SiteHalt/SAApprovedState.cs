﻿using Project.V1.DLL.Services.Interfaces;
using Project.V1.Models;
using Project.V1.Models.SiteHalt;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

            emailObj = GenerateMailBody("SAApprover", request, application);
            await SendNotification(request, emailObj, "SAApprover");

            emailObj = GenerateMailBody("TAApprover", request, application);
            await SendNotification(request, emailObj, "TAApprover");
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
                        Greetings = $"HUD {(request as dynamic).RequestAction} Request : <font color='orange'><b>Request Approved by ({(request as dynamic).SecondApprover.Fullname})</b></font>, awaiting next approval - See Details below:",
                        Comment = (request as dynamic).SecondApprover.ApproverComment,
                        Subject = ($"Halt | Unhalt | Decomission (HUD) {(request as dynamic).RequestAction} Request: {((dynamic)request).UniqueId} Update Notice"),
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
                        Name = "Hello " + (request as dynamic).SecondApprover.Fullname,
                        Title = "Update Notification on Request - See Below Request Details",
                        Greetings = $"HUD {(request as dynamic).RequestAction} Request : <font color='orange'><b>Request Approved</b></font> - See Details below:",
                        Comment = (request as dynamic).SecondApprover.ApproverComment,
                        Subject = ($"Halt | Unhalt | Decomission (HUD) {(request as dynamic).RequestAction} Request: {((dynamic)request).UniqueId} Update Notice"),
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
                        Name = "Hello " + (request as dynamic).ThirdApprover.Fullname,
                        Title = "Update Notification on Request - See Below Request Details",
                        Greetings = $"HUD {(request as dynamic).RequestAction} Request : <font color='orange'><b>Request Approved by ({(request as dynamic).SecondApprover.Fullname})</b></font>, but awaiting your approval - See Details below:",
                        Comment = (request as dynamic).SecondApprover.ApproverComment,
                        Subject = ($"Halt | Unhalt | Decomission (HUD) {(request as dynamic).RequestAction} Request: {((dynamic)request).UniqueId} Approval Notice"),
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