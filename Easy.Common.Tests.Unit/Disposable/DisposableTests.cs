namespace Easy.Common.Tests.Unit.Disposable;

using System;
using NUnit.Framework;
using Shouldly;
using Disposable = Easy.Common.Disposable;

[TestFixture]
internal sealed class DisposableTests
{
    [Test]
    public void GivenADisposable_WhenDisposing_ThenHandlerShouldBeCalled()
    {
        bool handled = false;
        Action someHandler = () => handled = true;

        IDisposable sut = Disposable.Create(someHandler);

        handled.ShouldBeFalse();

        sut.Dispose();

        handled.ShouldBeTrue();
    }
}