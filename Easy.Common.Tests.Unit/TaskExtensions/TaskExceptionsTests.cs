namespace Easy.Common.Tests.Unit.TaskExtensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public class TaskExceptionsTests
    {
        [Test]
        public void ShouldIgnoreNoException()
        {
            Action action = () =>
            {
                var t1 = Task.Factory.StartNew(() =>
                {
                    try { throw new DivideByZeroException(); } catch (Exception) { /* Ignore */ }
                }).IgnoreExceptions();

                t1.Wait();
            };

            action.ShouldNotThrow();
        }
        
        [Test]
        public void ShouldIgnoreNoExceptionWithChildTask()
        {
            Action action = () =>
            {
                var t1 = Task.Factory.StartNew(() =>
                {
                    try { throw new DivideByZeroException(); } catch (Exception) { /* Ignore */ }

                    Task.Factory.StartNew(() =>
                    {
                        try { throw new DivideByZeroException(); } catch (Exception) { /* Ignore */ }
                    }, TaskCreationOptions.AttachedToParent);
                }).IgnoreExceptions();

                t1.Wait();
            };

            action.ShouldNotThrow();
        }

        [Test]
        public void ShouldIgnoreCancelledTask()
        {
            Action action = () =>
            {
                var cts = new CancellationTokenSource();
                var t1 = Task.Factory.StartNew(() =>
                {
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                }, cts.Token).IgnoreExceptions();

                t1.Wait();
            };

            action.ShouldNotThrow();
        }

        [Test]
        public void ShouldIgnoreSingleException()
        {
            Action action = () =>
            {
                var t1 = Task.Factory.StartNew(() =>
                {
                    throw new DivideByZeroException();
                }).IgnoreExceptions();

                t1.Wait();
            };

            action.ShouldNotThrow();
        }
        
        [Test]
        public void ShouldIgnoreSingleExceptionWithChildTask()
        {
            Action action = () =>
            {
                var t1 = Task.Factory.StartNew(() =>
                {
                    Task.Factory.StartNew(() => {throw new InvalidOperationException();}, TaskCreationOptions.AttachedToParent);
                    throw new DivideByZeroException();
                }).IgnoreExceptions();

                t1.Wait();
            };

            action.ShouldNotThrow();
        }

        [Test]
        public void ShouldIgnoreMultipleExceptions()
        {
            Action action = () =>
            {
                var t1 = Task.Factory.StartNew(() =>
                {
                    Task.Factory.StartNew(() => { throw new InvalidOperationException(); });
                    Task.Factory.StartNew(() => { throw new InvalidCastException(); }, TaskCreationOptions.AttachedToParent);

                    throw new DivideByZeroException();
                }).IgnoreExceptions();

                t1.Wait();
            };

            action.ShouldNotThrow();
        }

        [Test]
        public void ShouldHandleExpectedExceptionWhenNoException()
        {
            var exceptionsQueue = new Queue<Exception>();

            Action action = () =>
            {
                var t1 = Task.Factory.StartNew(() =>
                {
                    try { throw new InvalidOperationException(); } catch (Exception) { /* Ignore */ }

                }).HandleException<InvalidOperationException>(e => { exceptionsQueue.Enqueue(e); });

                t1.Wait();
            };

            action.ShouldNotThrow();

            exceptionsQueue.ShouldBeEmpty();
        }

        [Test]
        public void ShouldHandleExpectedExceptionNonPresentException()
        {
            var exceptionsQueue = new Queue<Exception>();

            Action action = () =>
            {
                var t1 = Task.Factory.StartNew(() => { throw new DivideByZeroException(); })
                    .HandleException<InvalidOperationException>(
                        e => { exceptionsQueue.Enqueue(e); });

                t1.Wait();
            };

            action.ShouldThrow<AggregateException>()
                .Flatten().InnerExceptions
                .Count(e => e.GetType() == typeof(DivideByZeroException) && e.Message == "Attempted to divide by zero.")
                .ShouldBe(1);

            exceptionsQueue.ShouldBeEmpty();
        }

        [Test]
        public void ShouldHandleExpectedExceptionSingleException()
        {
            var exceptionsQueue = new Queue<Exception>();

            Action action = () =>
            {
                var t1 = Task.Factory.StartNew(() => { throw new DivideByZeroException(); })
                    .HandleException<DivideByZeroException>(
                        e => { exceptionsQueue.Enqueue(e); });

                t1.Wait();
            };

            action.ShouldNotThrow();

            exceptionsQueue.Count.ShouldBe(1);
            exceptionsQueue.Dequeue().ShouldBeOfType<DivideByZeroException>();
            exceptionsQueue.ShouldBeEmpty();
        }

        [Test]
        public void ShouldHandleExpectedExceptionMultipleExceptions()
        {
            var exceptionsQueue = new Queue<Exception>();

            Action action = () =>
            {
                var t1 = Task.Factory.StartNew(() =>
                {
                    Task.Factory.StartNew(() => { throw new InvalidOperationException(); }, TaskCreationOptions.AttachedToParent);
                    throw new DivideByZeroException();
                }).HandleException<DivideByZeroException>(
                        e => { exceptionsQueue.Enqueue(e); });

                t1.Wait();
            };

            action.ShouldThrow<AggregateException>()
                .Flatten().InnerExceptions[0]
                    .ShouldBeOfType<InvalidOperationException>();

            exceptionsQueue.Count.ShouldBe(1);
            exceptionsQueue.Dequeue().ShouldBeOfType<DivideByZeroException>();
            exceptionsQueue.ShouldBeEmpty();
        }

        [Test]
        public void ShouldHandleAllExceptionsWhenNoException()
        {
            var exceptionsQueue = new Queue<Exception>();

            Action action = () =>
            {
                var t1 = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        throw new InvalidOperationException();
                    } catch (Exception) { /* Ignore */ }
                })
                .HandleExceptions(e => { exceptionsQueue.Enqueue(e); });

                t1.Wait();
            };

            action.ShouldNotThrow();
            exceptionsQueue.ShouldBeEmpty();
        }

        [Test]
        public void ShouldHandleAllExceptionsWhenSingleException()
        {
            var exceptionsQueue = new Queue<Exception>();

            Action action = () =>
            {
                var t1 = Task.Factory.StartNew(() => { throw new DivideByZeroException(); })
                    .HandleExceptions(e => { exceptionsQueue.Enqueue(e); });

                t1.Wait();
            };

            action.ShouldNotThrow();
            exceptionsQueue.Count.ShouldBe(1);
            exceptionsQueue.Dequeue().ShouldBeOfType<DivideByZeroException>();
        }

        [Test]
        public void ShouldHandleAllExceptionsWhenMultipleException()
        {
            var exceptionsQueue = new Queue<Exception>();

            Action action = () =>
            {
                var t1 = Task.Factory.StartNew(() =>
                {
                    Task.Factory.StartNew(() => { throw new InvalidOperationException(); }, TaskCreationOptions.AttachedToParent);
                    throw new DivideByZeroException();
                }).HandleExceptions(e => { exceptionsQueue.Enqueue(e); });

                t1.Wait();
            };

            action.ShouldNotThrow();
            exceptionsQueue.Count.ShouldBe(2);
            exceptionsQueue.ShouldContain(e => e.GetType() == typeof(DivideByZeroException));
            exceptionsQueue.ShouldContain(e => e.GetType() == typeof(InvalidOperationException));
        }

        [Test]
        public void ShouldHandleAllExceptionsWhenMultipleExceptionWithAggregateException()
        {
            var exceptionsQueue = new Queue<Exception>();

            Action action = () =>
            {
                var t1 = Task.Factory.StartNew(() =>
                {
                    Task.Factory.StartNew(() =>
                    {
                        Task.Factory.StartNew(() =>
                        {
                            throw new AggregateException(new FileNotFoundException("File not found"));
                        }, TaskCreationOptions.AttachedToParent);

                        throw new InvalidOperationException(); 
                    }, TaskCreationOptions.AttachedToParent);
                    throw new DivideByZeroException();
                }).HandleExceptions(e => { exceptionsQueue.Enqueue(e); });

                t1.Wait();
            };

            action.ShouldNotThrow();
            exceptionsQueue.Count.ShouldBe(3);
            exceptionsQueue.ShouldContain(e => e.GetType() == typeof(DivideByZeroException));
            exceptionsQueue.ShouldContain(e => e.GetType() == typeof(InvalidOperationException));
            exceptionsQueue.ShouldContain(e => e.GetType() == typeof(FileNotFoundException));
        }

        [Test]
        public void ShouldHandleExpectedExceptionsNonPresentException()
        {
            var exceptionsQueue = new Queue<Exception>();

            Action action = () =>
            {
                var t1 = Task.Factory.StartNew(() => { throw new DivideByZeroException(); })
                    .HandleExceptions(e => e is InvalidOperationException, e => { exceptionsQueue.Enqueue(e); });

                t1.Wait();
            };

            action.ShouldThrow<AggregateException>()
                .Flatten().InnerExceptions
                .Count(e => e.GetType() == typeof(DivideByZeroException) && e.Message == "Attempted to divide by zero.")
                .ShouldBe(1);

            exceptionsQueue.ShouldBeEmpty();
        }

        [Test]
        public void ShouldHandleExpectedExceptionsSingleExceptionSinglePredicate()
        {
            var exceptionsQueue = new Queue<Exception>();

            Action action = () =>
            {
                var t1 = Task.Factory.StartNew(() => { throw new DivideByZeroException(); })
                    .HandleExceptions(e => e is DivideByZeroException, e => { exceptionsQueue.Enqueue(e); });

                t1.Wait();
            };

            action.ShouldNotThrow();

            exceptionsQueue.Count.ShouldBe(1);
            exceptionsQueue.Dequeue().ShouldBeOfType<DivideByZeroException>();
            exceptionsQueue.ShouldBeEmpty();
        }

        [Test]
        public void ShouldHandleExpectedExceptionsSingleExceptionWithMultipleExceptionInPredicate()
        {
            var exceptionsQueue = new Queue<Exception>();

            Action action = () =>
            {
                var t1 = Task.Factory.StartNew(() => { throw new DivideByZeroException(); })
                    .HandleExceptions(e => e is DivideByZeroException || e is FileNotFoundException, e => { exceptionsQueue.Enqueue(e); });

                t1.Wait();
            };

            action.ShouldNotThrow();

            exceptionsQueue.Count.ShouldBe(1);
            exceptionsQueue.Dequeue().ShouldBeOfType<DivideByZeroException>();
            exceptionsQueue.ShouldBeEmpty();
        }

        [Test]
        public void ShouldHandleExpectedExceptionsMultipleExceptions()
        {
            var exceptionsQueue = new Queue<Exception>();

            Action action = () =>
            {
                var t1 = Task.Factory.StartNew(() =>
                {
                    Task.Factory.StartNew(() => { throw new InvalidOperationException(); }, TaskCreationOptions.AttachedToParent);
                    throw new DivideByZeroException();
                }).HandleExceptions(e => e is DivideByZeroException, e => { exceptionsQueue.Enqueue(e); });

                t1.Wait();
            };

            action.ShouldThrow<AggregateException>()
                            .Flatten().InnerExceptions[0]
                                .ShouldBeOfType<InvalidOperationException>();

            exceptionsQueue.Count.ShouldBe(1);
            exceptionsQueue.Dequeue().ShouldBeOfType<DivideByZeroException>();
            exceptionsQueue.ShouldBeEmpty();
        }
        
        [Test]
        public void ShouldHandleExpectedExceptionsMultipleExceptionsWithMultipleExceptionInPredicate()
        {
            var exceptionsQueue = new Queue<Exception>();

            Action action = () =>
            {
                var t1 = Task.Factory.StartNew(() =>
                {
                    Task.Factory.StartNew(() => { throw new InvalidOperationException(); }, TaskCreationOptions.AttachedToParent);
                    throw new DivideByZeroException();
                }).HandleExceptions(e => e is DivideByZeroException || e is InvalidOperationException, e => { exceptionsQueue.Enqueue(e); });

                t1.Wait();
            };

            action.ShouldNotThrow();

            exceptionsQueue.Count.ShouldBe(2);
            exceptionsQueue.ShouldContain(e => e.GetType() == typeof(DivideByZeroException));
            exceptionsQueue.ShouldContain(e => e.GetType() == typeof(InvalidOperationException));
        }
    }
}