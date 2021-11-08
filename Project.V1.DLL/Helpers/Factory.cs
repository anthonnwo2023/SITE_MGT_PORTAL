using System;
using System.Collections.Generic;

namespace Project.V1.DLL.Helpers
{
    public class Factory
    {
        public static T CreateObject<T>() where T : new()
        {
            return new T();
        }

        public static T CreateObject<T, U>(string status, string requestType) where T : new() where U : class
        {
            string state = ProcessRequestState(status, requestType);

            Type requestViewModel = typeof(U);

            Type type = Type.GetType("Project.V1.DLL.RequestActions." + state)!;

            return (T)Activator.CreateInstance(type.MakeGenericType(requestViewModel))!;
        }

        public static string ProcessRequestState(string requestStatus, string requestType)
        {
            Dictionary<string, Func<string, string>> Processor = new()
            {
                ["Pending"] = (requestType) =>
                {
                    if (requestType == "Bulk")
                        return "PendingBulkState`1";

                    return "PendingState`1";
                },

                ["Rejected"] = (requestType) =>
                {
                    return "RejectedState`1";
                },

                ["Reworked"] = (requestType) =>
                {
                    return "ReworkedState`1";
                },

                ["Accepted"] = (requestType) =>
                {
                    return "AcceptedState`1";
                }
            };

            return Processor[requestStatus](requestType);
        }
    }
}
