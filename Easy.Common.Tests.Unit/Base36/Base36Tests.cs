using NUnit.Framework;
using Shouldly;

namespace Easy.Common.Tests.Unit.Base36
{
    [TestFixture]
    public class Base36Tests
    {
        [TestCase]
        public void When_encoding_long_with_base36()
        {
            Common.Base36.Encode(10).ShouldBe("A");
        }
        
        [TestCase]
        public void When_decoding_the_base36()
        {
            Common.Base36.Decode("A").ShouldBe(10);
        }
    }
}
