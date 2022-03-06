namespace Easy.Common.Tests.Unit.TypeLookup;

using NUnit.Framework;
using Shouldly;

[TestFixture]
internal sealed class TypeLookupTests
{
    [Test]
    public void When_adding_and_resolving_values()
    {
        TypeLookup<string> lookup = new();

        lookup.Count.ShouldBe(0);

        lookup.Add<int>("A");
        lookup.Add<string>("B");
        lookup.Add<int>("C");
        lookup.Add<double>(default);

        lookup.Count.ShouldBe(4);

        string value;
        lookup.TryGet<int>(out value).ShouldBeTrue();
        value.ShouldBe("C");

        lookup.TryGet<string>(out value).ShouldBeTrue();
        value.ShouldBe("B");

        lookup.TryGet<double>(out value).ShouldBeTrue();
        value.ShouldBe(default);

        lookup.TryGet<byte>(out value).ShouldBeFalse();
        value.ShouldBe(default);
    }
}