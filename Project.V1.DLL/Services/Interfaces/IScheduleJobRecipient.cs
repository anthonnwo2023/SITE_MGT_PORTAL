using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.V1.DLL.Services.Interfaces
{
    public interface IScheduleJobRecipient : IGenericRepo<ScheduleJobRecipientModel>, IDisposable
    {
        Task<ScheduleJobRecipientModel> GetByRecipientId(string Id);
    }
}
