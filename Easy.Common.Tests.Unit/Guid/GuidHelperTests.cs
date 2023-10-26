namespace Easy.Common.Tests.Unit.Guid;

using NUnit.Framework;
using Shouldly;
using System;

[TestFixture]
internal sealed class GuidHelperTests
{
    [Test]
    public void When_creating_a_sequential_uuid()
    {
        var defaultGuid = Guid.NewGuid().ToByteArray();
        var sequentialGuid = GuidHelper.CreateSequentialUUID().ToByteArray();

        sequentialGuid.ShouldNotBeEmpty();
        sequentialGuid.Length.ShouldBe(defaultGuid.Length);
    }

    [Test]
    public void When_creting_a_comb()
    {
        var defaultGuid = Guid.NewGuid().ToByteArray();
        var comb = GuidHelper.CreateComb().ToByteArray();

        comb.ShouldNotBeEmpty();
        comb.Length.ShouldBe(defaultGuid.Length);
    }
}