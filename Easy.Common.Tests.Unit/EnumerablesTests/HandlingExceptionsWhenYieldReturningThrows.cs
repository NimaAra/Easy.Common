namespace Easy.Common.Tests.Unit.EnumerablesTests
{
    using System;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public class HandlingExceptionsWhenYieldReturningThrows : Context
    {
        private Action _action;

        [OneTimeSetUp]
        public void SetUp()
        {
            Given_a_sequence_of_integers_with_exception_handled_and_wrapped();

            _action = When_enumerating_the_sequence;
        }

        [Test]
        public void Then_it_should_throw_the_correct_exception()
        {
            var exception = Should.Throw<InvalidOperationException>(_action);
                
            exception.Message.ShouldBe("Custom message");
            exception.InnerException.ShouldBeOfType<DivideByZeroException>()
                .Message.ShouldBe("Attempted to divide by zero.");
            
            Result.ShouldBeNull();
        }
    }
}