namespace Easy.Common.Tests.Unit.ThreadLocal
{
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
            threadLocal.ToString().ShouldBe("Castle.Proxies.IDisposableProxy");
            threadLocal.Dispose();
            Should.Throw<ObjectDisposedException>(() => threadLocal.Value.ShouldBe(someDisposable))
                .Message.ShouldBe("Cannot access a disposed object.\r\nObject name: 'The ThreadLocal object has been disposed.'.");
            someDisposable.DidNotReceive().Dispose();

            var threadLocalDisposeable = new ThreadLocalDisposable<IDisposable>(() => someDisposable);
            threadLocalDisposeable.IsValueCreated.ShouldBeFalse();
            threadLocalDisposeable.Value.ShouldBe(someDisposable);
            threadLocalDisposeable.IsValueCreated.ShouldBeTrue();
            threadLocalDisposeable.ToString().ShouldBe("Castle.Proxies.IDisposableProxy");
            threadLocalDisposeable.Dispose();
            Should.Throw<ObjectDisposedException>(() => threadLocalDisposeable.Value.ShouldBe(someDisposable))
                .Message.ShouldBe("Cannot access a disposed object.\r\nObject name: 'The ThreadLocal object has been disposed.'.");
            someDisposable.Received(1).Dispose();
        }
    }
}