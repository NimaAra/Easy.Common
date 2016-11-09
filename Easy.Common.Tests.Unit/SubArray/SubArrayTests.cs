namespace Easy.Common.Tests.Unit.SubArray
{
    using System;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public sealed class SubArrayTests
    {
        [Test]
        public void When_creating_a_sub_array()
        {
            var srcArray = new[] { 1, 2, 3, 4, 5 };

            var emptySub = new SubArray<int>(srcArray, 4, 0);
            emptySub.ShouldBeEmpty();
            emptySub.Segment.Array.ShouldBe(srcArray);
            emptySub.Segment.Offset.ShouldBe(4);
            emptySub.Segment.ShouldBeEmpty();

            var subOne = new SubArray<int>(srcArray, 0, 2);
            subOne.Length.ShouldBe(2);
            subOne[0].ShouldBe(1);
            subOne[1].ShouldBe(2);

            subOne.Segment.Array.ShouldBe(srcArray);
            subOne.Segment.Offset.ShouldBe(0);
            subOne.Segment.Count.ShouldBe(subOne.Length);

            var subTwo = new SubArray<int>(srcArray, 1, 3);
            subTwo.Length.ShouldBe(3);
            subTwo[0].ShouldBe(2);
            subTwo[1].ShouldBe(3);
            subTwo[2].ShouldBe(4);

            subTwo.Segment.Array.ShouldBe(srcArray);
            subTwo.Segment.Offset.ShouldBe(1);
            subTwo.Segment.Count.ShouldBe(subTwo.Length);

            // ReSharper disable once ObjectCreationAsStatement
            Should.Throw<ArgumentException>(() => new SubArray<int>(srcArray, 3, 5))
                .Message.ShouldBe("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");

            var subThree = new SubArray<int>(srcArray, 1, 4);
            subThree.Length.ShouldBe(4);
            subThree[0].ShouldBe(2);
            subThree[1].ShouldBe(3);
            subThree[2].ShouldBe(4);
            subThree[3].ShouldBe(5);

            subThree.Segment.Array.ShouldBe(srcArray);
            subThree.Segment.Offset.ShouldBe(1);
            subThree.Segment.Count.ShouldBe(subThree.Length);
        }

        [Test]
        public void When_reversing_a_sub_array()
        {
            var srcArray = new[] { 1, 2, 3, 4, 5 };

            var subArray = new SubArray<int>(srcArray, 0, 5);
            subArray.Length.ShouldBe(5);
            subArray[0].ShouldBe(1);
            subArray[1].ShouldBe(2);
            subArray[2].ShouldBe(3);
            subArray[3].ShouldBe(4);
            subArray[4].ShouldBe(5);

            subArray.Segment.Array.ShouldBe(srcArray);
            subArray.Segment.Offset.ShouldBe(0);
            subArray.Segment.Count.ShouldBe(subArray.Length);

            Array.Reverse(subArray.Segment.Array);

            subArray[4].ShouldBe(1);
            subArray[3].ShouldBe(2);
            subArray[2].ShouldBe(3);
            subArray[1].ShouldBe(4);
            subArray[0].ShouldBe(5);

            srcArray[4].ShouldBe(1);
            srcArray[3].ShouldBe(2);
            srcArray[2].ShouldBe(3);
            srcArray[1].ShouldBe(4);
            srcArray[0].ShouldBe(5);
        }

        [Test]
        public void When_getting_a_sub_array_an_a_new_array()
        {
            var srcArray = new[] { 1, 2, 3, 4, 5 };

            var subArray = new SubArray<int>(srcArray, 0, 5);
            subArray.Length.ShouldBe(5);
            subArray[0].ShouldBe(1);
            subArray[1].ShouldBe(2);
            subArray[2].ShouldBe(3);
            subArray[3].ShouldBe(4);
            subArray[4].ShouldBe(5);

            var copy = subArray.ToArray();
            copy[0].ShouldBe(1);
            copy[1].ShouldBe(2);
            copy[2].ShouldBe(3);
            copy[3].ShouldBe(4);
            copy[4].ShouldBe(5);

            Array.Reverse(subArray.Segment.Array);

            subArray[4].ShouldBe(1);
            subArray[3].ShouldBe(2);
            subArray[2].ShouldBe(3);
            subArray[1].ShouldBe(4);
            subArray[0].ShouldBe(5);

            copy[0].ShouldBe(1);
            copy[1].ShouldBe(2);
            copy[2].ShouldBe(3);
            copy[3].ShouldBe(4);
            copy[4].ShouldBe(5);
        }

        [Test]
        public void When_getting_a_generic_enumerator_from_sub_array()
        {
            var srcArray = new[] { 1, 2, 3, 4, 5 };

            var subArray = new SubArray<int>(srcArray, 0, 5);
            subArray.Length.ShouldBe(5);
            subArray[0].ShouldBe(1);
            subArray[1].ShouldBe(2);
            subArray[2].ShouldBe(3);
            subArray[3].ShouldBe(4);
            subArray[4].ShouldBe(5);

            subArray.ShouldBe(new[] { 1, 2, 3, 4, 5 });
        }

        [Test]
        public void When_getting_a_non_generic_enumerator_from_sub_array()
        {
            var srcArray = new[] { 1, 2, 3, 4, 5 };

            var subArray = new SubArray<int>(srcArray, 0, 5);
            subArray.Length.ShouldBe(5);
            subArray[0].ShouldBe(1);
            subArray[1].ShouldBe(2);
            subArray[2].ShouldBe(3);
            subArray[3].ShouldBe(4);
            subArray[4].ShouldBe(5);

            var enumerator = ((System.Collections.IEnumerable)subArray).GetEnumerator();
            enumerator.Current.ShouldBe(0);
            enumerator.MoveNext().ShouldBeTrue();
            enumerator.Current.ShouldBe(1);
            enumerator.MoveNext().ShouldBeTrue();
            enumerator.Current.ShouldBe(2);
            enumerator.MoveNext().ShouldBeTrue();
            enumerator.Current.ShouldBe(3);
            enumerator.MoveNext().ShouldBeTrue();
            enumerator.Current.ShouldBe(4);
            enumerator.MoveNext().ShouldBeTrue();
            enumerator.Current.ShouldBe(5);
            enumerator.MoveNext().ShouldBeFalse();
        }

        [Test]
        public void When_comparing_two_equal_sub_arrays()
        {
            var srcArray = new[] {1, 2, 3, 4, 5};

            var subOne = new SubArray<int>(srcArray, 0, 3);
            var subTwo = new SubArray<int>(srcArray, 0, 3);

            subOne.Equals(subTwo).ShouldBeTrue();
            subOne.Equals((object)subTwo).ShouldBeTrue();

            (subOne == subTwo).ShouldBeTrue();
            (subOne != subTwo).ShouldBeFalse();

            subOne.GetHashCode().ShouldBe(subTwo.GetHashCode());
            subOne.ToString().ShouldBe(subTwo.ToString());
        }

        [Test]
        public void When_comparing_two_different_sub_arrays()
        {
            var srcArray = new[] { 1, 2, 3, 4, 5 };

            var subOne = new SubArray<int>(srcArray, 0, 3);
            var subTwo = new SubArray<int>(srcArray, 1, 3);

            subOne.Equals(subTwo).ShouldBeFalse();
            subOne.Equals((object)subTwo).ShouldBeFalse();

            (subOne == subTwo).ShouldBeFalse();
            (subOne != subTwo).ShouldBeTrue();

            subOne.GetHashCode().ShouldNotBe(subTwo.GetHashCode());
            subOne.ToString().ShouldNotBe(subTwo.ToString());
        }
    }
}