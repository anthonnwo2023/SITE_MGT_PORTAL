using Project.V1.DLL.Helpers;
using Project.V1.DLL.Services.Interfaces;
using Project.V1.Models;
using Project.V1.Models.SiteHalt;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.V1.DLL.RequestActions.SiteHalt
{
    public class HaltedState<T> : RequestStateBase<T> where T : SiteHaltRequestModel, IDisposable
    {
        public override bool Approve(IRequestAction<T> request, T requests, Dictionary<string, object> variables)
        {
            return request.TransitionState(new ApprovedState<T>(), requests, variables);
        }

        public override bool Disapprove(IRequestAction<T> request, T requests, Dictionary<string, object> variables)
        {
            return request.TransitionState(new DisapprovedState<T>(), requests, variables);
        }

        public override async Task<bool> EnterState(IRequestAction<T> _request, T requests, Dictionary<string, object> variables)
        {
            try
            {
                string application = variables["App"] as string;

                await SendEmail(application, requests);

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

            SendEmailActionObj emailObj = await GenerateMailBody("Requester", request, user, application);
            await SendNotification(request, emailObj, "");

            emailObj = await GenerateMailBody("RFTeam", request, user, application);
            await SendNotification(request, emailObj, "RFTeam");
        }

        private static async Task<SendEmailActionObj> GenerateMailBody(string mailType, T request, ApplicationUser user, string application)
        {
            Dictionary<string, Func<SendEmailActionObj>> processMailBody = new()
            {
                ["Requester"] = () =>
                {
                    var emailObj = new SendEmailActionObj
                    {
                    };

                    return emailObj;
                },

                ["RFTeam"] = () =>
                {
                    var emailObj = new SendEmailActionObj
                    {
                    };

                    return emailObj;
                }
            };

            return processMailBody[mailType].Invoke();
        }
    }
}
