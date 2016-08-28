namespace Easy.Common.Tests.Unit.TypeExtensions
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public sealed class CheckingIfTypeIsASequenceTests
    {
        [Test]
        public void When_checking_type_of_null()
        {
            SequenceType sequenceType;
            var e = Should.Throw<ArgumentNullException>(() =>
            {
                Type nullType = null;
                // ReSharper disable once ExpressionIsAlwaysNull
                nullType.IsSequence(out sequenceType);
            });

            e.Message.ShouldBe("Value cannot be null.\r\nParameter name: type");
            e.ParamName.ShouldBe("type");
        }

        [Test]
        public void When_checking_type_of_string()
        {
            SequenceType sequenceType;
            typeof(string).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.String);
        }

        [Test]
        public void When_checking_a_non_sequence_type()
        {
            SequenceType sequenceType;
            typeof (NonSequenceTypeOne).IsSequence(out sequenceType)
                .ShouldBeFalse();

            sequenceType.ShouldBe(SequenceType.Invalid);
        }

        [Test]
        public void When_checking_a_non_sequence_type_which_implements_an_interface()
        {
            SequenceType sequenceType;
            typeof(NonSequenceTypeTwo).IsSequence(out sequenceType)
                .ShouldBeFalse();

            sequenceType.ShouldBe(SequenceType.Invalid);
        }

        [Test]
        public void When_checking_a_generic_sequence_type()
        {
            SequenceType sequenceType;
            typeof(GenericSequenceType).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.GenericCustom);
        }

        [Test]
        public void When_checking_a_non_generic_sequence_type()
        {
            SequenceType sequenceType;
            typeof(NonGenericSequenceType).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.Custom);
        }

        [Test]
        public void When_checking_an_array()
        {
            SequenceType sequenceType;
            typeof(int[]).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.Array);
        }

        [Test]
        public void When_checking_an_array_list()
        {
            SequenceType sequenceType;
            typeof(ArrayList).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.ArrayList);
        }

        [Test]
        public void When_checking_a_queue()
        {
            SequenceType sequenceType;
            typeof(Queue).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.Queue);
        }

        [Test]
        public void When_checking_a_stack()
        {
            SequenceType sequenceType;
            typeof(Stack).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.Stack);
        }

        [Test]
        public void When_checking_a_bit_array()
        {
            SequenceType sequenceType;
            typeof(BitArray).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.BitArray);
        }

        [Test]
        public void When_checking_a_list_dictionary()
        {
            SequenceType sequenceType;
            typeof(ListDictionary).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.ListDictionary);
        }

        [Test]
        public void When_checking_a_sorted_list()
        {
            SequenceType sequenceType;
            typeof(SortedList).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.SortedList);
        }

        [Test]
        public void When_checking_a_hash_table()
        {
            SequenceType sequenceType;
            typeof(Hashtable).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.Hashtable);
        }

        [Test]
        public void When_checking_an_interface_of_ilist()
        {
            SequenceType sequenceType;
            typeof(IList).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.IList);
        }

        [Test]
        public void When_checking_an_interface_of_icollection()
        {
            SequenceType sequenceType;
            typeof(ICollection).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.ICollection);
        }

        [Test]
        public void When_checking_an_interface_of_idictionary()
        {
            SequenceType sequenceType;
            typeof(IDictionary).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.IDictionary);
        }

        [Test]
        public void When_checking_an_interface_of_ienumerable()
        {
            SequenceType sequenceType;
            typeof(IEnumerable).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.IEnumerable);
        }

        [Test]
        public void When_checking_an_generic_list()
        {
            SequenceType sequenceType;
            typeof(List<int>).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.GenericList);
        }

        [Test]
        public void When_checking_a_generic_hash_set()
        {
            SequenceType sequenceType;
            typeof(HashSet<int>).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.GenericHashSet);
        }

        [Test]
        public void When_checking_a_generic_collection()
        {
            SequenceType sequenceType;
            typeof(Collection<int>).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.GenericCollection);
        }

        [Test]
        public void When_checking_a_generic_linked_list()
        {
            SequenceType sequenceType;
            typeof(LinkedList<int>).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.GenericLinkedList);
        }

        [Test]
        public void When_checking_a_generic_stack()
        {
            SequenceType sequenceType;
            typeof(Stack<int>).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.GenericStack);
        }

        [Test]
        public void When_checking_a_generic_queue()
        {
            SequenceType sequenceType;
            typeof(Queue<int>).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.GenericQueue);
        }

        [Test]
        public void When_checking_an_interface_of_generic_ilist()
        {
            SequenceType sequenceType;
            typeof(IList<string>).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.GenericIList);
        }

        [Test]
        public void When_checking_an_interface_of_generic_icollection()
        {
            SequenceType sequenceType;
            typeof(ICollection<string>).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.GenericICollection);
        }

        [Test]
        public void When_checking_an_interface_of_generic_ienumerable()
        {
            SequenceType sequenceType;
            typeof(IEnumerable<string>).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.GenericIEnumerable);
        }

        [Test]
        public void When_checking_a_generic_dictionary()
        {
            SequenceType sequenceType;
            typeof(Dictionary<int, string>).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.GenericDictionary);
        }

        [Test]
        public void When_checking_a_generic_sorted_dictionary()
        {
            SequenceType sequenceType;
            typeof(SortedDictionary<int, string>).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.GenericSortedDictionary);
        }

        [Test]
        public void When_checking_a_generic_sorted_list()
        {
            SequenceType sequenceType;
            typeof(SortedList<int, string>).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.GenericSortedList);
        }

        [Test]
        public void When_checking_an_interface_of_generic_idictionary()
        {
            SequenceType sequenceType;
            typeof(IDictionary<int, string>).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.GenericIDictionary);
        }

        [Test]
        public void When_checking_an_interface_of_generic_icollection_of_key_value_pair()
        {
            SequenceType sequenceType;
            typeof(ICollection<KeyValuePair<int, string>>).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.GenericICollectionKeyValue);
        }

        [Test]
        public void When_checking_an_interface_of_generic_ienumerable_of_key_value_pair()
        {
            SequenceType sequenceType;
            typeof(IEnumerable<KeyValuePair<int, string>>).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.GenericIEnumerableKeyValue);
        }

        [Test]
        public void When_checking_a_generic_blocking_collection()
        {
            SequenceType sequenceType;
            typeof(BlockingCollection<string>).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.GenericBlockingCollection);
        }

        [Test]
        public void When_checking_a_generic_concurrent_bag()
        {
            SequenceType sequenceType;
            typeof(ConcurrentBag<string>).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.GenericConcurrentBag);
        }

        [Test]
        public void When_checking_a_generic_concurrent_dictionary()
        {
            SequenceType sequenceType;
            typeof(ConcurrentDictionary<string, int>).IsSequence(out sequenceType)
                .ShouldBeTrue();

            sequenceType.ShouldBe(SequenceType.GenericConcurrentDictionary);
        }

        private class NonSequenceTypeOne {}

        private class NonSequenceTypeTwo : ICloneable {
            public object Clone()
            {
                throw new NotImplementedException();
            }
        }

        private class GenericSequenceType : IEnumerable<string> {
            public IEnumerator<string> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private class NonGenericSequenceType : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }
    }
}