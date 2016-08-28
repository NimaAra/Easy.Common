namespace Easy.Common.Tests.Unit.Ensure
{
    using System;
    using NUnit.Framework;
    using Shouldly;
    using Easy.Common;

    [TestFixture]
    public class EnsuringThatTests
    {
        [Test]
        public void When_ensuring_true_condition()
        {
            Should.NotThrow(() => Ensure.That(true), "Because the condition is true");
        }

        [Test]
        public void When_ensuring_true_condition_with_custom_exception()
        {
            Should.NotThrow(() => Ensure.That<ApplicationException>(true), "Because the condition is true");
        }

        [Test]
        public void When_ensuring_true_condition_with_custom_message()
        {
            Should.NotThrow(() => Ensure.That(true), "Because the condition is true");
        }

        [Test]
        public void When_ensuring_false_condition_with_default_exception()
        {
            Should.Throw<ArgumentException>(() => Ensure.That(false), "Because the condition is false")
                .Message.ShouldBe("The given condition is false.");
        }

        [Test]
        public void When_ensuring_false_condition_with_custom_exception()
        {
            Should.Throw<ApplicationException>(() => Ensure.That<ApplicationException>(false))
                .Message.ShouldBe("The given condition is false.");
        }

        [Test]
        public void When_ensuring_false_condition_with_custom_message()
        {
            Should.Throw<ApplicationException>(() => Ensure.That<ApplicationException>(false, "Cause I say so!"), "Because the condition is false")
                .Message.ShouldBe("Cause I say so!");
        }
    }
}