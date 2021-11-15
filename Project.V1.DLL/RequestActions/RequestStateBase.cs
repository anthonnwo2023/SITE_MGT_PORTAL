using Newtonsoft.Json;
using Project.V1.DLL.Services.Interfaces;
using Project.V1.Lib.Helpers;
using Project.V1.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Project.V1.DLL.RequestActions
{
    public class RequestStateBase<T> where T : RequestViewModel, IDisposable
    {
        public virtual Task EnterState(IRequestAction<T> request, T requests, Dictionary<string, object> variables)
        {
            return Task.CompletedTask;
        }

        public virtual Task EnterState(IRequestAction<T> request, List<T> requests, Dictionary<string, object> variables)
        {
            return Task.CompletedTask;
        }

        public virtual void Accept(IRequestAction<T> request, T requests, Dictionary<string, object> variables)
        {
            Log.Information("Cannot accept this request. " + JsonConvert.SerializeObject(request));
        }

        public virtual void Cancel(IRequestAction<T> request, T requests, Dictionary<string, object> variables)
        {
            Log.Information("Cannot cancel this request. " + JsonConvert.SerializeObject(request));
        }

        public virtual void Reject(IRequestAction<T> request, T requests, Dictionary<string, object> variables, string reason)
        {
            Log.Information("Cannot reject this request. " + JsonConvert.SerializeObject(request));
        }

        public virtual void Rework(IRequestAction<T> request, T requests, Dictionary<string, object> variables)
        {
            Log.Information("Cannot rework this request. " + JsonConvert.SerializeObject(request));
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
