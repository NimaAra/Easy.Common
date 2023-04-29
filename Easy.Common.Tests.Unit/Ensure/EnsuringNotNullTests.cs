namespace Easy.Common.Tests.Unit.Ensure;

using Easy.Common;
using NUnit.Framework;
using Shouldly;
using System;

[TestFixture]
public class EnsuringNotNullTests
{
    [Test]
    public void When_ensuring_null_string()
    {
        string nullStr = null;
        Should.Throw<ArgumentException>(() => Ensure.NotNull(nullStr, "nullStr"))
            .Message.ShouldBe("Value cannot be null. (Parameter 'nullStr')");
    }

    [Test]
    public void When_ensuring_null_object()
    {
        object nullObj = null;
        Should.Throw<ArgumentException>(() => Ensure.NotNull(nullObj, "nullObj"))
            .Message.ShouldBe("Value cannot be null. (Parameter 'nullObj')");
    }

    [Test]
    public void When_ensuring_non_null_object()
    {
        object nonNullObj = 1;
        Should.NotThrow(() => Ensure.NotNull(nonNullObj, "nonNullObj"), 
            "Because nonNullObject is not null");
    }

    [Test]
    public void When_ensuring_non_null_object_with_null_argument_name()
    {
        object anyObject = 120;
        Should.NotThrow(() => Ensure.NotNull(anyObject, null),
            "Because object is not null");
    }

    [Test]
    public void When_ensuring_null_object_with_null_argument_name()
    {
        object nullObject = null;
        Should.Throw<ArgumentException>(() => Ensure.NotNull(nullObject, null))
            .Message.ShouldBe("Value cannot be null.");
    }
}