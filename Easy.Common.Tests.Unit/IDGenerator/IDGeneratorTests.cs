namespace Easy.Common.Tests.Unit.IDGenerator
{
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
            var id = IDGenerator.Instance.Generate;
            
            id.ShouldNotBeNullOrWhiteSpace();
            id.Length.ShouldBe(13);
        }

        [Test]
        public void When_generating_multiple_ids()
        {
            var one = IDGenerator.Instance.Generate;
            var two = IDGenerator.Instance.Generate;

            one.ShouldNotBeNullOrWhiteSpace();
            one.Length.ShouldBe(13);

            two.ShouldNotBeNullOrWhiteSpace();
            two.Length.ShouldBe(13);

            two.ShouldBeGreaterThan(one);
        }
    }
}