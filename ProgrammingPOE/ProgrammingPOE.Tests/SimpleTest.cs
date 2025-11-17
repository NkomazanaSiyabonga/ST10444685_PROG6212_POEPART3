using Xunit;

namespace ProgrammingPOE.Test
{
    public class SimpleTest
    {
        [Fact]
        public void OnePlusOneEqualsTwo()
        {
            Assert.Equal(2, 1 + 1);
        }
    }
}