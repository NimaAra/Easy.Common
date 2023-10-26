namespace Easy.Common.Tests.Unit.Guid;

using System;
using Easy.Common.Extensions;
using NUnit.Framework;
using Shouldly;

[TestFixture]
internal sealed class GuidExtensionsTests
{
    [Test]
    public void When_getting_guid_as_base64_short_code()
    {
        var defaultGuid = Guid.NewGuid();
        var shortBase64GuidTrimmed = defaultGuid.AsShortCodeBase64();

        shortBase64GuidTrimmed.ShouldNotBeNullOrWhiteSpace();
        shortBase64GuidTrimmed.ShouldNotEndWith("=");
        shortBase64GuidTrimmed.ShouldNotEndWith("==");

        shortBase64GuidTrimmed.ToGuid().ShouldBe(defaultGuid);


        var shortBase64GuidUntrimmed = defaultGuid.AsShortCodeBase64(false);

        shortBase64GuidUntrimmed.ShouldNotBeNullOrWhiteSpace();
        shortBase64GuidUntrimmed.ShouldEndWith("=");
        shortBase64GuidUntrimmed.ShouldEndWith("==");

        shortBase64GuidUntrimmed.ToGuid(false).ShouldBe(defaultGuid);
    }

    [Test]
    public void When_getting_guid_as_short_code()
    {
        var defaultGuid = Guid.NewGuid();
        var shortCodeGuid = defaultGuid.AsShortCode();

        shortCodeGuid.ShouldNotBeNullOrWhiteSpace();
        shortCodeGuid.Length.ShouldBeGreaterThanOrEqualTo(15);
    }

    [Test]
    public void When_getting_guid_as_number()
    {
        var defaultGuid = Guid.NewGuid();
        var numberGuid = defaultGuid.AsNumber();

        numberGuid.ShouldBeGreaterThan(0);
        numberGuid.ToString().Length.ShouldBe(19);
    }
}