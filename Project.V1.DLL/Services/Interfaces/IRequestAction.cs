using Project.V1.DLL.RequestActions;
using Project.V1.Lib.Interfaces;
using Project.V1.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.V1.DLL.Services.Interfaces
{
    public interface IRequestAction<T> : IGenericRepo<T> where T : RequestViewModel, IDisposable
    {
        void TransitionState(RequestStateBase<T> requestBase, T requests, Dictionary<string, object> variables);
        void SetTransitionState(RequestStateBase<T> newState);

        void Accept(T request, Dictionary<string, object> variables);
        void Cancel(T request, Dictionary<string, object> variables);
        void Reject(T request, Dictionary<string, object> variables, string reason);
        void Rework(T request, Dictionary<string, object> variables);

        Task SetState(T requestClass);
        Task<bool> SetCreateState(T requestClass, dynamic variables);
        Task<bool> SetCreateState(List<T> requestClass, dynamic variables);
    }
}
