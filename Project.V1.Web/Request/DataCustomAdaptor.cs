using Syncfusion.Blazor.Data;
using System.Net.Http;
using System.Net.Http.Json;

namespace Project.V1.Web.Request;

public class AcceptanceAdaptor : DataCustomAdaptor<RequestViewModel>
{
    public AcceptanceAdaptor()
    {
        base.GetData("odata/Acceptance");
    }
}

public class DataCustomAdaptor<T> : DataAdaptor where T : class, new()
{
    private readonly HttpClient _http;
    public List<T> RequestData { get; set; }

    public DataCustomAdaptor()
    {
        _http = new HttpClient();
    }

    public async void GetData(string path)
    {
        RequestData = await _http.GetFromJsonAsync<List<T>>(path);
    }

    // Performs data Read operation
    public override object Read(DataManagerRequest dm, string key = null)
    {
        IEnumerable<T> DataSource = RequestData;

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

        int count = DataSource.Cast<T>().Count();
        if (dm.Skip != 0)
        {
            //Paging
            DataSource = DataOperations.PerformSkip(DataSource, dm.Skip);
        }
        if (dm.Take != 0)
        {
            DataSource = DataOperations.PerformTake(DataSource, dm.Take);
        }

        return dm.RequiresCounts ? new DataResult() { Result = DataSource, Count = count } : (object)DataSource;
    }

    // Performs Insert operation
    public override object Insert(DataManager dm, object value, string key)
    {
        RequestData.Insert(0, value as T);
        return value;
    }

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
