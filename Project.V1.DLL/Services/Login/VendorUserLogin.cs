using System.Runtime.Versioning;

namespace Project.V1.Lib.Services.Login
{
    [SupportedOSPlatform("windows")]
    public class VendorUserLogin : UserLogin
    {
        public VendorUserLogin() : base(new ExternalUserLogin()) { }
    }
}
