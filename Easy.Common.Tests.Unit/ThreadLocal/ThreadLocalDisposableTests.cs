namespace Easy.Common.Tests.Unit.ThreadLocal;

using System;
using System.Threading;
using NSubstitute;
using NUnit.Framework;
using Shouldly;

[TestFixture]
internal sealed class ThreadLocalDisposableTests
{
    [Test]
    public void Run()
    {
        var someDisposable = Substitute.For<IDisposable>();
        someDisposable.DidNotReceive().Dispose();

        var threadLocal = new ThreadLocal<IDisposable>(() => someDisposable);
        threadLocal.IsValueCreated.ShouldBeFalse();
        threadLocal.Value.ShouldBe(someDisposable);
        threadLocal.IsValueCreated.ShouldBeTrue();
        threadLocal.ToString().ShouldStartWith("Substitute.IDisposable|");
        threadLocal.Dispose();
        Should.Throw<ObjectDisposedException>(() => threadLocal.Value.ShouldBe(someDisposable))
            .Message.ShouldBe("Cannot access a disposed object.\r\nObject name: 'System.Threading.ThreadLocal`1[[System.IDisposable, System.Private.CoreLib, Version=8.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]'.");
        someDisposable.DidNotReceive().Dispose();

        var threadLocalDisposable = new ThreadLocalDisposable<IDisposable>(() => someDisposable);
        threadLocalDisposable.IsValueCreated.ShouldBeFalse();
        threadLocalDisposable.Value.ShouldBe(someDisposable);
        threadLocalDisposable.IsValueCreated.ShouldBeTrue();
        threadLocalDisposable.ToString().ShouldStartWith("Substitute.IDisposable|");
        threadLocalDisposable.Dispose();
        Should.Throw<ObjectDisposedException>(() => threadLocalDisposable.Value.ShouldBe(someDisposable))
            .Message.ShouldBe("Cannot access a disposed object.\r\nObject name: 'System.Threading.ThreadLocal`1[[System.IDisposable, System.Private.CoreLib, Version=8.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]'.");
        someDisposable.Received(1).Dispose();
    }
}