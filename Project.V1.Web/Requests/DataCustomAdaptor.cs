using Syncfusion.Blazor.Data;
using System.Net.Http;
using System.Net.Http.Json;

namespace Project.V1.Web.Requests;

public class AcceptanceAdaptor : DataCustomAdaptor<RequestViewModel>
{
    private string LoggedInUser { get; set; }
    private readonly IRequest _request;

    public AcceptanceAdaptor()
    {
        HttpContextAccessor httpContextAccessor = new();
        LoggedInUser = httpContextAccessor.HttpContext?.User.Identity.Name;

        if (LoggedInUser == null)
            return;

        _request = ServiceActivator.GetScope().ServiceProvider.GetService<IRequest>();

        GetData().GetAwaiter().GetResult();
    }

    public async Task GetData()
    {
        var Requests = Array.Empty<RequestViewModel>().AsQueryable();

        var user = await LoginObject.UserManager.FindByNameAsync(LoggedInUser);

        if (user == null)
            RequestData = Requests;

        var vendor = await LoginObject.Vendor.GetById(x => x.Id == user.VendorId);

        if (user.ShowAllRegionReport)
        {
            RequestData = await _request.Get(x => x.Id != null, x => x.OrderByDescending(y => y.DateCreated), new RequestViewModel().Navigations);
            Count = await _request.Count(x => x.Id != null);
        }
        else if (vendor.Name == "MTN Nigeria" || (await LoginObject.UserManager.IsInRoleAsync(user, "User")))
        {
            RequestData = await _request.Get(x => user.Regions.Select(x => x.Id).Contains(x.RegionId), x => x.OrderByDescending(y => y.DateCreated), new RequestViewModel().Navigations);
            Count = await _request.Count(x => user.Regions.Select(x => x.Id).Contains(x.RegionId));
        }
        else
        {
            RequestData = await _request.Get(x => x.Requester.VendorId == user.VendorId, x => x.OrderByDescending(y => y.DateCreated), new RequestViewModel().Navigations);
            Count = await _request.Count(x => x.Requester.VendorId == user.VendorId);
        }
    }

}

public class DataCustomAdaptor<T> : DataAdaptor where T : class
{
    public IQueryable<T> RequestData { get; set; }
    public int Count { get; set; }

    // Performs data Read operation
    public override object Read(DataManagerRequest dm, string key = null)
    {
        IQueryable<T> DataSource = RequestData;

        if (dm.Search != null && dm.Search.Count > 0)
        {
            // Searching
            DataSource = DataOperations.PerformSearching(DataSource, dm.Search);
        }
        if (dm.Sorted != null && dm.Sorted.Count > 0)
        {
            // Sorting
            DataSource = DataOperations.PerformSorting(DataSource, dm.Sorted);
        }
        if (dm.Where != null && dm.Where.Count > 0)
        {
            // Filtering
            DataSource = DataOperations.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
        }

        //int count = DataSource.Cast<T>().Count();
        if (dm.Skip != 0)
        {
            //Paging
            DataSource = DataOperations.PerformSkip(DataSource, dm.Skip);
        }
        if (dm.Take != 0)
        {
            DataSource = DataOperations.PerformTake(DataSource, dm.Take);
        }

        return dm.RequiresCounts ? new DataResult() { Result = DataSource.ToList(), Count = Count } : (object)DataSource.ToList();
    }

    // Performs Insert operation
    //public override object Insert(DataManager dm, object value, string key)
    //{
    //    RequestData.Insert(0, value as T);
    //    return value;
    //}

    // Performs Remove operation
    //public override object Remove(DataManager dm, object value, string keyField, string key)
    //{
    //    RequestData.Remove(RequestData.Where(or => or.TID == int.Parse(value.ToString())).FirstOrDefault());
    //    return value;
    //}

    // Performs Update operation
    //public override object Update(DataManager dm, object value, string keyField, string key)
    //{
    //    var data = RequestData.Where(or => or.TID == (value as T).TID).FirstOrDefault();
    //    if (data != null)
    //    {
    //        data.TID = (value as T).TID;
    //        data.CustomerID = (value as T).CustomerID;
    //        data.Freight = (value as T).Freight;
    //    }
    //    return value;
    //}

    // Performs BatchUpdate operation
    //public override object BatchUpdate(DataManager dm, object Changed, object Added, object Deleted, string KeyField, string Key)
    //{
    //    if (Changed != null)
    //    {
    //        foreach (var rec in (IEnumerable<T>)Changed)
    //        {
    //            RequestData[0].CustomerID = rec.CustomerID;
    //        }

    //    }
    //    if (Added != null)
    //    {
    //        foreach (var rec in (IEnumerable<T>)Added)
    //        {
    //            RequestData.Add(rec);
    //        }

    //    }
    //    if (Deleted != null)
    //    {
    //        foreach (var rec in (IEnumerable<T>)Deleted)
    //        {
    //            RequestData.RemoveAt(0);
    //        }

    //    }
    //    return RequestData;
    //}
}
