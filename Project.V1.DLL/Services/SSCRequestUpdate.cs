namespace Project.V1.DLL.Services;

public class SSCRequestUpdate : MTNISGenericRepo<SSCUpdatedCell>, ISSCRequestUpdate, IDisposable
{
    public SSCRequestUpdate(MTNISDbContext context, ICLogger logger)
        : base(context, "", logger)
    {
    }
}