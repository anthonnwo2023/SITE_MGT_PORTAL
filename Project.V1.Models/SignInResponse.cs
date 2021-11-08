using Microsoft.AspNetCore.Identity;

namespace Project.V1.Models
{
    public class SignInResponse
    {
        public string UserType { get; set; } = "";

        public string Message { get; set; }

        public SignInResult Result { get; set; }

        public string[] Roles { get; set; } = null;

        public bool IsNewPassword { get; set; } = false;
    }
}
