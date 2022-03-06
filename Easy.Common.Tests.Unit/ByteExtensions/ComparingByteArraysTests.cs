namespace Easy.Common.Tests.Unit.ByteExtensions
{
    using System;
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    internal sealed class ComparingByteArraysTests
    {
        [Test]
        public void When_comparing_byte_arrays()
        {
            var left = new byte[] {1, 2, 3};
            var right = new byte[] {1, 2, 3};

            left.Compare(left).ShouldBeTrue();

            left.Compare(right).ShouldBeTrue();
            right.Compare(left).ShouldBeTrue();

            right = new byte[] {1, 3, 2};
            left.Compare(right).ShouldBeFalse();
            right.Compare(left).ShouldBeFalse();

            right = new byte[] { 1, 2, 3 };
            left.Compare(right).ShouldBeTrue();
            right.Compare(left).ShouldBeTrue();

            right = new byte[] { 1, 2, 3, 4 };
            left.Compare(right).ShouldBeFalse();
            right.Compare(left).ShouldBeFalse();

            left = null;
            Should.Throw<ArgumentNullException>(() => left.Compare(right))
#if NET471_OR_GREATER
                .Message.ShouldBe("Value cannot be null.\r\nParameter name: left");
#else
                .Message.ShouldBe("Value cannot be null. (Parameter 'left')");
#endif

            left = new byte[] {1};
            right = null;

            Should.Throw<ArgumentNullException>(() => left.Compare(right))
#if NET471_OR_GREATER
                .Message.ShouldBe("Value cannot be null.\r\nParameter name: right");
#else
                .Message.ShouldBe("Value cannot be null. (Parameter 'right')");
#endif

            left = new byte[0];
            right = new byte[0];

            left.Compare(right).ShouldBeTrue();
            right.Compare(left).ShouldBeTrue();
        }
    }
}