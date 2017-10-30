namespace Easy.Common.Tests.Unit.TryAndRetry
{
    using System;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    internal sealed class TryFuncTests
    {
        [Test]
        public void When_running_a_func()
        {
            Func<int> thrower = () => throw new NullReferenceException();
            Func<int> returner = () => 1;

            Should.NotThrow(() =>
            {
                Try.Handle<NullReferenceException, int>(thrower, out var result)
                    .ShouldBeFalse();
                result.ShouldBe(0);
            });

            Should.NotThrow(() =>
            {
                Try.Handle<NullReferenceException, int>(returner, out var result)
                    .ShouldBeTrue();
                result.ShouldBe(1);
            });

            Func<int> anotherThrower = () => throw new IndexOutOfRangeException();
            int result3 = -1;
            Should.Throw<IndexOutOfRangeException>(() =>
            {
                Try.Handle<NullReferenceException, int>(anotherThrower, out result3);
            });
            result3.ShouldBe(-1);
        }

        [Test]
        public void When_handling_multiple_exceptions_for_func()
        {
            Func<int> thrower = () => throw new NullReferenceException();
            Func<int> returner = () => 1;

            Exception thrownEx = null;

            Should.NotThrow(() =>
            {
                Try.HandleAny<ArgumentNullException, NullReferenceException, int>(
                    thrower, e => thrownEx = e, out var result);

                result.ShouldBe(0);
                thrownEx.ShouldNotBeNull();
                thrownEx.ShouldBeOfType<NullReferenceException>();
            });

            Should.NotThrow(() =>
            {
                thrownEx = null;
                Try.HandleAny<ArgumentNullException, NullReferenceException, int>(
                    returner, e => thrownEx = e, out var result);

                result.ShouldBe(1);
                thrownEx.ShouldBeNull();
            });

            Func<int> anotherThrower = () => throw new IndexOutOfRangeException();
            int result3 = -1;
            Exception thrown3 = null;
            Should.Throw<IndexOutOfRangeException>(() =>
            {
                Try.HandleAny<ArgumentNullException, NullReferenceException, int>(
                    anotherThrower, null, out result3);
            });

            result3.ShouldBe(-1);
            thrown3.ShouldBeNull();
        }
    }
}