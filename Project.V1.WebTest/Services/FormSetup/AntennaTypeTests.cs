using Autofac.Extras.Moq;
using Moq;
using Project.V1.DLL.Services.Interfaces.FormSetup;
using Project.V1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Project.V1.DLLTest.Services.FormSetup;

public class AntennaTypeTests : IDisposable
{
    private readonly ITestOutputHelper _output;

    public AntennaTypeTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async void Create_WhenCalled_ShouldReturnType()
    {
        using var mock = AutoMock.GetLoose();

        var antennaType = (await GetSampleAntennaTypes()).First();
        mock.Mock<IAntennaType>().Setup(x => x.Create(antennaType).Result)
            .Returns((antennaType, ""));

        var userProcessor = mock.Create<IAntennaType>();
        var actual = await userProcessor.Create(antennaType);

        mock.Mock<IAntennaType>().Verify(repo => repo.Create(antennaType), Times.Exactly(1));

        var expected = (await GetSampleAntennaTypes())[0];

        Assert.Equal(expected.Name, actual.Item1.Name);
    }

    [Fact]
    public async void Update_WhenCalled_ShouldReturnType()
    {
        using var mock = AutoMock.GetLoose();

        var antennaType = (await GetSampleAntennaTypes()).First();
        antennaType.IsActive = false;

        mock.Mock<IAntennaType>().Setup(x => x.Update(antennaType, x => x.Id == antennaType.Id).Result)
            .Returns((antennaType, ""));

        var userProcessor = mock.Create<IAntennaType>();
        var actual = await userProcessor.Update(antennaType, x => x.Id == antennaType.Id);

        mock.Mock<IAntennaType>().Verify(repo => repo.Update(antennaType, x => x.Id == antennaType.Id), Times.Exactly(1));

        var expected = (await GetSampleAntennaTypes())[0];

        Assert.NotEqual(expected.IsActive, actual.Item1.IsActive);
    }

    [Fact]
    public async void Get_WhenCalled_ShouldReturnListOfObject()
    {
        //var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        //var context = new ApplicationDbContext(optionsBuilder.Options);
        //var genericRepo = new Mock<IAntennaType>();

        //var user = GetSampleUsers().Result.First();
        //genericRepo.Setup(repo => repo.Get())
        //    .Returns(GetSampleUsers());

        //var repo = genericRepo.Object;

        //var actual = await repo.Get();
        //var expected = await GetSampleUsers();

        using var mock = AutoMock.GetLoose();

        mock.Mock<IAntennaType>().Setup(x => x.Get())
            .Returns(GetSampleAntennaTypes());

        var cls = mock.Create<IAntennaType>();
        var actual = await cls.Get();
        var expected = await GetSampleAntennaTypes();

        mock.Mock<IAntennaType>().Verify(repo => repo.Get(), Times.Exactly(1));
        Assert.True(actual != null);
        Assert.Equal(expected.Count, actual.Count);
        Assert.IsType(expected.GetType(), actual);

        for (int i = 0; i < expected.Count; i++)
        {
            Assert.Equal(expected[i].Name, actual[i].Name);
            Assert.Equal(expected[i].IsActive, actual[i].IsActive);
        }
    }

    private static async Task<List<AntennaTypeModel>> GetSampleAntennaTypes()
    {
        var users = new List<AntennaTypeModel>()
    {
        new  AntennaTypeModel
        {
            Name = "Adekunle Adeyemi",
            Id = Guid.NewGuid().ToString(),
            IsActive = true,
            DateCreated = DateTime.Now,
        },
        new AntennaTypeModel
        {
            Name = "Anthony Nwosu",
            Id = Guid.NewGuid().ToString(),
            IsActive = true,
            DateCreated = DateTime.Now,
        },
        new AntennaTypeModel
        {
            Name = "Huawei",
            Id = Guid.NewGuid().ToString(),
            IsActive = true,
            DateCreated = DateTime.Now,
        },
    };

        return await Task.FromResult(users);
    }

    public void Dispose()
    {
        _output.WriteLine("this has been disposed");
    }
}
