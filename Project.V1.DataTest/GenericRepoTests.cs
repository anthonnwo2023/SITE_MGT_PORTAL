using Project.V1.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Project.V1.DataTest;

public class GenericRepoTests
{
    private readonly ITestOutputHelper _output;
    private readonly DbContextOptions<ApplicationDbContext> _optionsBuilder;

    public GenericRepoTests(ITestOutputHelper output)
    {
        _output = output;
        _optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDB").Options;
    }

    [Fact]
    public async void Get_WhenCalled_ShouldReturnListOfObject()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var context = new ApplicationDbContext(optionsBuilder.Options);
        var genericRepo = new Mock<GenericRepo<ApplicationUser>>(context, "");

        //var user = GetSampleUsers().Result.First();
        genericRepo.Setup(repo => repo.Get())
            .Returns(GetSampleUsers());

        var repo = genericRepo.Object;

        var actual = await repo.Get();
        var expected = await GetSampleUsers();

        //using var mock = AutoMock.GetLoose();

        //mock.Mock<IGenericRepo<ApplicationUser>>().Setup(x => x.Get())
        //    .Returns(GetSampleUsers());

        //var userProcessor = mock.Create<IGenericRepo<ApplicationUser>>();
        //var actual = await userProcessor.Get();
        //var expected = await GetSampleUsers();

        genericRepo.Verify(repo => repo.Get(), Times.AtMostOnce);
        Assert.True(actual != null);
        Assert.Equal(expected.Count, actual.Count);

        for (int i = 0; i < expected.Count; i++)
        {
            Assert.Equal(expected[i].Fullname, actual[i].Fullname);
            Assert.Equal(expected[i].Email, actual[i].Email);
            Assert.Equal(expected[i].PhoneNumber, actual[i].PhoneNumber);
        }
    }

    private async Task<List<ApplicationUser>> GetSampleUsers()
    {
        var users = new List<ApplicationUser>()
        {
            new ApplicationUser
            {
                Fullname = "Adekunle Adeyemi",
                Email = "Adekunle.Adeyemi@mtn.com",
                Id = Guid.NewGuid().ToString(),
                IsActive = true,
                DateCreated = DateTime.Now,
                UserName = "adekadey"
            },
            new ApplicationUser
            {
                Fullname = "Anthony Nwosu",
                Email = "Anthony.Nwosu@mtn.com",
                Id = Guid.NewGuid().ToString(),
                IsActive = true,
                DateCreated = DateTime.Now,
                UserName = "anthonnwo"
            },
            new ApplicationUser
            {
                Fullname = "Kehinde Ayoola-Oni",
                Email = "Kehinde.Ayoola-Oni@mtn.com",
                Id = Guid.NewGuid().ToString(),
                IsActive = true,
                DateCreated = DateTime.Now,
                UserName = "kehindad"
            },
        };

        return await Task.FromResult(users);
    }
}
