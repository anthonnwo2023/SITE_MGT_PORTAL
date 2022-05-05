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

            Type type = GetStateType(state);
            Type requestModel = typeof(U);

            return CreateInstance<T>(type, requestModel);
        }

        public static Type[] GetAssemblyTypes()
        {
            return Assembly.GetExecutingAssembly().GetTypes();
        }

        public static Type GetStateType(string state)
        {
            Type[] AssemblyTypes = GetAssemblyTypes();

            return AssemblyTypes.FirstOrDefault(x => x.FullName.EndsWith(state));
        }

        public static T CreateInstance<T>(Type type, Type model)
        {
            return (T)Activator.CreateInstance(type.MakeGenericType(model))!;
        }

        private static string ProcessRequestState(dynamic request, string requestType, string folder)
        {
            var requestStatus = request.Status;

            Dictionary<string, Func<string, string>> Processor = new()
            {
                ["Pending"] = (requestType) =>
                {
                    if (requestType == "Bulk")
                    {
                        return $"{folder}.PendingBulkState`1";
                    }

                    if (folder == "SiteHalt" && request.RequestAction == "UnHalt")
                    {
                        return $"{folder}.TAApprovedState`1";
                    }

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
                    return $"{folder}.FAApprovedState`1";
                },

                ["SAApproved"] = (requestType) =>
                {
                    return $"{folder}.SAApprovedState`1";
                },

                ["TAApproved"] = (requestType) =>
                {
                    return $"{folder}.TAApprovedState`1";
                },

                ["FADisapproved"] = (requestType) =>
                {
                    return $"{folder}.DisapprovedState`1";
                },

                ["SADisapproved"] = (requestType) =>
                {
                    return $"{folder}.DisapprovedState`1";
                },

                ["TADisapproved"] = (requestType) =>
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
