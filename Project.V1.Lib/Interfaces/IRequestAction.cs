using Project.V1.Lib.Interfaces;
using Project.V1.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.V1.Lib.Services.Interfaces
{
    public interface IRequestAction<T> : IGenericRepo<T> where T : RequestViewModel, IDisposable
    {
        void TransitionState(RequestStateBase<T> requestBase, T requests, Dictionary<string, object> variables);
        void SetTransitionState(RequestStateBase<T> newState);

        //void Disapprove(T reques, Dictionary<string, object> variablest, string reason);
        void Complete(T request, Dictionary<string, object> variables);

        Task<bool> CompleteRequest(T requestObj);
        Task<bool> CreateRequest(T requests);
        Task<T> GetByIdAsync(string Id);
        //IIncludableQueryable<T, RequestApproverModel> GetAllAsync();
        Task SetState(T requestClass);
        Task<bool> SetCreateState(T requestClass, dynamic variables);
        Task<bool> CreateBulkRequest(List<T> requestObjs);
    }
}
