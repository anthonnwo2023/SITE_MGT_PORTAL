using Autofac.Extras.Moq;
using Project.V1.Lib.Interfaces;
using Project.V1.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.V1.DataTest;

public class GenericRepoTests
{
    [Fact]
    public async void Get_WhenCalled_ShouldReturnListOfObject()
    {
        //var optionsBuilder = new Mock<DbContextOptions<ApplicationDbContext>>();

        //var store = new Mock<IUserStore<ApplicationUser>>();
        //store.Setup(x => x.FindByIdAsync("123", CancellationToken.None))
        //    .ReturnsAsync(new ApplicationUser()
        //    {
        //        UserName = "test@email.com",
        //        Id = "123"
        //    });

        //var mgr = new UserManager<ApplicationUser>(store.Object, null, null, null, null, null, null, null, null);
        //var mock = new Mock<ApplicationDbContext>()
        //    .Setup;

        //var context = new ApplicationDbContext(optionsBuilder.Object);
        using var mock = AutoMock.GetLoose();
        //var contextOptions = mock.Mock<DbContextOptions<ApplicationDbContext>>()
        //    .Setup(x => x)
        //    .Returns(optionsBuilder.Object);

        //mock.Mock<UserManager<ApplicationUser>>().Setup(x =>)
        mock.Mock<IGenericRepo<ApplicationUser>>().Setup(x => x.Get())
            .Returns(GetSampleUsers());

        var userProcessor = mock.Create<IGenericRepo<ApplicationUser>>();
        var actual = await userProcessor.Get();
        var expected = await GetSampleUsers();

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
