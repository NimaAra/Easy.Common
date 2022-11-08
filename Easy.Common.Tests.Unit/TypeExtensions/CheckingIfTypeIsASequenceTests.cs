namespace Easy.Common.Tests.Unit.TypeExtensions;

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
        var e = Should.Throw<ArgumentNullException>(() =>
        {
            Type nullType = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            nullType.IsSequence(out _);
        });
        e.Message.ShouldBe("Value cannot be null. (Parameter 'type')");

        e.ParamName.ShouldBe("type");
    }

    [Test]
    public void When_checking_type_of_string()
    {
        typeof(string).IsSequence(out SequenceType sequenceType)
            .ShouldBeTrue();

        sequenceType.ShouldBe(SequenceType.String);
    }

    [Test]
    public void When_checking_a_non_sequence_type()
    {
        typeof (NonSequenceTypeOne).IsSequence(out SequenceType sequenceType)
            .ShouldBeFalse();

        sequenceType.ShouldBe(SequenceType.Invalid);
    }

    [Test]
    public void When_checking_a_non_sequence_type_which_implements_an_interface()
    {
        typeof(NonSequenceTypeTwo).IsSequence(out SequenceType sequenceType)
            .ShouldBeFalse();

        sequenceType.ShouldBe(SequenceType.Invalid);
    }

    [Test]
    public void When_checking_a_generic_sequence_type()
    {
        typeof(GenericSequenceType).IsSequence(out SequenceType sequenceType)
            .ShouldBeTrue();

        sequenceType.ShouldBe(SequenceType.GenericCustom);
    }

    [Test]
    public void When_checking_a_non_generic_sequence_type()
    {
        typeof(NonGenericSequenceType).IsSequence(out SequenceType sequenceType)
            .ShouldBeTrue();

        sequenceType.ShouldBe(SequenceType.Custom);
    }

    [Test]
    public void When_checking_an_array()
    {
        typeof(int[]).IsSequence(out SequenceType sequenceType)
            .ShouldBeTrue();

        sequenceType.ShouldBe(SequenceType.Array);
    }

    [Test]
    public void When_checking_an_array_list()
    {
        typeof(ArrayList).IsSequence(out SequenceType sequenceType)
            .ShouldBeTrue();

        sequenceType.ShouldBe(SequenceType.ArrayList);
    }

    [Test]
    public void When_checking_a_queue()
    {
        typeof(Queue).IsSequence(out SequenceType sequenceType)
            .ShouldBeTrue();

        sequenceType.ShouldBe(SequenceType.Queue);
    }

    [Test]
    public void When_checking_a_stack()
    {
        typeof(Stack).IsSequence(out SequenceType sequenceType)
            .ShouldBeTrue();

        sequenceType.ShouldBe(SequenceType.Stack);
    }

    [Test]
    public void When_checking_a_bit_array()
    {
        typeof(BitArray).IsSequence(out SequenceType sequenceType)
            .ShouldBeTrue();

        sequenceType.ShouldBe(SequenceType.BitArray);
    }

    [Test]
    public void When_checking_a_list_dictionary()
    {
        typeof(ListDictionary).IsSequence(out SequenceType sequenceType)
            .ShouldBeTrue();

        sequenceType.ShouldBe(SequenceType.ListDictionary);
    }

    [Test]
    public void When_checking_a_sorted_list()
    {
        typeof(SortedList).IsSequence(out SequenceType sequenceType)
            .ShouldBeTrue();

        sequenceType.ShouldBe(SequenceType.SortedList);
    }

    [Test]
    public void When_checking_a_hash_table()
    {
        typeof(Hashtable).IsSequence(out SequenceType sequenceType)
            .ShouldBeTrue();

        sequenceType.ShouldBe(SequenceType.Hashtable);
    }

    [Test]
    public void When_checking_an_interface_of_ilist()
    {
        typeof(IList).IsSequence(out SequenceType sequenceType)
            .ShouldBeTrue();

        sequenceType.ShouldBe(SequenceType.IList);
    }

    [Test]
    public void When_checking_an_interface_of_icollection()
    {
        typeof(ICollection).IsSequence(out SequenceType sequenceType)
            .ShouldBeTrue();

        sequenceType.ShouldBe(SequenceType.ICollection);
    }

    [Test]
    public void When_checking_an_interface_of_idictionary()
    {
        typeof(IDictionary).IsSequence(out SequenceType sequenceType)
            .ShouldBeTrue();

        sequenceType.ShouldBe(SequenceType.IDictionary);
    }

    [Test]
    public void When_checking_an_interface_of_ienumerable()
    {
        typeof(IEnumerable).IsSequence(out SequenceType sequenceType)
            .ShouldBeTrue();

        sequenceType.ShouldBe(SequenceType.IEnumerable);
    }

    [Test]
    public void When_checking_an_generic_list()
    {
        typeof(List<int>).IsSequence(out SequenceType sequenceType)
            .ShouldBeTrue();

        sequenceType.ShouldBe(SequenceType.GenericList);
    }

    [Test]
    public void When_checking_a_generic_hash_set()
    {
        typeof(HashSet<int>).IsSequence(out SequenceType sequenceType)
            .ShouldBeTrue();

        sequenceType.ShouldBe(SequenceType.GenericHashSet);
    }

    [Test]
    public void When_checking_a_generic_collection()
    {
        typeof(Collection<int>).IsSequence(out SequenceType sequenceType)
            .ShouldBeTrue();

        sequenceType.ShouldBe(SequenceType.GenericCollection);
    }

    [Test]
    public void When_checking_a_generic_linked_list()
    {
        typeof(LinkedList<int>).IsSequence(out SequenceType sequenceType)
            .ShouldBeTrue();

        sequenceType.ShouldBe(SequenceType.GenericLinkedList);
    }

    [Test]
    public void When_checking_a_generic_stack()
    {
        typeof(Stack<int>).IsSequence(out SequenceType sequenceType)
            .ShouldBeTrue();

        sequenceType.ShouldBe(SequenceType.GenericStack);
    }

    [Test]
    public void When_checking_a_generic_queue()
    {
        typeof(Queue<int>).IsSequence(out SequenceType sequenceType)
            .ShouldBeTrue();

        sequenceType.ShouldBe(SequenceType.GenericQueue);
    }

    [Test]
    public void When_checking_an_interface_of_generic_ilist()
    {
        typeof(IList<string>).IsSequence(out SequenceType sequenceType)
            .ShouldBeTrue();

        sequenceType.ShouldBe(SequenceType.GenericIList);
    }

    [Test]
    public void When_checking_an_interface_of_generic_icollection()
    {
        typeof(ICollection<string>).IsSequence(out SequenceType sequenceType)
            .ShouldBeTrue();

        sequenceType.ShouldBe(SequenceType.GenericICollection);
    }

    [Test]
    public void When_checking_an_interface_of_generic_ienumerable()
    {
        typeof(IEnumerable<string>).IsSequence(out SequenceType sequenceType)
            .ShouldBeTrue();

        sequenceType.ShouldBe(SequenceType.GenericIEnumerable);
    }

    [Test]
    public void When_checking_a_generic_dictionary()
    {
        typeof(Dictionary<int, string>).IsSequence(out SequenceType sequenceType)
            .ShouldBeTrue();

        sequenceType.ShouldBe(SequenceType.GenericDictionary);
    }

    [Test]
    public void When_checking_a_generic_sorted_dictionary()
    {
        typeof(SortedDictionary<int, string>).IsSequence(out SequenceType sequenceType)
            .ShouldBeTrue();

        sequenceType.ShouldBe(SequenceType.GenericSortedDictionary);
    }

    [Test]
    public void When_checking_a_generic_sorted_list()
    {
        typeof(SortedList<int, string>).IsSequence(out SequenceType sequenceType)
            .ShouldBeTrue();

        sequenceType.ShouldBe(SequenceType.GenericSortedList);
    }

    [Test]
    public void When_checking_an_interface_of_generic_idictionary()
    {
        typeof(IDictionary<int, string>).IsSequence(out SequenceType sequenceType)
            .ShouldBeTrue();

        sequenceType.ShouldBe(SequenceType.GenericIDictionary);
    }

    [Test]
    public void When_checking_an_interface_of_generic_icollection_of_key_value_pair()
    {
        typeof(ICollection<KeyValuePair<int, string>>).IsSequence(out SequenceType sequenceType)
            .ShouldBeTrue();

        sequenceType.ShouldBe(SequenceType.GenericICollectionKeyValue);
    }

    [Test]
    public void When_checking_an_interface_of_generic_ienumerable_of_key_value_pair()
    {
        typeof(IEnumerable<KeyValuePair<int, string>>).IsSequence(out SequenceType sequenceType)
            .ShouldBeTrue();

        sequenceType.ShouldBe(SequenceType.GenericIEnumerableKeyValue);
    }

    [Test]
    public void When_checking_a_generic_blocking_collection()
    {
        typeof(BlockingCollection<string>).IsSequence(out SequenceType sequenceType)
            .ShouldBeTrue();

        sequenceType.ShouldBe(SequenceType.GenericBlockingCollection);
    }

    [Test]
    public void When_checking_a_generic_concurrent_bag()
    {
        typeof(ConcurrentBag<string>).IsSequence(out SequenceType sequenceType)
            .ShouldBeTrue();

        sequenceType.ShouldBe(SequenceType.GenericConcurrentBag);
    }

    [Test]
    public void When_checking_a_generic_concurrent_dictionary()
    {
        typeof(ConcurrentDictionary<string, int>).IsSequence(out SequenceType sequenceType)
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