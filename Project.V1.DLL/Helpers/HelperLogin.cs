namespace Project.V1.DLL.Helpers
{
    public static class HelperLogin
    {
        private static async Task<SignInResponse> ProcessSuccessSignInResult(string username, VendorModel Vendor, ApplicationUser user, ADUserDomainModel userADData,
            Microsoft.AspNetCore.Identity.SignInResult result)
        {
            if (!user.IsActive)
            {
                await LoginObject.SignInManager.SignOutAsync();
                Log.Information("Inactive user account. Signout completed ", new { username, Vendor = JsonSerializer.Serialize(Vendor) });

                return ExtractResponse(user, Microsoft.AspNetCore.Identity.SignInResult.NotAllowed, "Account is inactive. Please contact Switch Support Team");
            }

            Log.Information("Application Login successful. ", new { username, Vendor = JsonSerializer.Serialize(Vendor), UserADData = JsonSerializer.Serialize(userADData) });

            user.LastLoginDate = DateTime.Now;

            if (userADData != null && !user.IsADLoaded)
            {
                user.Email = userADData.Email;
                user.Fullname = userADData.Fullname.Split('[')[0];
                user.PhoneNumber = userADData.PhoneNo;
                user.Department = userADData.Department;
                user.JobTitle = userADData.Title;
                user.Department = userADData.Department;
                user.IsADLoaded = true;
                user.IsNewPassword = false;
                user.UserType = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("Internal"));
            }

            await LoginObject.UserManager.UpdateAsync(user);

            return ExtractResponse(user, result, "Application Login successful.", user.IsNewPassword);
        }

        public static async Task<SignInResponse> SignInNewUserWithAttributes(ApplicationUser newUser, string username, string password, VendorModel Vendor)
        {
            foreach (string roleName in newUser.Roles)
            {
                await LoginObject.User.ChangeUserRoleByName(newUser, roleName, true);
            }

            ApplicationUser user = await LoginObject.UserManager.FindByNameAsync(username);
            Microsoft.AspNetCore.Identity.SignInResult result = await LoginObject.SignInManager.PasswordSignInAsync(user, password, true, lockoutOnFailure: true);
            //await loginObject.SignInManager.SignInAsync(user, isPersistent: false);

            Log.Information("Application Login successful. ", new { username, Vendor = Vendor.Id });

            user.LastLoginDate = DateTime.Now;
            await LoginObject.UserManager.UpdateAsync(user);

            return ExtractResponse(newUser, result, "Application Login successful.");
        }

        public static async Task<SignInResponse> ProcessCreateUser(ApplicationUser newUser, string username, string password, VendorModel Vendor)
        {
            IdentityResult resultCreateUser = await LoginObject.UserManager.CreateAsync(newUser, password);

            if (resultCreateUser.Succeeded)
            {
                return await SignInNewUserWithAttributes(newUser, username, password, Vendor);
            }

            return ExtractResponse(newUser, Microsoft.AspNetCore.Identity.SignInResult.Failed, "Internal error occurred! Login failed.");
        }

        public static SignInResponse ExtractResponse(ApplicationUser user, Microsoft.AspNetCore.Identity.SignInResult result, string message, bool isNewPassword = false)
        {
            SignInResponse response = new()
            {
                User = user,
                UserType = user?.UserType,
                Result = result,
                Message = message,
                Roles = user?.Roles,
                IsNewPassword = isNewPassword,
            };

            return response;
        }

        public static async Task<SignInResponse> PerformSignInOp(string username, string vendorId, VendorModel Vendor, ApplicationUser user,
            Microsoft.AspNetCore.Identity.SignInResult result, ADUserDomainModel userADData)

        {
            Log.Information("Performing SignIn Operation. ", new { username, Vendor = Vendor.Id, UserADData = JsonSerializer.Serialize(userADData) });

            Dictionary<Microsoft.AspNetCore.Identity.SignInResult, Func<string, string, VendorModel, ApplicationUser, Microsoft.AspNetCore.Identity.SignInResult, ADUserDomainModel, Task<SignInResponse>>> operations = new()
            {
                [Microsoft.AspNetCore.Identity.SignInResult.Success] = async (username, vendorId, Vendor, user, result, userADData) =>
                {
                    return await ProcessSuccessSignInResult(username, Vendor, user, userADData, result);
                },

                [Microsoft.AspNetCore.Identity.SignInResult.TwoFactorRequired] = async (username, vendorId, Vendor, user, result, userADData) =>
                {
                    user.LastLoginDate = DateTime.Now;
                    _ = LoginObject.UserManager.UpdateAsync(user).Result;
                    return await Task.Run(() => ExtractResponse(user, result, "Login attempt requires 2FA."));
                },

                [Microsoft.AspNetCore.Identity.SignInResult.LockedOut] = async (username, vendorId, Vendor, user, result, userADData) =>
                {
                    Log.Information("Account is Locked out. ", new { username, Vendor = Vendor.Id, UserADData = JsonSerializer.Serialize(userADData) });
                    return await Task.Run(() => ExtractResponse(user, result, "Account is Locked Out."));
                },

                [Microsoft.AspNetCore.Identity.SignInResult.NotAllowed] = async (username, vendorId, Vendor, user, result, userADData) =>
                {
                    Log.Information("Unauthorized Login Attempt.", new { username, Vendor = Vendor.Id, UserADData = JsonSerializer.Serialize(userADData) });
                    return await Task.Run(() => ExtractResponse(user, result, "Unauthorized Login Attempt."));
                },

                [Microsoft.AspNetCore.Identity.SignInResult.Failed] = async (username, vendorId, Vendor, user, result, userADData) =>
                {
                    Log.Information("Invalid Login Attempt.", new { username, Vendor = Vendor.Id, UserADData = JsonSerializer.Serialize(userADData) });
                    return await Task.Run(() => ExtractResponse(user, Microsoft.AspNetCore.Identity.SignInResult.Failed, "Invalid login attempt."));
                }
            };

            return await operations[result].Invoke(username, vendorId, Vendor, user, result, userADData);
        }

        public static string GenerateApiAccessKey(string username, string password)
        {
            return $"{WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(username))}/APIKEY/{WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes($"Patrick{DateTime.Now.ToShortTimeString()}"))}!//!!{WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(password))}";
        }
    }
}
