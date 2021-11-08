using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.V1.Lib.Helpers
{
    public class AppState
    {
        public string RejectedWorklistCount { get; private set; }
        public string EngineerWorklistCount { get; private set; }

        public event Action OnChange;

        public AppState(IRequest request)
        {
        }

        public void SetRejectedCount(int count)
        {
            RejectRequestCount = (await IRequest.Get(x => x.Requester.Username == Principal.Identity.Name && x.Status == "Rejected")).Count();
            RegionWIPRequestCount = (await IRequest.Get(x => x.RegionId == User.RegionId && x.Status != "Rejected" && x.Status != "Completed")).Count();

            RejectedWorklistCount = count.ToString();
            NotifyStateChanged();
        }

        public void SetWorklistCount(int count)
        {
            EngineerWorklistCount = count.ToString();
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
