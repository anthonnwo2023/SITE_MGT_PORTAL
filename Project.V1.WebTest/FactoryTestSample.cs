using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace Project.V1.DLLTest;

public class FactoryTestSample : IDisposable
{
    string _customMessage = "Custom Message";
    int _counter = 0;
    private readonly ITestOutputHelper _output;

    public FactoryTestSample(ITestOutputHelper output)
    {
        _counter++;
        _output = output;
    }

    public void Dispose()
    {
        _counter--;
        _output.WriteLine("this has been disposed");
    }

    [Trait("Category", "Calculator")]
    [Theory]
    [InlineData(3, 1, 4)]
    [InlineData(-1, 1, 0)]
    [InlineData(int.MaxValue, int.MaxValue, -2)]
    public void ShouldAddTwoNumbers(int addend1, int addend2, int result)
    {
        //Arrange
        var calculator = new Calculator();
        //Act
        var actual = calculator.Add(addend1, addend2);
        //Assert
        Assert.Equal(result, actual);
    }

    [Trait("Category", "Calculator")]
    [Fact]
    public void ShouldSubtractTwoNumbers()
    {
        var calc = new Calculator();
        var actual = calc.Subtract(5, 3);
        var expected = 2;
        Assert.Equal(expected, actual);
    }

    [Trait("Category", "Calculator")]
    [Theory]
    [InlineData(5, 3, 1.6)]
    [InlineData(5, 10, 0.5)]
    [InlineData(12, 3, 4)]
    //[InlineData(12, 3, 3)]
    public void ShouldDivideTwoNumbers(double addend1, double divisor, double expected)
    {
        var calc = new Calculator();
        var actual = calc.Divide(addend1, divisor);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ShouldBeOne()
    {
        Assert.Equal(1, _counter);
    }

    [Fact]
    public void ShouldAlsoBeOne()
    {
        Assert.Equal(1, _counter);
    }

    [Fact]
    public void FirstFact()
    {
        Assert.Equal(5, 3 + 2);
    }

    [Theory]
    [InlineData(5, 3, 2)]
    [InlineData(7, 3, 4)]
    [InlineData(-1, -3, 2)]
    public void FirstTheory(int expected, int addend1, int addend2)
    {
        Assert.Equal(expected, addend2 + addend1);
    }

    [Fact(DisplayName = "Ignored Test - Custom Display Name", Skip = "This can be anything")]
    public void ThiIsIgnored()
    {
        //TODO: Fix this test
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void MemberDataTheory(int expected, int addend1, int addend2)
    {
        Assert.Equal(expected, addend2 + addend1);
    }

    public static IEnumerable<object[]> TestData => new[]
    {
        new object[] {5, 3, 2},
        new object[] {-1, -3, 2},
    };

    [Theory]
    [ClassData(typeof(TestDataClass))]
    public void ClassDataTheory(int expected, int addend1, int addend2)
    {
        Assert.Equal(expected, addend1 + addend2);
    }

    [Fact]
    public void ShouldThrowAnException()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => ThrowAnError());
        Assert.Equal(_customMessage, ex.Message);
    }

    [Fact]
    public void ShouldRecordAnException()
    {
        Exception ex = Record.Exception(() => ThrowAnError());
        Assert.Equal(_customMessage, ex.Message);
    }

    [Fact]
    public void AddPersonToPeopleList_ShouldWork()
    {
        //PersonModel newPerson = new PersonModel { Firstname = "Tim", LastName = "Corey" };
        //List<PersonModel> people = new();
        //DataAccess.AddPersonToPeopleList(people, newPerson);

        //Assert.True(people.Count == 1);
        //Assert.Contains<PersonModel>(newPerson, people);
    }

    private void ThrowAnError()
    {
        throw new InvalidOperationException(_customMessage);
    }
}

public class TestDataClass : IEnumerable<object[]>
{
    private readonly List<object[]> _data = new()
    {
        new object[] { 5, 3, 2 },
        new object[] { -1, -3, 2 }
    };

    public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _data.GetEnumerator();
}

public class Calculator
{
    public int Add(int? addend1, int? addend2)
    {
        if (!addend1.HasValue || !addend2.HasValue)
        {
            throw new ArgumentNullException("value expected.");
        }

        return addend1.Value + addend2.Value;
    }

    public int Subtract(int addend1, int addend2)
    {
        return addend1 - addend2;
    }

    public double Divide(double addend1, double divisor)
    {
        return Math.Round(addend1 / divisor, 1, MidpointRounding.ToZero);
    }
}