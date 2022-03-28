namespace Project.V1.Web.Pages.SiteHalt.Request
{
    public class HUDRequest : SiteHUDRequestModel
    {
        private readonly IHUDRequest _request;

        public HUDRequest(IHUDRequest request, ApplicationUser user)
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
                return (true, $"{RequestAction} Request Submitted successfully for: {((HasLargeSiteIdCount) ? TextFileExtension.Initialize("HUD_SiteID", SiteIds).ReadFromFile() : SiteIds)}.");
            }

            return (false, $"{RequestAction} Request could not be created. {errorMsg}");
        }

        public override async Task SetCreateState(RequestApproverModel ActionedBy = null)
        {
            await _request.SetCreateState(this, Variables, "SiteHalt", ActionedBy);
        }
    }
}
