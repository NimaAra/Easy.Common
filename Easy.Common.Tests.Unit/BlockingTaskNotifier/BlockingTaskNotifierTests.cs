namespace Easy.Common.Tests.Unit.BlockingTaskNotifier;

using Easy.Common.Extensions;
using NUnit.Framework;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using BlockingTaskNotifier = Easy.Common.BlockingTaskNotifier;

[TestFixture]
internal sealed class BlockingTaskNotifierTests
{
    [Test]
    public async Task Run()
    {
        object sender = null;
        string stackTrace = null;
        int count = 0;

        BlockingTaskNotifier.OnDetection += (s, st) =>
        {
            sender = s;
            stackTrace = st;
            count++;
        };

        BlockingTaskNotifier.Start();

        await Task.Delay(1.Seconds());
        Task task = Task.Delay(1.Seconds());

        while (!task.IsCompleted)
        {
            Thread.Sleep(10);
        }

        sender.ShouldNotBeNull();
        sender.ShouldBeOfType<BlockingTaskNotifier>();
        stackTrace.ShouldNotBeNullOrEmpty();
        stackTrace.ShouldContain("at System.Environment.get_StackTrace()");
        count.ShouldBe(1);
    }
}