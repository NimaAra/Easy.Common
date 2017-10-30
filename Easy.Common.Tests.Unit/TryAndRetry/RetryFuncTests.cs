namespace Easy.Common.Tests.Unit.TryAndRetry
{
    using System;
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    internal sealed class RetryFuncTests
    {
        [Test]
        public void When_retrying_a_func_that_does_not_fail()
        {
            Should.NotThrow(async () =>
            {
                var counter = 0;
                var result = await Retry.On<NullReferenceException, int>(
                    () => {
                        counter++;
                        return 42;
                    });

                counter.ShouldBe(1);
                result.ShouldBe(42);
            });

            Should.NotThrow(async () =>
            {
                var counter = 0;
                var result = await Retry.On<NullReferenceException, int>(
                    () => {
                        counter++;
                        return 42;
                    }, 
                    100.Milliseconds(),
                    100.Milliseconds(),
                    100.Milliseconds());

                counter.ShouldBe(1);
                result.ShouldBe(42);
            });
        }

        [Test]
        public void When_retrying_a_func_that_fails_once()
        {
            Should.NotThrow(async () =>
            {
                var counter = 0;
                var result = await Retry.On<NullReferenceException, int>(
                    () => {
                        if (counter++ == 0) { throw new NullReferenceException(); }
                        return 42;
                    });

                counter.ShouldBe(2);
                result.ShouldBe(42);
            });

            Should.NotThrow(async () =>
            {
                var counter = 0;
                var result = await Retry.On<NullReferenceException, int>(
                    () => {
                        if (counter++ == 0) { throw new NullReferenceException(); }
                        return 42;
                    },
                    100.Milliseconds());

                counter.ShouldBe(2);
                result.ShouldBe(42);
            });
        }

        [Test]
        public void When_retrying_a_func_that_fails_twice_but_succeeds_eventually()
        {
            Should.NotThrow(async () =>
            {
                var counter = 0;
                var result = await Retry.On<NullReferenceException, int>(
                    () => {
                        if (counter++ < 2)
                        { throw new NullReferenceException(); }
                        return 42;
                    },
                    100.Milliseconds(),
                    100.Milliseconds(),
                    100.Milliseconds());

                counter.ShouldBe(3);
                result.ShouldBe(42);
            });
        }

        [Test]
        public void When_retrying_a_func_that_always_fails()
        {
            var result = -1;
            var counter = 0;
            Should.Throw<NullReferenceException>(async () =>
            {
                result = await Retry.On<NullReferenceException, int>(
                    () => {
                        counter++;
                        throw new NullReferenceException();
                    },
                    100.Milliseconds(),
                    100.Milliseconds(),
                    100.Milliseconds());
            });

            counter.ShouldBe(4);
            result.ShouldBe(-1);
        }

        [Test]
        public void When_retrying_a_func_that_does_not_fail_on_multiple_exceptions()
        {
            Should.NotThrow(async () =>
            {
                var counter = 0;
                var result = await Retry.OnAny<ArgumentNullException, NullReferenceException, int>(
                    () => {
                        counter++;
                        return 42;
                    },
                    100.Milliseconds(),
                    100.Milliseconds(),
                    100.Milliseconds());

                counter.ShouldBe(1);
                result.ShouldBe(42);
            });
        }

        [Test]
        public void When_retrying_a_func_that_fails_twice_but_succeeds_eventually_on_multiple_exceptions()
        {
            Should.NotThrow(async () =>
            {
                var counter = 0;
                var result = await Retry.OnAny<ArgumentNullException, NullReferenceException, int>(
                    () => {
                        if (counter++ < 2) { throw new NullReferenceException(); }
                        return 42;
                    },
                    100.Milliseconds(),
                    100.Milliseconds(),
                    100.Milliseconds());

                counter.ShouldBe(3);
                result.ShouldBe(42);
            });
        }

        [Test]
        public void When_retrying_a_func_that_always_fails_on_multiple_exceptions()
        {
            var result = -1;
            var counter = 0;
            Should.Throw<NullReferenceException>(async () =>
            {
                result = await Retry.OnAny<ArgumentNullException, NullReferenceException, int>(
                    () => {
                        counter++;
                        throw new NullReferenceException();
                    },
                    100.Milliseconds(),
                    100.Milliseconds(),
                    100.Milliseconds());
            });

            counter.ShouldBe(4);
            result.ShouldBe(-1);
        }
    }
}