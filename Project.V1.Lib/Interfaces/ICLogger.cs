using System;

namespace Project.V1.Lib.Interfaces
{
    public interface ICLogger
    {
        void LogDebug(string message, object obj);
        void LogError(string message, object obj, Exception ex = null);
        void LogFatal(string message, object obj, Exception ex);
        void LogInformation(string message, object obj);
        void LogWarning(string message, object obj);
    }
}
