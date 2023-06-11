namespace Easy.Common.Tests.Unit.AsyncSemaphore;

using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;
using AsyncSemaphore = Easy.Common.AsyncSemaphore;

[TestFixture]
internal sealed class AsyncSemaphoreTests
{
    [Test]
    public async Task GivenAnAsyncSemaphore_WhenMultipleThreadsAttemptToAcquireLock_ThenOnlyThoseAllowedSucceed()
    {
        TimeSpan timeoutDuration = TimeSpan.FromMilliseconds(50);

        AsyncSemaphore sem = new(2);

        Task worker1 = GetWorker();
        Task worker2 = GetWorker();
        Task worker3 = GetWorker();

        TimeoutException ex = await Should.ThrowAsync<TimeoutException>(Task.WhenAll(worker1, worker2, worker3));
        ex.Message.ShouldBe("The request to semaphore timed out after: " + timeoutDuration);
        
        worker1.Status.ShouldBe(TaskStatus.RanToCompletion);
        worker2.Status.ShouldBe(TaskStatus.RanToCompletion);
        worker3.Status.ShouldBe(TaskStatus.Faulted);

        async Task GetWorker()
        {
            using IDisposable _ = await sem.Acquire(timeoutDuration);
            await Task.Delay(TimeSpan.FromMilliseconds(70));
        }
    }

    [Test]
    public async Task GivenAnAsyncLock_WhenMultipleThreadsAttemptToAcquireLock_ThenOnlyOneSucceeds()
    {
        TimeSpan timeoutDuration = TimeSpan.FromMilliseconds(50);

        AsyncLock locker = new();

        Task worker1 = GetWorker();
        Task worker2 = GetWorker();
        Task worker3 = GetWorker();

        TimeoutException ex = await Should.ThrowAsync<TimeoutException>(Task.WhenAll(worker1, worker2, worker3));
        ex.Message.ShouldBe("The request to semaphore timed out after: " + timeoutDuration);
        
        worker1.Status.ShouldBe(TaskStatus.RanToCompletion);
        worker2.Status.ShouldBe(TaskStatus.Faulted);
        worker3.Status.ShouldBe(TaskStatus.Faulted);

        async Task GetWorker()
        {
            using IDisposable _ = await locker.Acquire(timeoutDuration);
            await Task.Delay(TimeSpan.FromMilliseconds(70));
        }
    }
}