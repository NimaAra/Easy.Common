namespace Easy.Common.Tests.Unit.TypeExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public sealed class GettingArgumentTypeOfGenericTypeTests
    {
        [Test]
        public void When_checking_type_of_null()
        {
            Type[] result;

            var e = Should.Throw<ArgumentNullException>(() =>
            {
                ((Type) null).TryGetGenericArguments(out result);
            });
#if NET471_OR_GREATER
            e.Message.ShouldBe("Value cannot be null.\r\nParameter name: type");
#else
            e.Message.ShouldBe("Value cannot be null. (Parameter 'type')");
#endif

            e.ParamName.ShouldBe("type");
        }

        [Test]
        public void When_checking_type_of_non_generic_class()
        {
            Type[] result;
            typeof(NonGenericType)
                .TryGetGenericArguments(out result)
                .ShouldBeFalse();

            result.ShouldBeNull();
        }

        [Test]
        public void When_checking_type_of_generic_class_with_single_argument()
        {
            Type[] result;
            typeof(SingleGenericType<string>)
                .TryGetGenericArguments(out result)
                .ShouldBeTrue();

            result.ShouldNotBeNull();
            result.Length.ShouldBe(1);
            result[0].ShouldBe(typeof(string));
        }

        [Test]
        public void When_checking_type_of_generic_class_with_double_arguments()
        {
            Type[] result;
            typeof(DoubleGenericType<int, string>)
                .TryGetGenericArguments(out result)
                .ShouldBeTrue();

            result.ShouldNotBeNull();
            result.Length.ShouldBe(2);
            result[0].ShouldBe(typeof(int));
            result[1].ShouldBe(typeof(string));
        }

        [Test]
        public void When_checking_type_of_generic_class_with_multiple_arguments()
        {
            Type[] result;
            typeof(MultipleGenericType<int, string, DateTime, string, short>)
                .TryGetGenericArguments(out result)
                .ShouldBeTrue();

            result.ShouldNotBeNull();
            result.Length.ShouldBe(5);
            result[0].ShouldBe(typeof(int));
            result[1].ShouldBe(typeof(string));
            result[2].ShouldBe(typeof(DateTime));
            result[3].ShouldBe(typeof(string));
            result[4].ShouldBe(typeof(short));
        }

        [Test]
        public void When_checking_type_of_generic_array()
        {
            Type[] result;
            typeof(int[]).TryGetGenericArguments(out result)
                .ShouldBeTrue();

            result.Length.ShouldBe(1);
            result[0].ShouldBe(typeof(int));
        }

        [Test]
        public void When_checking_type_of_generic_list()
        {
            Type[] result;
            typeof(List<byte>).TryGetGenericArguments(out result)
                .ShouldBeTrue();

            result.Length.ShouldBe(1);
            result[0].ShouldBe(typeof(byte));
        }

        [Test]
        public void When_checking_type_of_generic_queue()
        {
            Type[] result;
            typeof(Queue<DateTime>).TryGetGenericArguments(out result)
                .ShouldBeTrue();

            result.Length.ShouldBe(1);
            result[0].ShouldBe(typeof(DateTime));
        }

        [Test]
        public void When_checking_type_of_generic_stack()
        {
            Type[] result;
            typeof(Stack<DateTime>).TryGetGenericArguments(out result)
                .ShouldBeTrue();

            result.Length.ShouldBe(1);
            result[0].ShouldBe(typeof(DateTime));
        }

        [Test]
        public void When_checking_type_of_generic_collection()
        {
            Type[] result;
            typeof(Collection<DateTime>).TryGetGenericArguments(out result)
                .ShouldBeTrue();

            result.Length.ShouldBe(1);
            result[0].ShouldBe(typeof(DateTime));
        }

        [Test]
        public void When_checking_type_of_generic_hash_set()
        {
            Type[] result;
            typeof(HashSet<DateTime>).TryGetGenericArguments(out result)
                .ShouldBeTrue();

            result.Length.ShouldBe(1);
            result[0].ShouldBe(typeof(DateTime));
        }

        [Test]
        public void When_checking_type_of_generic_linked_list()
        {
            Type[] result;
            typeof(LinkedList<DateTime>).TryGetGenericArguments(out result)
                .ShouldBeTrue();

            result.Length.ShouldBe(1);
            result[0].ShouldBe(typeof(DateTime));
        }

        [Test]
        public void When_checking_type_of_generic_dictionary()
        {
            Type[] result;
            typeof(Dictionary<DateTime, TimeSpan>).TryGetGenericArguments(out result)
                .ShouldBeTrue();

            result.Length.ShouldBe(2);
            result[0].ShouldBe(typeof(DateTime));
            result[1].ShouldBe(typeof(TimeSpan));
        }

        [Test]
        public void When_checking_type_of_generic_collection_of_key_value()
        {
            Type[] result;
            typeof(ICollection<KeyValuePair<DateTime, TimeSpan>>).TryGetGenericArguments(out result)
                .ShouldBeTrue();

            result.Length.ShouldBe(1);
            result[0].ShouldBe(typeof(KeyValuePair<DateTime, TimeSpan>));
        }

        private class NonGenericType { }

        private class SingleGenericType<T> { }

        private class DoubleGenericType<T1, T2> { }

        private class MultipleGenericType<T1, T2, T3, T4, T5> { }
    }
}