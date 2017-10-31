namespace Easy.Common.Tests.Unit.TryAndRetry
{
    using System;
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    internal sealed class RetryActionTests
    {
        [Test]
        public void When_retrying_an_action_that_does_not_fail()
        {
            Should.NotThrow(async () =>
            {
                var counter = 0;
                await Retry.On<NullReferenceException>(() => counter++);
                counter.ShouldBe(1);
            });

            Should.NotThrow(async () =>
            {
                var counter = 0;
                await Retry.On<NullReferenceException>(() => counter++, 
                    100.Milliseconds(), 
                    100.Milliseconds(), 
                    100.Milliseconds());
                counter.ShouldBe(1);
            });
        }

        [Test]
        public void When_retrying_an_action_that_fails_once()
        {
            Should.NotThrow(async () =>
            {
                var counter = 0;
                await Retry.On<NullReferenceException>(() =>
                {
                    if (counter++ == 0) { throw new NullReferenceException(); }
                });
                counter.ShouldBe(2);
            });

            Should.NotThrow(async () =>
            {
                var counter = 0;
                await Retry.On<NullReferenceException>(() =>
                    {
                        if (counter++ == 0) { throw new NullReferenceException(); }
                    },
                    100.Milliseconds());
                counter.ShouldBe(2);
            });
        }

        [Test]
        public void When_retrying_an_action_that_fails_twice_but_succeeds_eventually()
        {
            Should.NotThrow(async () =>
            {
                var result = 0;
                var counter = 0;
                await Retry.On<NullReferenceException>(
                    () => {
                        if (counter++ < 2) { throw new NullReferenceException(); }
                        result = 42;
                    }, 
                    100.Milliseconds(),
                    100.Milliseconds(),
                    100.Milliseconds());

                counter.ShouldBe(3);
                result.ShouldBe(42);
            });
        }

        [Test]
        public void When_retrying_an_action_that_always_fails()
        {
            var counter = 0;

            var retryEx = Should.Throw<RetryException>(async () =>
            {
                await Retry.On<NullReferenceException>(
                    () => {
                        counter++;
                        throw new NullReferenceException();
                    });
            });
            retryEx.RetryCount.ShouldBe((uint)1);
            retryEx.Message.ShouldBe("Retry failed after: 1 attempts.");
            
            counter.ShouldBe(2);

            counter = 0;

            retryEx = Should.Throw<RetryException>(async () =>
            {
                await Retry.On<NullReferenceException>(
                    () => {
                        counter++;
                        throw new NullReferenceException();
                    },
                    100.Milliseconds(),
                    100.Milliseconds(),
                    100.Milliseconds());
            });
            retryEx.RetryCount.ShouldBe((uint)3);
            retryEx.Message.ShouldBe("Retry failed after: 3 attempts.");

            counter.ShouldBe(4);
        }

        [Test]
        public void When_retrying_an_action_that_does_not_fail_on_multiple_exceptions()
        {
            Should.NotThrow(async () =>
            {
                var counter = 0;
                await Retry.OnAny<ArgumentNullException, NullReferenceException>(() => counter++);
                counter.ShouldBe(1);
            });

            Should.NotThrow(async () =>
            {
                var counter = 0;
                await Retry.OnAny<ArgumentNullException, NullReferenceException>(() => counter++, 
                    100.Milliseconds(),
                    100.Milliseconds(),
                    100.Milliseconds());
                counter.ShouldBe(1);
            });
        }

        [Test]
        public void When_retrying_an_action_that_fails_twice_but_succeeds_eventually_on_multiple_exceptions()
        {
            Should.NotThrow(async () =>
            {
                var result = 0;
                var counter = 0;
                await Retry.OnAny<ArgumentNullException, NullReferenceException>(
                    () => {
                        if (counter++ < 2) { throw new NullReferenceException(); }
                        result = 42;
                    },
                    100.Milliseconds(),
                    100.Milliseconds(),
                    100.Milliseconds());

                counter.ShouldBe(3);
                result.ShouldBe(42);
            });
        }

        [Test]
        public void When_retrying_an_action_that_always_fails_on_multiple_exceptions()
        {
            var counter = 0;

            var retryEx = Should.Throw<RetryException>(async () =>
            {
                await Retry.OnAny<ArgumentNullException, NullReferenceException>(
                    () => {
                        counter++;
                        throw new NullReferenceException();
                    });
            });
            retryEx.RetryCount.ShouldBe((uint)1);
            retryEx.Message.ShouldBe("Retry failed after: 1 attempts.");
            
            counter.ShouldBe(2);

            counter = 0;

            retryEx = Should.Throw<RetryException>(async () =>
            {
                await Retry.OnAny<ArgumentNullException, NullReferenceException>(
                    () => {
                        counter++;
                        throw new NullReferenceException();
                    },
                    100.Milliseconds(),
                    100.Milliseconds(),
                    100.Milliseconds());
            });
            retryEx.RetryCount.ShouldBe((uint)3);
            retryEx.Message.ShouldBe("Retry failed after: 3 attempts.");

            counter.ShouldBe(4);
        }
    }
}