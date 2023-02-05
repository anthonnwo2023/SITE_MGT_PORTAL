using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.V1.DLL.Services
{

    [Authorize]
    public class ScheduleJobRecipient : GenericRepo<ScheduleJobRecipientModel>, IScheduleJobRecipient, IDisposable
    {
        private readonly ApplicationDbContext context;

        public ScheduleJobRecipient(ApplicationDbContext context, ICLogger logger)
            : base(context, null, logger)
        {
            this.context = context;
        }

        public async Task<ScheduleJobRecipientModel> GetByRecipientId(string Id)
        {
            try
            {
                return await context.ScheduleJobRecipientModel.FirstOrDefaultAsync(x => x.Id == Id);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }


        }
    }

}
