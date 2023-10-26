namespace Easy.Common.Tests.Unit.PredicateBuilder;

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Shouldly;
using Easy.Common;

[TestFixture]
public sealed class PredicateBuilderTests
{
    private List<int> _numbers;

    [OneTimeSetUp]
    public void Given_a_list_of_numbers()
    {
        _numbers = new List<int> {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
    }

    [Test]
    public void When_creating_a_predicate()
    {
        var predicate = PredicateBuilder.Create<int>(n => n >= 8).Compile();
        var result = _numbers.Where(predicate);

        result.ShouldBe(new [] {8, 9, 10});
    }

    [Test]
    public void When_creating_a_not_predicate()
    {
        var predicate = PredicateBuilder.Not<int>(n => n >= 8).Compile();
        var result = _numbers.Where(predicate);

        result.ShouldBe(new[] { 1, 2, 3, 4, 5, 6, 7 });
    }

    [Test]
    public void When_creating_a_true_predicate()
    {
        var predicate = PredicateBuilder.True<int>().Compile();

        var result = _numbers.Where(predicate);

        result.ShouldBe(new[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10});
    }

    [Test]
    public void When_creating_a_false_predicate()
    {
        var predicate = PredicateBuilder.False<int>().Compile();

        var result = _numbers.Where(predicate);

        result.ShouldBe(Enumerable.Empty<int>());
    }

    [Test]
    public void When_creating_an_and_predicate()
    {
        var predicate = PredicateBuilder.True<int>()
            .And(i => i > 5)
            .Compile();

        var result = _numbers.Where(predicate);

        result.ShouldBe(new [] {6, 7, 8, 9, 10});
    }

    [Test]
    public void When_creating_an_or_predicate()
    {
        var predicate = PredicateBuilder.False<int>()
            .Or(i => i > 5)
            .Compile();

        var result = _numbers.Where(predicate);

        result.ShouldBe(new[] { 6, 7, 8, 9, 10 });
    }

    [Test]
    public void When_creating_a_mixed_predicate()
    {
        var predicate = PredicateBuilder.False<int>()
            .Or(i => i > 5)
            .And(i => i > 8)
            .Compile();

        var result = _numbers.Where(predicate);

        result.ShouldBe(new[] { 9, 10 });
    }
}