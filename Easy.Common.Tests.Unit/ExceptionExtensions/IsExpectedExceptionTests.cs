namespace Easy.Common.Tests.Unit.ExceptionExtensions
{
    using System;
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    internal sealed class IsExpectedExceptionTests
    {
        [Test]
        public void When_checking_using_generics()
        {
            new InvalidOperationException()
                .IsExpectedException<InvalidOperationException>()
                .ShouldBeTrue();

            new InvalidOperationException()
                .IsExpectedException<ArgumentNullException>()
                .ShouldBeFalse();

            new InvalidOperationException()
                .IsExpectedException<IndexOutOfRangeException, InvalidOperationException>()
                .ShouldBeTrue();

            new InvalidOperationException()
                .IsExpectedException<IndexOutOfRangeException, ArgumentNullException>()
                .ShouldBeFalse();
        }
        
        [Test]
        public void When_checking_simple_exceptions()
        {
            new InvalidOperationException()
                .IsExpectedException(typeof(InvalidOperationException))
                .ShouldBeTrue();

            new InvalidOperationException()
                .IsExpectedException(typeof(ArgumentNullException))
                .ShouldBeFalse();

            new InvalidOperationException()
                .IsExpectedException(typeof(IndexOutOfRangeException), typeof(InvalidOperationException))
                .ShouldBeTrue();

            new InvalidOperationException()
                .IsExpectedException(typeof(IndexOutOfRangeException), typeof(ArgumentNullException))
                .ShouldBeFalse();
        }

        [Test]
        public void When_checking_nested_exceptions()
        {
            var innerE = new InvalidOperationException();
            var e = new ArgumentNullException("foo", innerE);

            e.IsExpectedException(typeof(InvalidOperationException))
                .ShouldBeFalse();

            e.IsExpectedException(typeof(ArgumentNullException))
                .ShouldBeTrue();
        }

        [Test]
        public void When_checking_aggregate_exception()
        {
            new AggregateException("foo")
                .IsExpectedException(typeof(AggregateException))
                .ShouldBeTrue();

            new AggregateException("foo")
                .IsExpectedException(typeof(InvalidOperationException))
                .ShouldBeFalse();

            var innerE = new InvalidOperationException();
            new AggregateException(innerE).IsExpectedException(typeof(InvalidOperationException))
                .ShouldBeTrue();

            new AggregateException(innerE).IsExpectedException(typeof(IndexOutOfRangeException))
                .ShouldBeFalse();

            var innerAgg = new AggregateException(innerE);
            new AggregateException(innerAgg).IsExpectedException(typeof(InvalidOperationException))
                .ShouldBeTrue();
        }
    }
}