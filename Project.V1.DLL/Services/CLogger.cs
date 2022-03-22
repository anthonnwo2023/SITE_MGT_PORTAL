using Microsoft.AspNetCore.Http;
using Project.V1.Lib.Helpers;
using Project.V1.Lib.Interfaces;
using Serilog;
using System;

namespace Project.V1.Lib.Services
{
    public class CLogger : ICLogger
    {
        private string LoggedInUser { get; set; }

        public CLogger()
        {
            HttpContextAccessor httpContextAccessor = new();
            LoggedInUser = httpContextAccessor.HttpContext?.User.Identity.Name;

            Log.Logger = HelperFunctions.GetSerilogLogger();
        }

        public void LogInformation(string message, object obj)
        {
            var newObj = new { obj, LoggedInUser };
            Log.Information($"{message} {@newObj}");
        }

        public void LogError(string message, object obj, Exception ex)
        {
            var newObj = new { obj, LoggedInUser };
            Log.Error(ex, message, @newObj);
        }

        public void LogFatal(string message, object obj, Exception ex)
        {
            var newObj = new { obj, LoggedInUser };
            Log.Fatal(ex, message, @newObj);
        }

        public void LogDebug(string message, object obj)
        {
            var newObj = new { obj, LoggedInUser };
            Log.Debug($"{message} {@newObj}");
        }

        public void LogWarning(string message, object obj)
        {
            var newObj = new { obj, LoggedInUser };
            Log.Warning($"{message} {@newObj}");
        }
    }
}
