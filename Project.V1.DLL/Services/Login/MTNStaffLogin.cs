using System.Runtime.Versioning;

namespace Project.V1.Lib.Services.Login
{
    [SupportedOSPlatform("windows")]
    public class MTNStaffLogin : UserLogin
    {
        public MTNStaffLogin() : base(new InternalUserLogin()) { }
    }
}
