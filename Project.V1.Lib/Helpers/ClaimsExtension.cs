using Project.V1.Models;
using System.Collections.Generic;

namespace Project.V1.Lib.Helpers
{
    public static class ClaimsExtension
    {
        public static List<ClaimViewModel> FormatClaimSelection(this List<ClaimViewModel> allRoleClaims)
        {
            allRoleClaims.ForEach(item =>
            {
                item.IsSelected = false;
            });

            return allRoleClaims;
        }
    }
}
