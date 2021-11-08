using System;
using System.Collections.Generic;

namespace Project.V1.Lib.Helpers
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

            Type type = Type.GetType("Project.V1.DLL.RequestActions." + state);

            return (T)Activator.CreateInstance(type.MakeGenericType(requestViewModel))!;
        }

        public static string ProcessRequestState(string requestStatus, string requestType)
        {
            Dictionary<string, Func<string>> Processor = new()
            {
                ["Pending"] = () =>
                {
                    return "PendingState`1";
                },

                ["Rejected"] = () =>
                {
                    return "RejectState`1";
                },

                ["Rework"] = () =>
                {
                    return "ReworkState`1";
                },

                ["Accepted"] = () =>
                {
                    return "AcceptState`1";
                },

                ["Completed"] = () =>
                {
                    return "CompleteState`1";
                }
            };

            return Processor[requestStatus]();
        }
    }
}
