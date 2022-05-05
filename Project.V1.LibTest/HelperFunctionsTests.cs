using Project.V1.Lib.Helpers;
using System;
using Xunit;
using Xunit.Abstractions;

namespace Project.V1.LibTest
{
    public class HelperFunctionsTests : IDisposable
    {
        private readonly ITestOutputHelper _output;

        public HelperFunctionsTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData("SA", "acceptance")]
        [InlineData("EM", "eq-matching")]
        public void GetAppLink_ShouldWork(string appAbbr, string expected)
        {
            var actual = HelperFunctions.GetAppLink(appAbbr);

            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("HS", "Holistic Site")]
        [InlineData("H|U|D", "Halt | Unhalt | Decom")]
        public void GetTypeName_ShouldWork(string appAbbr, string expected)
        {
            var actual = HelperFunctions.GetTypeName(appAbbr);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RemoveSpecialCharacters_ShouldWork()
        {
            var stringName = "Adekunle//|#";

            var actual = HelperFunctions.RemoveSpecialCharacters(stringName);
            var expected = "Adekunle____";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UpperCaseFirst_ShouldWork()
        {
            var stringName = "adekunle//|#";

            var actual = HelperFunctions.UpperCaseFirst(stringName);
            var expected = "Adekunle//|#";

            Assert.Equal(expected, actual);
        }

        public void Dispose()
        {
            _output.WriteLine("this has been disposed");
        }
    }
}
