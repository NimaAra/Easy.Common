namespace Easy.Common.Tests.Unit.IDGenerator;

using NUnit.Framework;
using Shouldly;
using IDGenerator = Easy.Common.IDGenerator;

[TestFixture]
internal sealed class IDGeneratorTests
{
    [Test]
    public void When_creating_multiple_instances_of_the_generator()
    {
        var one = IDGenerator.Instance;
        var two = IDGenerator.Instance;

        one.ShouldBeSameAs(two);
    }

    [Test]
    public void When_generating_an_id()
    {
        var id = IDGenerator.Instance.Next;
            
        id.ShouldNotBeNullOrWhiteSpace();
        id.Length.ShouldBe(20);
    }

    [Test]
    public void When_generating_multiple_ids()
    {
        var one = IDGenerator.Instance.Next;
        var two = IDGenerator.Instance.Next;

        one.ShouldNotBeNullOrWhiteSpace();
        one.Length.ShouldBe(20);

        two.ShouldNotBeNullOrWhiteSpace();
        two.Length.ShouldBe(20);

        two.ShouldBeGreaterThan(one);

        one.ShouldNotBe(two);

        one.Substring(0, 7).ShouldBe(two.Substring(0, 7));
    }
}