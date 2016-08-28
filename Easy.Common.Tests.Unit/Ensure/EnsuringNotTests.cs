namespace Easy.Common.Tests.Unit.Ensure
{
    using System;
    using NUnit.Framework;
    using Shouldly;
    using Easy.Common;

    [TestFixture]
    public class EnsuringNotTests
    {
        [Test]
        public void When_ensuring_true_condition()
        {
            Should.Throw<ArgumentException>(() => Ensure.Not(true))
                .Message.ShouldBe("The given condition is true.");
        }

        [Test]
        public void When_ensuring_true_condition_with_custom_exception()
        {
            Should.Throw<ApplicationException>(() => Ensure.Not<ApplicationException>(true))
                .Message.ShouldBe("The given condition is true.");
        }

        [Test]
        public void When_ensuring_true_condition_with_custom_message()
        {
            Should.Throw<ArgumentException>(() => Ensure.Not(true, "Cause I say so!"))
                .Message.ShouldBe("Cause I say so!");
        }

        [Test]
        public void When_ensuring_false_condition_with_default_exception()
        {
            Should.NotThrow(() => Ensure.Not(false), "Because the condition is false");
        }

        [Test]
        public void When_ensuring_false_condition_with_custom_exception()
        {
            Should.Throw<ApplicationException>(() => Ensure.Not<ApplicationException>(true))
                .Message.ShouldBe("The given condition is true.");
        }

        [Test]
        public void When_ensuring_false_condition_with_custom_message()
        {
            Should.Throw<ApplicationException>(() => Ensure.Not<ApplicationException>(true, "Cause I say so!"))
                .Message.ShouldBe("Cause I say so!");
        }

    }
}