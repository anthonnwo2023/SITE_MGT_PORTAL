using Project.V1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.V1.DLL.Helpers
{
    public static class ClaimsExtension
    {
        public static async Task<List<ClaimViewModel>> FormatClaimSelection(this List<ClaimViewModel> allRoleClaims)
        {
            allRoleClaims.ForEach(item =>
            {
                item.IsSelected = false;
            });

            return await Task.FromResult(allRoleClaims);
        }
    }
}
