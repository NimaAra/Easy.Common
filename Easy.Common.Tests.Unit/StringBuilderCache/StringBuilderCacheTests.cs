namespace Easy.Common.Tests.Unit.StringBuilderCache
{
    using System.Text;
    using NUnit.Framework;
    using Shouldly;
    using Easy.Common;

    [TestFixture]
    public sealed class StringBuilderCacheTests
    {
        [Test]
        public void When_acquiring_multiple_instances()
        {
            var builderOne = StringBuilderCache.Acquire();
            var builderTwo = StringBuilderCache.Acquire();

            builderOne.ShouldNotBeSameAs(builderTwo);

            builderOne.Append("Hello");

            var builderOneStr = StringBuilderCache.GetStringAndRelease(builderOne);

            builderOneStr.ShouldBe("Hello");

            var builderThree = StringBuilderCache.Acquire();
            builderThree.ShouldBeSameAs(builderOne);

            var builderTwoStr = StringBuilderCache.GetStringAndRelease(builderTwo);

            builderTwoStr.ShouldBeEmpty();

            builderThree.ShouldNotBeSameAs(builderTwo);
        }

        [Test]
        public void When_returning_an_instance_to_the_cache()
        {
            var builderOne = new StringBuilder();
            builderOne.Append("Foo");

            var builderOneFirstStr = builderOne.ToString();
            builderOneFirstStr.ShouldBe("Foo");

            var builderOneSecondStr = StringBuilderCache.GetStringAndRelease(builderOne);
            builderOneSecondStr.ShouldBe("Foo");

            var builderTwo = StringBuilderCache.Acquire();

            builderOne.ShouldBeSameAs(builderTwo);

            builderTwo.Capacity.ShouldBe(builderOne.Capacity);

            var builderTwoThirdStr = builderTwo.ToString();
            builderTwoThirdStr.ShouldBeEmpty();

            var builderTwoFourthStr = StringBuilderCache.GetStringAndRelease(builderTwo);
            builderTwoFourthStr.ShouldBeEmpty();
        }
    }
}