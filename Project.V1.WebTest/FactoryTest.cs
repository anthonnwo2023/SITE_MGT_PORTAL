using Project.V1.DLL.Helpers;
using Project.V1.DLL.RequestActions;
using Project.V1.Models;
using Project.V1.Models.SiteHalt;
using System;
using Xunit;
using Xunit.Abstractions;

namespace Project.V1.DLLTest;

public class FactoryTest : IDisposable
{
    private readonly ITestOutputHelper _output;

    public FactoryTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Create_SimpleObjectShouldReturnNew()
    {
        var newObj = Factory.CreateObject<Calculator>();
        Assert.NotNull(newObj);
        Assert.IsType<Calculator>(newObj);
        Assert.Equal(typeof(Calculator), newObj.GetType());
    }

    [Theory]
    [InlineData("Rejected", "")]
    [InlineData("Pending", "Bulk")]
    [InlineData("Pending", "")]
    [InlineData("Accepted", "")]
    [InlineData("Restarted", "")]
    [InlineData("Reworked", "")]
    [InlineData("Cancelled", "")]
    public void Create_ComplexObjectShouldReturnNew(string status, string requestType)
    {
        var request = new RequestViewModel
        {
            Status = status
        };

        var newObject = Factory.CreateObject<RequestStateBase<RequestViewModel>, RequestViewModel>(
            request, requestType, null);

        var expected = $"{request.Status}{requestType}State`1";

        Assert.NotNull(newObject);
        Assert.Equal(expected, newObject.GetType().Name);
    }

    [Theory]
    [InlineData("Pending", "Halt", "", "SiteHalt")]
    [InlineData("FAApproved", "", "", "SiteHalt")]
    [InlineData("SAApproved", "", "", "SiteHalt")]
    [InlineData("TAApproved", "", "", "SiteHalt")]
    [InlineData("Restarted", "", "", "SiteHalt")]
    [InlineData("Completed", "", "", "SiteHalt")]
    public void Create_ComplexObjectShouldReturnNewSiteHalt(string status, string requestAction, string requestType, string folder)
    {
        var request = new SiteHUDRequestModel
        {
            Status = status,
            RequestAction = requestAction
        };

        var newObject = Factory.CreateObject<RequestStateBase<SiteHUDRequestModel>, SiteHUDRequestModel>(
            request, requestType, folder);

        var expected = $"{folder}{(string.IsNullOrWhiteSpace(folder) ? null : ".")}{request.Status}{requestType}State`1";//.Replace("..", ".");
        var expectedType = Factory.GetStateType(expected);
        var expectedTypeInstance = Factory.CreateInstance<dynamic>(expectedType, request.GetType());

        Assert.NotNull(newObject);
        Assert.Equal(expectedTypeInstance.GetType(), newObject.GetType());
    }

    [Theory]
    [InlineData("Pending", "UnHalt", "", "SiteHalt", "TAApproved")]
    public void Create_ComplexObjectShouldReturnNewObject(string status, string requestAction, string requestType, string folder, string expect)
    {
        var request = new SiteHUDRequestModel
        {
            Status = status,
            RequestAction = requestAction
        };

        var newObject = Factory.CreateObject<RequestStateBase<SiteHUDRequestModel>, SiteHUDRequestModel>(
            request, requestType, folder);

        var expected = $"{folder}{(string.IsNullOrWhiteSpace(folder) ? null : ".")}{expect}{requestType}State`1";
        var expectedType = Factory.GetStateType(expected);
        var expectedTypeInstance = Factory.CreateInstance<dynamic>(expectedType, request.GetType());

        Assert.NotNull(newObject);
        Assert.Equal(expectedTypeInstance.GetType(), newObject.GetType());
    }

    [Theory]
    [InlineData("FADisapproved", "", "", "SiteHalt")]
    [InlineData("SADisapproved", "", "", "")]
    [InlineData("TADisapproved", "", "", "SiteHalt")]
    public void Create_ComplexObjectShouldReturnNewSiteHalt_DisapprovedState(string status, string requestAction, string requestType, string folder)
    {
        var request = new SiteHUDRequestModel
        {
            Status = status,
            RequestAction = requestAction
        };

        var newObject = Factory.CreateObject<RequestStateBase<SiteHUDRequestModel>, SiteHUDRequestModel>(
            request, requestType, folder);

        status = "Disapproved";

        var expected = $"{folder}{(string.IsNullOrWhiteSpace(folder) ? null : ".")}{status}{requestType}State`1";//.Replace("..", ".");
        var expectedType = Factory.GetStateType(expected);
        var expectedTypeInstance = Factory.CreateInstance<dynamic>(expectedType, request.GetType());

        Assert.NotNull(newObject);
        Assert.Equal(expectedTypeInstance.GetType(), newObject.GetType());
    }

    public void Dispose()
    {
        _output.WriteLine("this has been disposed");
    }
}
