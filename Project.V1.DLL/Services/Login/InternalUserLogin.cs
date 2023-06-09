﻿using Microsoft.AspNetCore.WebUtilities;
using Project.V1.DLL.Helpers;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json;

namespace Project.V1.Lib.Services.Login
{

    [SupportedOSPlatform("windows")]
    public class InternalUserLogin : IUserLoginType
    {
        public async Task<SignInResponse> DoLogin(string username, string password, string vendorId)
        {
            //Log.Logger = HelperFunctions.GetSerilogLogger();

            Log.Information("Internal login process. ", new { Username = username, Vendor = vendorId, UserType = "Internal" });
            VendorModel Vendor = (await LoginObject.Vendor.GetById(x => x.Id == vendorId && x.Name.ToUpper() == "MTN NIGERIA"));

            if (Vendor == null)
            {
                Log.Information("Inactive vendor selected.", new { username, Vendor = vendorId, VendorData = JsonSerializer.Serialize(Vendor) });
                return HelperLogin.ExtractResponse(null, Microsoft.AspNetCore.Identity.SignInResult.Failed, "Invalid vendor selected.");
            }

            string doADLogin = ADHelper.Auth_user(username, password, "MTN");

            if (doADLogin.ToLower() == "true")
            {
                try
                {
                    ADUserDomainModel userADData = ADHelper.GetADUserData(username);

                    if (userADData == null)
                    {
                        Log.Information("Missing AD data after successful login.", new { username, Vendor = Vendor.Id, StackTrace = new NullReferenceException("Missing AD data") });
                        return HelperLogin.ExtractResponse(null, Microsoft.AspNetCore.Identity.SignInResult.Failed, "Internal error occurred! Login failed.");
                    }

                    userADData.Username = username;
                    userADData.Password = password;
                    userADData.Vendor = Vendor;

                    Log.Information("AD User Login validation successful. ", new { username, Vendor = Vendor.Id, UserADData = JsonSerializer.Serialize(userADData) });
                    ApplicationUser user = await LoginObject.UserManager.Users.Include(x => x.Vendor).FirstOrDefaultAsync(x => x.UserName.ToLower() == username.ToLower());

                    if (user != null)
                    {
                        if (!user.IsActive)
                        {
                            Log.Information("User Account Deleted. Contact #TSS", new { username, Vendor = Vendor.Id });
                            return HelperLogin.ExtractResponse(null, Microsoft.AspNetCore.Identity.SignInResult.NotAllowed, "User Account Deleted. Contact TSS");
                        }

                        Log.Information("AD User AppData. ", new { username, Vendor = vendorId });

                        user.Roles = (await LoginObject.User.GetUserRoles(user)).ToArray();
                        List<ClaimListManager> userClaims = (LoginObject.ClaimService.Get(y => y.IsActive, null, "Category").GetAwaiter().GetResult()).Where(z => z.Category.Name == "Project").GroupBy(v => v.Category.Name).Select(u => new ClaimListManager
                        {
                            Category = u.Key,
                            Claims = u.ToList().FormatClaimSelection(user)
                        }).ToList();

                        user.Projects = userClaims.SelectMany(x => x.Claims).Where(x => x.IsSelected).ToList();
                        await LoginObject.SignInManager.SignInAsync(user, true);

                        return await ProcessSignInResultOldUser(username, vendorId, Vendor, user, Microsoft.AspNetCore.Identity.SignInResult.Success, userADData);
                    }

                    ApplicationUser newUser = new()
                    {
                        Department = userADData.Department,
                        JobTitle = userADData.Title,
                        UserName = userADData.Username,
                        Email = userADData.Email,
                        Fullname = userADData.Fullname.Split('[')[0].Trim(),
                        PhoneNumber = userADData.PhoneNo,
                        Vendor = userADData.Vendor,
                        IsActive = true,
                        DateCreated = DateTime.Now,
                        LastLoginDate = DateTime.Now,
                        UserType = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("Internal")),
                        IsADLoaded = true,
                        IsNewPassword = false,
                    };

                    newUser.Roles = new string[] { "User" };
                    return await HelperLogin.ProcessCreateUser(newUser, username, "0004Fa1c-H!!3f7-47f4-9e58-25c9001d5426", Vendor);
                    //Log.Information("Unauthorized Login Attempt.", new { username, Vendor = Vendor.Id });
                    //return HelperLogin.ExtractResponse(null, SignInResult.NotAllowed, "Unauthorized Login Attempt.");
                }
                catch (Exception ex)
                {
                    Log.Error("Internal error occurred", new { ex.Message, StactTrace = ex.StackTrace });
                    return HelperLogin.ExtractResponse(null, Microsoft.AspNetCore.Identity.SignInResult.Failed, "Internal error occurred.");
                }
            }

            Log.Information("Invalid login attempt.", new { username, Vendor = Vendor.Id });
            return HelperLogin.ExtractResponse(null, Microsoft.AspNetCore.Identity.SignInResult.Failed, "Invalid login attempt.");
        }

        private static async Task<SignInResponse> ProcessSignInResultOldUser(string username, string vendorId, VendorModel MTN_Vendor, ApplicationUser user,
            Microsoft.AspNetCore.Identity.SignInResult result, ADUserDomainModel userADData)
        {

            if (user.Vendor.Id != vendorId)
            {
                return HelperLogin.ExtractResponse(null, Microsoft.AspNetCore.Identity.SignInResult.Failed, "Invalid login attempt.");
            }

            return await HelperLogin.PerformSignInOp(username, vendorId, MTN_Vendor, user, result, userADData);
        }
    }
}