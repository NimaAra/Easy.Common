namespace Easy.Common.Tests.Unit.BloomFilter
{
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    internal sealed class BloomFilterTests
    {
        [Test]
        public void When_creating_a_filter()
        {
            var filter = new BloomFilter<string>(2);

            filter.Truthiness.ShouldBe(0);
            filter.Contains("Hello").ShouldBeFalse();
            filter.Truthiness.ShouldBe(0);

            filter.Add("Hello");
            filter.Truthiness.ShouldNotBe(0);
            filter.Truthiness.ShouldNotBe(1);
            filter.Contains("Hello").ShouldBeTrue();

            filter.Contains(string.Empty).ShouldBeFalse();
            filter.Contains("Cat").ShouldBeFalse();
            filter.Contains("hELlo").ShouldBeFalse();

            filter.Add("Big");
            filter.Truthiness.ShouldNotBe(0);
            filter.Truthiness.ShouldNotBe(1);
            filter.Contains("Big").ShouldBeTrue();

            filter.Contains(string.Empty).ShouldBeTrue();
            filter.Contains("Cat").ShouldBeTrue();
            filter.Contains("hELlo").ShouldBeTrue();

            filter.Add("World");
            filter.Truthiness.ShouldNotBe(0);
            filter.Truthiness.ShouldNotBe(1);
            filter.Contains("World").ShouldBeTrue();

            filter.Add("!");
            filter.Truthiness.ShouldNotBe(0);
            filter.Truthiness.ShouldNotBe(1);
            filter.Contains("!").ShouldBeTrue();

            filter.Add("!!");
            filter.Truthiness.ShouldNotBe(0);
            filter.Truthiness.ShouldBe(1);
            filter.Contains("!!").ShouldBeTrue();

            filter.Contains("Hello").ShouldBeTrue();
            filter.Contains("Big").ShouldBeTrue();
            filter.Contains("World").ShouldBeTrue();
            filter.Contains("!").ShouldBeTrue();
            filter.Contains("!!").ShouldBeTrue();
        }
    }
}