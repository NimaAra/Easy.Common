namespace Easy.Common.Tests.Unit.HashHelper;

using System;
using Easy.Common.Extensions;
using NUnit.Framework;
using Shouldly;
using HashHelper = Easy.Common.HashHelper;

[TestFixture]
internal sealed class HashHelperTests
{
    [Test]
    public void When_getting_hash_code_of_arrays()
    {
        var array1 = new[] {1, 2, 3};
        var array2 = new[] {1, 2, 3};
        var array3 = new[] {3, 2, 1};

        HashHelper.GetHashCode(array1)
            .ShouldBe(HashHelper.GetHashCode(array2));

        HashHelper.GetHashCode(array1)
            .ShouldNotBe(HashHelper.GetHashCode(array3));
    }

    [Test]
    public void When_getting_hash_code_for_different_types()
    {
        HashHelper.GetHashCode(1, "foo")
            .ShouldBe(HashHelper.GetHashCode(1, "foo"));

        HashHelper.GetHashCode(1, "foo")
            .ShouldNotBe(HashHelper.GetHashCode("foo", 1));
    }

    [Test]
    public void When_checking_hash_code_for_default_values_of_different_types()
    {
        HashHelper.GetHashCode(0)
            .ShouldBe(HashHelper.GetHashCode(default(TimeSpan)));
            
        HashHelper.GetHashCode(0)
            .ShouldBe(HashHelper.GetHashCode((string)null));

        HashHelper.GetHashCode(1)
            .ShouldNotBe(HashHelper.GetHashCode("1"));
    }

    [Test]
    public void When_getting_hash_code_for_int()
    {
        HashHelper.GetHashCode(0).ShouldBe(0);

        HashHelper.GetHashCode(0)
            .ShouldBe(HashHelper.GetHashCode(0));

        HashHelper.GetHashCode(0)
            .ShouldNotBe(HashHelper.GetHashCode(1));

        HashHelper.GetHashCode(int.MaxValue)
            .ShouldNotBe(HashHelper.GetHashCode(int.MinValue));

        HashHelper.GetHashCode(1, 2)
            .ShouldNotBe(HashHelper.GetHashCode(2, 1));

        HashHelper.GetHashCode(1, 2)
            .ShouldNotBe(HashHelper.GetHashCode(1, 2, 0));
    }

    [Test]
    public void When_getting_hash_code_for_string()
    {
        string nullString = null;

        HashHelper.GetHashCode(nullString).ShouldBe(0);

        HashHelper.GetHashCode(nullString)
            .ShouldBe(HashHelper.GetHashCode(nullString));

        HashHelper.GetHashCode(nullString)
            .ShouldNotBe(HashHelper.GetHashCode("foo"));

        HashHelper.GetHashCode("foo")
            .ShouldNotBe(HashHelper.GetHashCode("bar"));

        HashHelper.GetHashCode("foo", "bar")
            .ShouldNotBe(HashHelper.GetHashCode("bar", "foo"));

        HashHelper.GetHashCode("foo", "bar")
            .ShouldNotBe(HashHelper.GetHashCode("foo", "bar", nullString));
    }

    [Test]
    public void When_getting_hash_code_for_timespan()
    {
        HashHelper.GetHashCode(default(TimeSpan)).ShouldBe(0);

        HashHelper.GetHashCode(default(TimeSpan))
            .ShouldBe(HashHelper.GetHashCode(default(TimeSpan)));

        HashHelper.GetHashCode(default(TimeSpan))
            .ShouldNotBe(HashHelper.GetHashCode(1.Seconds()));

        HashHelper.GetHashCode(1.Seconds())
            .ShouldNotBe(HashHelper.GetHashCode(2.Seconds()));

        HashHelper.GetHashCode(1.Seconds(), 2.Seconds())
            .ShouldNotBe(HashHelper.GetHashCode(2.Seconds(), 1.Seconds()));

        HashHelper.GetHashCode(1.Seconds(), 2.Seconds())
            .ShouldNotBe(HashHelper.GetHashCode(1.Seconds(), 2.Seconds(), default(TimeSpan)));
    }
}