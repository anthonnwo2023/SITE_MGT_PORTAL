﻿using Newtonsoft.Json;

namespace Project.V1.DLL.RequestActions
{
    public class RequestStateBase<T> where T : class
    {
        public virtual Task<bool> EnterState(IRequestAction<T> request, T requests, Dictionary<string, object> variables, RequestApproverModel ActionedBy)
        {
            return Task.FromResult(false);
        }

        public virtual Task<bool> EnterState(IRequestAction<T> request, List<T> requests, Dictionary<string, object> variables)
        {
            return Task.FromResult(false);
        }

        public virtual bool Approve(IRequestAction<T> request, T requests, Dictionary<string, object> variables)
        {
            Log.Information("Cannot approve this request. " + JsonConvert.SerializeObject(request));

            return false;
        }

        public virtual bool Update(IRequestAction<T> request, T requests, Dictionary<string, object> variables)
        {
            Log.Information("Cannot update this request. " + JsonConvert.SerializeObject(request));

            return false;
        }

        public virtual bool Complete(IRequestAction<T> request, T requests, Dictionary<string, object> variables)
        {
            Log.Information("Cannot complete this request. " + JsonConvert.SerializeObject(request));

            return false;
        }

        public virtual bool Disapprove(IRequestAction<T> request, T requests, Dictionary<string, object> variables, RequestApproverModel ActionedBy)
        {
            Log.Information("Cannot disapprove this request. " + JsonConvert.SerializeObject(request));

            return false;
        }

        public virtual bool Restart(IRequestAction<T> request, T requests, Dictionary<string, object> variables)
        {
            Log.Information("Cannot restart this request. " + JsonConvert.SerializeObject(request));

            return false;
        }

        public virtual bool Accept(IRequestAction<T> request, T requests, Dictionary<string, object> variables)
        {
            Log.Information("Cannot accept this request. " + JsonConvert.SerializeObject(request));

            return false;
        }

        public virtual bool Cancel(IRequestAction<T> request, T requests, Dictionary<string, object> variables)
        {
            Log.Information("Cannot cancel this request. " + JsonConvert.SerializeObject(request));

            return false;
        }

        public virtual bool Reject(IRequestAction<T> request, T requests, Dictionary<string, object> variables, string reason)
        {
            Log.Information("Cannot reject this request. " + JsonConvert.SerializeObject(request));

            return false;
        }

        public virtual bool Rework(IRequestAction<T> request, T requests, Dictionary<string, object> variables)
        {
            Log.Information("Cannot rework this request. " + JsonConvert.SerializeObject(request));

            return false;
        }

        public async Task SendNotification<T1>(T1 request, SendEmailActionObj emailObj, string role) where T1 : class, IDisposable
        {
            await HelperFunctions.SendEmailAction(request, emailObj, role);
        }

        public async Task SendNotification<T1>(List<T1> requests, SendEmailActionObj emailObj, string role, string requestType) where T1 : class, IDisposable
        {
            await HelperFunctions.SendEmailAction(requests, emailObj, role, requestType);
        }
    }
}
