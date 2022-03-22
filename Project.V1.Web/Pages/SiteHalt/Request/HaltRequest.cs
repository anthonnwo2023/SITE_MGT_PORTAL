using Project.V1.DLL.Services.Interfaces.SiteHalt;

namespace Project.V1.Web.Pages.SiteHalt.Request
{
    public class HaltRequest : SiteHaltRequestModel
    {
        private readonly IHUDRequest _request;

        public HaltRequest(IHUDRequest request, ApplicationUser user)
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
                return (true, $"Halt Request Submitted successfully {SiteIds}.");
            }

            return (false, $"Halt Request could not be created. {errorMsg}");
        }
    }
}
