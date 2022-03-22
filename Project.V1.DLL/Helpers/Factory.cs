using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Project.V1.DLL.Helpers
{
    public class Factory
    {
        public static T CreateObject<T>() where T : new()
        {
            return new T();
        }

        public static T CreateObject<T, U>(dynamic request, string requestType, string folder = null) where T : new() where U : class
        {
            string state = ProcessRequestState(request, requestType, folder);

            Type[] AssemblyTypes = Assembly.GetExecutingAssembly().GetTypes();
            Type type = AssemblyTypes.FirstOrDefault(x => x.FullName.Contains(state));

            Type requestModel = typeof(U);

            //Type type = Type.GetType("Project.V1.DLL.RequestActions." + state)!;

            return (T)Activator.CreateInstance(type.MakeGenericType(requestModel))!;
        }

        public static string ProcessRequestState(dynamic request, string requestType, string folder)
        {
            var requestStatus = (request as dynamic).Status;

            Dictionary<string, Func<string, string>> Processor = new()
            {
                ["Pending"] = (requestType) =>
                {
                    if (requestType == "Bulk")
                        return $"{folder}.PendingBulkState`1";

                    if (folder == "SiteHalt" && (request as dynamic).RequestAction == "UnHalt")
                        return $"{folder}.TAApprovedState`1";

                    return $"DLL.RequestActions.{folder}.PendingState`1".Replace("..", ".");
                },

                ["Rejected"] = (requestType) =>
                {
                    return $"{folder}.RejectedState`1";
                },

                ["Reworked"] = (requestType) =>
                {
                    return $"{folder}.ReworkedState`1";
                },

                ["Accepted"] = (requestType) =>
                {
                    return $"{folder}.AcceptedState`1";
                },

                ["Cancelled"] = (requestType) =>
                {
                    return $"{folder}.CancelledState`1";
                },

                ["Restarted"] = (requestType) =>
                {
                    return $"{folder}.RestartedState`1";
                },

                ["FAApproved"] = (requestType) =>
                {
                    return $"{folder}.RFSMApprovedState`1";
                },

                ["SAApproved"] = (requestType) =>
                {
                    return $"{folder}.SAApprovedState`1";
                },

                ["TAApproved"] = (requestType) =>
                {
                    return $"{folder}.TAApprovedState`1";
                },

                ["Disapproved"] = (requestType) =>
                {
                    return $"{folder}.DisapprovedState`1";
                },

                ["Completed"] = (requestType) =>
                {
                    return $"{folder}.CompletedState`1";
                }
            };

            return Processor[requestStatus](requestType);
        }
    }
}
