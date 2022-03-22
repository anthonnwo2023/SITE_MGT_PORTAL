using Project.V1.DLL.Services.Interfaces.SiteHalt;

namespace Project.V1.Web.Pages.SiteHalt.Request
{
    public class DecomRequest : SiteHaltRequestModel
    {
        private readonly IHUDRequest _request;

        public DecomRequest(IHUDRequest request, ApplicationUser user)
        {
            _request = request;
            User = user;
        }

        public override async Task<(bool, string)> Create()
        {
            await base.Create();

            var (isCreated, errorMsg) = await _request.CreateRequest(this);

            if (isCreated)
            {
                return (true, $"Decomission Request Submitted successfully {SiteIds}.");
            }

            return (false, $"Decomission Request could not be created. {errorMsg}");
        }
    }
}