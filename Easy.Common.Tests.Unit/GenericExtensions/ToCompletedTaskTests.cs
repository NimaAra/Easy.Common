namespace Easy.Common.Tests.Unit.GenericExtensions;

using System.Threading.Tasks;
using Easy.Common.Extensions;
using NUnit.Framework;
using Shouldly;

[TestFixture]
internal sealed class ToCompletedTaskTests
{
    [Test]
    public void When_creating_a_completed_task_from_reference_type()
    {
        const string SOURCE = "some-text";

        var task = SOURCE.ToCompletedTask();
        task.Status.ShouldBe(TaskStatus.RanToCompletion);
            
        task.Result.ShouldBe(SOURCE);
    }

    [Test]
    public void When_creating_a_completed_task_from_value_type()
    {
        const int SOURCE = 42;

        var task = SOURCE.ToCompletedTask();
        task.Status.ShouldBe(TaskStatus.RanToCompletion);

        task.Result.ShouldBe(SOURCE);
    }

    [Test]
    public void When_creating_a_completed_task_from_null_value()
    {
        const string SOURCE = null;

        var task = SOURCE.ToCompletedTask();
        task.Status.ShouldBe(TaskStatus.RanToCompletion);

        task.Result.ShouldBe(SOURCE);
    }
}