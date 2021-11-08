using Microsoft.AspNetCore.Components;
using Project.V1.Models;
using System.Threading.Tasks;

namespace Project.V1.Web.Pages.Components
{
    public partial class DisplayClaimCheckbox
    {
        [Parameter] public ClaimViewModel Claim { get; set; }
        [Parameter] public int Index { get; set; }
        [Parameter] public EventCallback<bool> OnClaimSelection { get; set; }

        protected async Task CheckboxChanged(ChangeEventArgs e)
        {
            //Claim.IsSelected = false;
            await OnClaimSelection.InvokeAsync((bool)e.Value);
            if ((bool)e.Value)
            {
                Claim.IsSelected = true;
            }
            else
            {
                Claim.IsSelected = false;
            }
        }
    }
}
