namespace Easy.Common.Tests.Unit.ByteExtensions;

using Easy.Common.Extensions;
using NUnit.Framework;
using Shouldly;
using System;

[TestFixture]
internal sealed class ComparingByteArraysTests
{
    [Test]
    public void When_comparing_byte_arrays()
    {
        byte[] left = { 1, 2, 3 };
        byte[] right = { 1, 2, 3 };

        left.Compare(left).ShouldBeTrue();

        left.Compare(right).ShouldBeTrue();
        right.Compare(left).ShouldBeTrue();

        right = new byte[] { 1, 3, 2 };
        left.Compare(right).ShouldBeFalse();
        right.Compare(left).ShouldBeFalse();

        right = new byte[] { 1, 2, 3 };
        left.Compare(right).ShouldBeTrue();
        right.Compare(left).ShouldBeTrue();

        right = new byte[] { 1, 2, 3, 4 };
        left.Compare(right).ShouldBeFalse();
        right.Compare(left).ShouldBeFalse();

        ((byte[]) null).Compare(right).ShouldBeFalse();

        left = new byte[] { 1 };
        left.Compare(null).ShouldBeFalse();

        left = Array.Empty<byte>();
        right = Array.Empty<byte>();

        left.Compare(right).ShouldBeTrue();
        right.Compare(left).ShouldBeTrue();
    }
}