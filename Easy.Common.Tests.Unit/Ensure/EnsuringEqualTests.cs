namespace Easy.Common.Tests.Unit.Ensure
{
    using System;
    using NUnit.Framework;
    using Shouldly;
    using Easy.Common;

    [TestFixture]
    public class EnsuringEqualTests
    {
        [Test]
        public void When_comparing_equal_strings()
        {
            const string FirstString = "One";
            const string SecondString = "One";

            Action action = () => Ensure.Equal(FirstString, SecondString);

            action.ShouldNotThrow("Because the two strings are equal");
        }

        [Test]
        public void When_comparing_equal_integers()
        {
            const int FirstInteger = 1;
            const int SecondInteger = 1;

            Action action = () => Ensure.Equal(FirstInteger, SecondInteger);

            action.ShouldNotThrow("Because the two integers are equal");
        }

        [Test]
        public void When_comparing_different_types_with_equal_values()
        {
            const string FirstString = "One";
            object secondString = "One";

            Action action = () => Ensure.Equal(FirstString, secondString);

            action.ShouldNotThrow("Because the two objects are equal");
        }

        [Test]
        public void When_comparing_different_strings()
        {
            const string FirstString = "One";
            const string SecondString = "Two";

            Action action = () => Ensure.Equal(FirstString, SecondString);

            action.ShouldThrow<ArgumentException>("Because the two strings are different")
                    .Message.ShouldBe("Values must be equal.");
        }

        [Test]
        public void When_comparing_different_integers()
        {
            const int FirstInteger = 1;
            const int SecondInteger = 2;

            Action action = () => Ensure.Equal(FirstInteger, SecondInteger);

            action.ShouldThrow<ArgumentException>("Because the two integers are different")
                    .Message.ShouldBe("Values must be equal.");
        }

        [Test]
        public void When_comparing_string_against_null_object()
        {
            const string FirstString = "Sample";
            string secondString = null;

            Action action = () => Ensure.Equal(FirstString, secondString);

            action.ShouldThrow<ArgumentException>("Because the objects are different")
                    .Message.ShouldBe("Values must be equal.");
        }

        [Test]
        public void When_comparing_null_object_against_string()
        {
            string firstString = null;
            const string SecondString = "Sample";

            Action action = () => Ensure.Equal(firstString, SecondString);

            action.ShouldThrow<ArgumentException>().Message.ShouldBe("Values must be equal.");
        }

        [Test]
        public void When_comparing_two_null_objects()
        {
            string firstString = null;
            string secondSTring = null;

            Action action = () => Ensure.Equal(firstString, secondSTring);

            action.ShouldNotThrow();
        }
    }
}