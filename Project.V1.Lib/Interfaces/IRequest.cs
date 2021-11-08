using Project.V1.Lib.Services.Interfaces;
using Project.V1.Models;
using System;

namespace Project.V1.Lib.Interface
{
    public interface IRequest : IRequestAction<RequestViewModel>, IDisposable
    {
    }
}
