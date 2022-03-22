using Project.V1.DLL.RequestActions;
using Project.V1.Lib.Interfaces;
using Project.V1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.V1.DLL.Services.Interfaces
{
    public interface IRequestAction<T> : IGenericRepo<T> where T : class
    {
        bool TransitionState(RequestStateBase<T> requestBase, T requests, Dictionary<string, object> variables, RequestApproverModel ActionedBy);
        void SetTransitionState(RequestStateBase<T> newState);

        bool Approve(T request, Dictionary<string, object> variables);
        bool Disapprove(T request, Dictionary<string, object> variables, RequestApproverModel ActionedBy);
        bool Update(T request, Dictionary<string, object> variables);
        bool Accept(T request, Dictionary<string, object> variables);
        bool Cancel(T request, Dictionary<string, object> variables);
        bool Reject(T request, Dictionary<string, object> variables, string reason);
        bool Rework(T request, Dictionary<string, object> variables);

        Task SetState(T requestClass, string folder = null);
        Task<bool> SetCreateState(T requestClass, dynamic variables, string folder, RequestApproverModel ActionedBy);
        Task<bool> SetCreateState(List<T> requestClass, dynamic variables);
        bool Complete(T request, Dictionary<string, object> variables);
    }
}
