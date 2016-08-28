namespace Easy.Common.Tests.Unit.EnumerablesTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Context
    {
        private IEnumerable<int> _sequence;
        protected List<int> Result;
        protected Exception IgnoredException;

        protected void Given_a_sequence_of_integers_with_exception_handled_and_wrapped()
        {
            _sequence = GetSequenceOfIntegers()
                .HandleExceptionWhenYieldReturning(
                e => e is DivideByZeroException, 
                e => 
                {
                    throw new InvalidOperationException("Custom message", e);
                });
        }

        protected void Given_a_sequence_of_integers_with_exception_handled_and_ignored()
        {
            _sequence = GetSequenceOfIntegers()
                .HandleExceptionWhenYieldReturning(
                    e => e is DivideByZeroException,
                    e => IgnoredException = e);
        }

        protected void When_enumerating_the_sequence()
        {
            Result = _sequence.ToList();
        }

        private static IEnumerable<int> GetSequenceOfIntegers()
        {
            // ReSharper disable once InconsistentNaming
            // ReSharper disable once ConvertToConstant.Local
            var ZERO = 0;
            yield return 1;
            yield return 10 / ZERO;
        }
    }
}