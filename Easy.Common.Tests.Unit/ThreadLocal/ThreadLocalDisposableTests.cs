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
            threadLocal.ToString().ShouldStartWith("Substitute.IDisposable|");
            threadLocal.Dispose();
            Should.Throw<ObjectDisposedException>(() => threadLocal.Value.ShouldBe(someDisposable))
                .Message.ShouldBe("Cannot access a disposed object.\r\nObject name: 'The ThreadLocal object has been disposed.'.");
            someDisposable.DidNotReceive().Dispose();

            var threadLocalDisposable = new ThreadLocalDisposable<IDisposable>(() => someDisposable);
            threadLocalDisposable.IsValueCreated.ShouldBeFalse();
            threadLocalDisposable.Value.ShouldBe(someDisposable);
            threadLocalDisposable.IsValueCreated.ShouldBeTrue();
            threadLocalDisposable.ToString().ShouldStartWith("Substitute.IDisposable|");
            threadLocalDisposable.Dispose();
            Should.Throw<ObjectDisposedException>(() => threadLocalDisposable.Value.ShouldBe(someDisposable))
                .Message.ShouldBe("Cannot access a disposed object.\r\nObject name: 'The ThreadLocal object has been disposed.'.");
            someDisposable.Received(1).Dispose();
        }
    }
}