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
        bool TransitionState(RequestStateBase<T> requestBase, T requests, Dictionary<string, object> variables);
        void SetTransitionState(RequestStateBase<T> newState);

        bool Accept(T request, Dictionary<string, object> variables);
        bool Cancel(T request, Dictionary<string, object> variables);
        bool Reject(T request, Dictionary<string, object> variables, string reason);
        bool Rework(T request, Dictionary<string, object> variables);

        Task SetState(T requestClass);
        Task<bool> SetCreateState(T requestClass, dynamic variables);
        Task<bool> SetCreateState(List<T> requestClass, dynamic variables);
    }
}
