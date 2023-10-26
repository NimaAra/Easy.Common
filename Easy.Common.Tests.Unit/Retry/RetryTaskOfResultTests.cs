#pragma warning disable 162
namespace Easy.Common.Tests.Unit.Retry;

using System;
using System.Threading;
using System.Threading.Tasks;
using Easy.Common.Extensions;
using NUnit.Framework;
using Shouldly;
using Retry = Easy.Common.Retry;

[TestFixture]
internal sealed class RetryTaskOfResultTests
{
    [Test]
    public void When_retrying_a_task_of_result_that_does_not_fail()
    {
        Should.NotThrow(async () =>
        {
            var counter = 0;
            var result = await Retry.On<NullReferenceException, int>(async () => {
                await Task.Yield();    
                counter++;
                return 42;

            });

            counter.ShouldBe(1);
            result.ShouldBe(42);
        });

        Should.NotThrow(async () =>
        {
            var counter = 0;
            var result = await Retry.On<NullReferenceException, int>(async () => {
                    await Task.Yield();
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
    public void When_retrying_a_task_of_result_that_fails_once()
    {
        Should.NotThrow(async () =>
        {
            var counter = 0;
            var result = await Retry.On<NullReferenceException, int>(async () => {
                await Task.Yield();    
                if (counter++ == 0) { throw new NullReferenceException(); }
                return 42;
            });

            counter.ShouldBe(2);
            result.ShouldBe(42);
        });

        Should.NotThrow(async () =>
        {
            var counter = 0;
            var result = await Retry.On<NullReferenceException, int>(async () => {
                    await Task.Yield();
                    if (counter++ == 0) { throw new NullReferenceException(); }
                    return 42;
                },
                100.Milliseconds());

            counter.ShouldBe(2);
            result.ShouldBe(42);
        });
    }

    [Test]
    public void When_retrying_a_task_of_result_that_fails_twice_but_succeeds_eventually()
    {
        Should.NotThrow(async () =>
        {
            var counter = 0;
            var result = await Retry.On<NullReferenceException, int>(async () => {
                    await Task.Yield();
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
    public void When_retrying_a_task_of_result_that_always_fails()
    {
        var result = -1;
        var counter = 0;
        var retryEx = Should.Throw<RetryException>(async () =>
        {
            result = await Retry.On<NullReferenceException, int>(() => {
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
        result.ShouldBe(-1);

        result = -1;
        counter = 0;
        retryEx = Should.Throw<RetryException>(async () =>
        {
            result = await Retry.On<NullReferenceException, int>(() => {
                counter++;
                throw new NullReferenceException();
            });
        });
        retryEx.RetryCount.ShouldBe((uint)1);
        retryEx.Message.ShouldBe("Retry failed after: 1 attempts.");

        counter.ShouldBe(2);
        result.ShouldBe(-1);
    }

    [Test]
    public void When_retrying_a_task_of_result_that_does_not_fail_on_multiple_exceptions()
    {
        Should.NotThrow(async () =>
        {
            var counter = 0;
            var result = await Retry.OnAny<ArgumentNullException, NullReferenceException, int>(async () => {
                    await Task.Yield();
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
    public void When_retrying_a_task_of_result_that_fails_twice_but_succeeds_eventually_on_multiple_exceptions()
    {
        Should.NotThrow(async () =>
        {
            var counter = 0;
            var result = await Retry.OnAny<ArgumentNullException, NullReferenceException, int>(async () => {
                    await Task.Yield();
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
    public void When_retrying_a_task_of_result_that_always_fails_on_multiple_exceptions()
    {
        var result = -1;
        var counter = 0;
        var retryEx = Should.Throw<RetryException>(async () =>
        {
            result = await Retry.OnAny<ArgumentNullException, NullReferenceException, int>(() => {
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
        result.ShouldBe(-1);
    }

    [Test]
    public void When_retrying_a_task_that_throws_aggregate_exception_one()
    {
        var counter = 0;
            
        var retryEx = Should.Throw<RetryException>(async () =>
        {
            await Retry.On<ArgumentNullException>(
                () => Task.Factory.StartNew(() =>
                {
                    counter++;
                    var inner = new ArgumentNullException("someArg");
                    throw new AggregateException(inner);
                    return 1;
                }),
                100.Milliseconds(),
                100.Milliseconds(),
                100.Milliseconds());
        });

        retryEx.RetryCount.ShouldBe((uint)3);
        retryEx.InnerException.ShouldBeOfType<AggregateException>();
        retryEx.Message.ShouldBe("Retry failed after: 3 attempts.");

        counter.ShouldBe(4);
    }

    [Test]
    public void When_retrying_a_task_that_throws_aggregate_exception_two()
    {
        var counter = 0;
            
        var retryEx = Should.Throw<RetryException>(async () =>
        {
            await Retry.On<IndexOutOfRangeException>(
                () => Task.Factory.StartNew(() =>
                {
                    counter++;
                    var inner1 = new ArgumentNullException("someArg1");
                    var inner2 = new IndexOutOfRangeException("someArg2");
                    throw new AggregateException(inner1, inner2);
                    return 1;
                }),
                100.Milliseconds(),
                100.Milliseconds(),
                100.Milliseconds());
        });

        retryEx.RetryCount.ShouldBe((uint)3);
        retryEx.InnerException.ShouldBeOfType<AggregateException>();
        retryEx.Message.ShouldBe("Retry failed after: 3 attempts.");

        counter.ShouldBe(4);
    }

    [Test]
    public void When_retrying_a_task_that_throws_aggregate_exception_and_no_expected_exception()
    {
        var counter = 0;
            
        Should.Throw<IndexOutOfRangeException>(async () =>
        {
            await Retry.On<ArgumentNullException>(
                () => Task.Factory.StartNew(() =>
                {
                    counter++;
                    var inner = new IndexOutOfRangeException();
                    throw new AggregateException(inner);
                    return 1;
                }),
                100.Milliseconds(),
                100.Milliseconds(),
                100.Milliseconds());
        });

        counter.ShouldBe(1);
    }

    [Test]
    public void When_retrying_a_task_with_a_predicate_returning_true()
    {
        var predicateCounter = 0;
            
        Func<Exception, bool> exceptionPredicate = e =>
        {
            e.ShouldBeOfType<ArgumentException>();
            predicateCounter++;
            return true;
        };

        var executionCounter = 0;

        Func<Task<int>> task = () => Task.Run(() =>
        {
            executionCounter++;
            throw new ArgumentException();
            return 1;
        });

        var retryEx = Should.Throw<RetryException>(async () =>
        {
            await Retry.On(task, exceptionPredicate);
        });

        retryEx.RetryCount.ShouldBe((uint)1);
        retryEx.InnerException.ShouldBeOfType<ArgumentException>();
        retryEx.Message.ShouldBe("Retry failed after: 1 attempts.");

        executionCounter.ShouldBe(2);
        predicateCounter.ShouldBe(1);
    }

    [Test]
    public void When_retrying_a_task_with_a_predicate_returning_false()
    {
        var predicateCounter = 0;
            
        Func<Exception, bool> exceptionPredicate = e =>
        {
            e.ShouldBeOfType<ArgumentException>();
            predicateCounter++;
            return false;
        };

        var executionCounter = 0;
        Func<Task<int>> task = () => Task.Run(() =>
        {
            executionCounter++;
            throw new ArgumentException();
            return 1;
        });

        Should.Throw<ArgumentException>(async () =>
        {
            await Retry.On(task, exceptionPredicate);
        });

        executionCounter.ShouldBe(1);
        predicateCounter.ShouldBe(1);
    }

    [Test]
    public void When_retrying_a_task_with_delay_factory()
    {
        var cts = new CancellationTokenSource();
            
        var predicateCounter = 0;
            
        Func<Exception, bool> exceptionPredicate = e =>
        {
            e.ShouldBeOfType<ArgumentException>();
            predicateCounter++;
            return true;
        };
            
        Func<uint, TimeSpan> delayFactory = failureCount =>
        {
            if (failureCount == 4)
            {
                cts.Cancel();
                return 0.Seconds();
            }

            return Sigmoid(failureCount);
        };

        var executionCounter = 0;
            
        var retryEx = Should.Throw<RetryException>(async () =>
        {
            await Retry.On(async () =>
            {
                await Task.Delay(1);
                executionCounter++;
                throw new ArgumentException();
                return 1;
            }, exceptionPredicate, delayFactory, cts.Token);
        });

        retryEx.RetryCount.ShouldBe((uint)3);
        retryEx.InnerException.ShouldBeOfType<ArgumentException>();
        retryEx.Message.ShouldBe("Retry failed after: 3 attempts.");

        executionCounter.ShouldBe(4);
        predicateCounter.ShouldBe(4);
    }

    private static Func<uint, TimeSpan> Sigmoid = x => TimeSpan.FromMilliseconds(
        Convert.ToInt32(Math.Round((1 / (1 + Math.Exp(-x + 5))) * 100)) * 100);
}