using Project.V1.DLL.Services.Interfaces;
using Project.V1.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.V1.DLL.Interface
{
    public interface IRequest : IRequestAction<RequestViewModel>, IDisposable
    {
        Task<bool> GetValidRequest(RequestViewModel item);
        Task<(IEnumerable<RequestViewModel> Valid, IEnumerable<RequestViewModel> Invalid)> GetValidRequests(IEnumerable<RequestViewModel> items);
    }
}
