namespace Easy.Common.Tests.Unit.GenericExtensions;

using Easy.Common.Extensions;
using NUnit.Framework;
using Shouldly;

[TestFixture]
internal sealed class ToCompletedValueTaskTests
{
    [Test]
    public void When_creating_a_completed_task_from_reference_type()
    {
        const string SOURCE = "some-text";

        var task = SOURCE.ToCompletedValueTask();
        task.IsCompleted.ShouldBeTrue();
        task.IsCompletedSuccessfully.ShouldBeTrue();

        task.Result.ShouldBe(SOURCE);
    }

    [Test]
    public void When_creating_a_completed_task_from_value_type()
    {
        const int SOURCE = 42;

        var task = SOURCE.ToCompletedValueTask();
        task.IsCompleted.ShouldBeTrue();
        task.IsCompletedSuccessfully.ShouldBeTrue();

        task.Result.ShouldBe(SOURCE);
    }

    [Test]
    public void When_creating_a_completed_task_from_null_value()
    {
        const string SOURCE = null;

        var task = SOURCE.ToCompletedValueTask();
        task.IsCompleted.ShouldBeTrue();
        task.IsCompletedSuccessfully.ShouldBeTrue();

        task.Result.ShouldBe(SOURCE);
    }
}