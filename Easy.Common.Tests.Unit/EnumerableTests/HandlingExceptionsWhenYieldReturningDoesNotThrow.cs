namespace Easy.Common.Tests.Unit.EnumerableTests;

using System;
using NUnit.Framework;
using Shouldly;

[TestFixture]
public class HandlingExceptionsWhenYieldReturningDoesNotThrow : Context
{
    private Action _action;

    [OneTimeSetUp]
    public void SetUp()
    {
        Given_a_sequence_of_integers_with_exception_handled_and_ignored();

        _action = When_enumerating_the_sequence;
    }

    [Test]
    public void Then_it_should_not_throw_any_exceptions()
    {
        Should.NotThrow(_action);
            
        IgnoredException.ShouldBeOfType<DivideByZeroException>();
        IgnoredException.Message.ShouldBe("Attempted to divide by zero.");

        Result.ShouldBe(new[] { 1 });
    }
}