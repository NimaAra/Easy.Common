namespace Easy.Common.Tests.Unit.Disposable;

using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

[TestFixture]
internal sealed class AsyncDisposableTests
{
    [Test]
    public async Task GivenADisposable_WhenDisposing_ThenHandlerShouldBeCalled()
    {
        bool handled = false;
        Func<ValueTask> someHandler = () =>
        {
            handled = true;
            return ValueTask.CompletedTask;
        };

        IAsyncDisposable sut = AsyncDisposable.Create(someHandler);

        handled.ShouldBeFalse();

        await sut.DisposeAsync();

        handled.ShouldBeTrue();
    }
}