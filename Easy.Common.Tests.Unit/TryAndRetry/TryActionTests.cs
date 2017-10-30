namespace Easy.Common.Tests.Unit.TryAndRetry
{
    using System;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    internal sealed class TryActionTests
    {
        [Test]
        public void When_running_an_action()
        {
            Should.NotThrow(() =>
            {
                Try.Handle<NullReferenceException>(() => throw new NullReferenceException())
                    .ShouldBeFalse();
            });

            Should.NotThrow(() =>
            {
                Try.Handle<NullReferenceException>(() => Console.Write(string.Empty))
                    .ShouldBeTrue();
            });

            Should.Throw<IndexOutOfRangeException>(() =>
            {
                Try.Handle<NullReferenceException>(() => throw new IndexOutOfRangeException());
            });
        }

        [Test]
        public void When_handling_multiple_exceptions_for_action()
        {
            Exception thrownEx = null;
            
            Should.NotThrow(() =>
            {
                Try.HandleAny<ArgumentNullException, NullReferenceException>(
                        () => throw new NullReferenceException(), e => thrownEx = e);
                
                thrownEx.ShouldNotBeNull();
                thrownEx.ShouldBeOfType<NullReferenceException>();
            });

            Should.NotThrow(() =>
            {
                thrownEx = null;
                Try.HandleAny<ArgumentNullException, NullReferenceException>(
                        () => Console.Write(string.Empty), e => thrownEx = e);

                thrownEx.ShouldBeNull();
            });

            Should.Throw<IndexOutOfRangeException>(() =>
            {
                Try.HandleAny<ArgumentNullException, NullReferenceException>(
                    () => throw new IndexOutOfRangeException(), null);
            });
        }
    }
}