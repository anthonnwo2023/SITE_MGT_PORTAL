using Microsoft.EntityFrameworkCore;
using Project.V1.Data;
using Project.V1.Lib.Interfaces;
using Project.V1.DLL.Helpers;
using Project.V1.DLL.RequestActions;
using Project.V1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.V1.DLL.Services.Interfaces;

namespace Project.V1.Lib.Services
{
    public class BaseActionOps<T> : GenericRepo<T>,  IRequestAction<T> where T : RequestViewModel
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

        public void Accept(T request, Dictionary<string, object> variables) => _state.Accept(this, request, variables);

        public void Cancel(T request, Dictionary<string, object> variables) => _state.Cancel(this, request, variables);

        public void Reject(T request, Dictionary<string, object> variables, string reason) => _state.Reject(this, request, variables, reason);

        public void Rework(T request, Dictionary<string, object> variables) => _state.Rework(this, request, variables);

        public void SetTransitionState(RequestStateBase<T> newState)
        {
            _state = newState;
        }

        public void TransitionState(RequestStateBase<T> newState, T requests, Dictionary<string, object> variables)
        {
            _state = newState;
            _state.EnterState(this, requests, variables);
        }

        public void TransitionState(RequestStateBase<T> newState, List<T> requests, Dictionary<string, object> variables)
        {
            _state = newState;
            _state.EnterState(this, requests, variables);
        }

        private static async Task<RequestStateBase<T>> BuildState(T requestClass, string requestType)
        {
            return await Task.FromResult(Factory.CreateObject<RequestStateBase<T>, T>(requestClass.Status, requestType));
        }

        public async Task<bool> SetCreateState(T request, dynamic variables)
        {
            try
            {
                RequestStateBase<T> state = await BuildState(request, "");

                TransitionState(state, request, variables);

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

        public async Task SetState(T requestClass)
        {
            RequestStateBase<T> state = await BuildState(requestClass, "");

            await Task.Run(() => SetTransitionState(state));
        }
    }
}
