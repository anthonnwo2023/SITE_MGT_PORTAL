using Project.V1.DLL.Services.Interfaces;
using Project.V1.Models;
using System;

namespace Project.V1.DLL.Interface
{
    public interface IRequest : IRequestAction<RequestViewModel>, IDisposable
    {
    }
}
