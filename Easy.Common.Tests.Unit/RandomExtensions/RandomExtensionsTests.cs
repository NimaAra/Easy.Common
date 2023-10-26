namespace Easy.Common.Tests.Unit.RandomExtensions;

using System;
using Easy.Common.Extensions;
using NUnit.Framework;
using Shouldly;

[TestFixture]
internal sealed class RandomExtensionsTests
{
    [Test]
    public void When_generating_random_double_between_two_given_numbers()
    {
        var random = new Random();
        random.GenerateRandomBetween(1, 2).ShouldBeInRange(1, 2);
        random.GenerateRandomBetween(1, 1).ShouldBe(1);
    }

    [Test]
    public void When_generating_random_double_between_two_invalid_numbers()
    {
        var random = new Random();
        Should.Throw<ArgumentException>(() => random.GenerateRandomBetween(2, 1))
            .Message.ShouldBe("min: 2 should be less than max: 1");
    }

    [Test]
    public void When_generating_random_sequence()
    {
        var random = new Random();
        var randomNubmers = random.GenerateRandomSequence(5, 1, 10);
        randomNubmers.ShouldNotBeEmpty();
        randomNubmers.Length.ShouldBe(5);
        randomNubmers.ForEach(n => n.ShouldBeInRange(1, 10));
    }

    [Test]
    public void When_generating_random_sequence_with_invalid_arguments()
    {
        var random = new Random();
        Should.Throw<ArgumentException>(() => random.GenerateRandomSequence(5, 1, 2))
            .Message.ShouldBe("The given range of: 1 to 2 (1 value(s)), with the count of: 5 is illegal.");
    }
}