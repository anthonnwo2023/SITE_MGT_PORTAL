using Microsoft.EntityFrameworkCore;
using Project.V1.Data;
using Project.V1.DLL.Helpers;
using Project.V1.DLL.RequestActions;
using Project.V1.DLL.Services.Interfaces;
using Project.V1.Lib.Interfaces;
using Project.V1.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.V1.Lib.Services
{
    public class BaseActionOps<T> : GenericRepo<T>, IRequestAction<T> where T : class, new()
    {
        private RequestStateBase<T> _state;
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _entities;
        private readonly string _KeyString;
        private readonly ICLogger _logger;

        public BaseActionOps(ApplicationDbContext context, string KeyString, ICLogger logger)
            : base(context, KeyString)
        {
            _KeyString = KeyString;
            _logger = logger;
            _context = context;
            _entities = context.Set<T>();
        }

        public bool Approve(T request, Dictionary<string, object> variables) => _state.Approve(this, request, variables);

        public bool Update(T request, Dictionary<string, object> variables) => _state.Update(this, request, variables);

        public bool Disapprove(T request, Dictionary<string, object> variables, RequestApproverModel ActionedBy) => _state.Disapprove(this, request, variables, ActionedBy);

        public bool Complete(T request, Dictionary<string, object> variables) => _state.Complete(this, request, variables);

        public bool Accept(T request, Dictionary<string, object> variables) => _state.Accept(this, request, variables);

        public bool Cancel(T request, Dictionary<string, object> variables) => _state.Cancel(this, request, variables);

        public bool Reject(T request, Dictionary<string, object> variables, string reason) => _state.Reject(this, request, variables, reason);

        public bool Rework(T request, Dictionary<string, object> variables) => _state.Rework(this, request, variables);

        public bool Restart(T request, Dictionary<string, object> variables) => _state.Restart(this, request, variables);

        public void SetTransitionState(RequestStateBase<T> newState)
        {
            _state = newState;
        }

        public bool TransitionState(RequestStateBase<T> newState, T requests, Dictionary<string, object> variables, RequestApproverModel ActionedBy)
        {
            _state = newState;
            return _state.EnterState(this, requests, variables, ActionedBy).GetAwaiter().GetResult();
        }

        public void TransitionState(RequestStateBase<T> newState, List<T> requests, Dictionary<string, object> variables)
        {
            _state = newState;
            _state.EnterState(this, requests, variables);
        }

        private static async Task<RequestStateBase<T>> BuildState(T requestClass, string requestType, string folder = null)
        {
            return await Task.FromResult(Factory.CreateObject<RequestStateBase<T>, T>(requestClass, requestType, folder));
        }

        public async Task<bool> SetCreateState(T request, dynamic variables, string folder = null, RequestApproverModel ActionedBy = null)
        {
            try
            {
                RequestStateBase<T> state = await BuildState(request, "", folder);

                TransitionState(state, request, variables, ActionedBy);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SetCreateState(List<T> request, dynamic variables)
        {
            try
            {
                RequestStateBase<T> state = await BuildState(request.First(), "Bulk");

                TransitionState(state, request, variables);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task SetState(T requestClass, string folder = null)
        {
            RequestStateBase<T> state = await BuildState(requestClass, "", folder);

            await Task.Run(() => SetTransitionState(state));
        }
    }
}
