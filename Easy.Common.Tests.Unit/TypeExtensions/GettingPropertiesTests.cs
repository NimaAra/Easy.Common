namespace Easy.Common.Tests.Unit.TypeExtensions
{
    using System.Linq;
    using System.Reflection;
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public sealed class GettingPropertiesTests : Context
    {
        [Test]
        public void When_getting_valid_property_with_given_name()
        {
            PropertyInfo childIdProp;
            typeof(SampleChild).TryGetInstanceProperty("ChildId", out childIdProp).ShouldBeTrue();
            childIdProp.ShouldNotBeNull();
            childIdProp.PropertyType.ShouldBe(typeof(int));

            PropertyInfo privateChildNameProp;
            typeof(SampleChild).TryGetInstanceProperty("PrivateChildName", out privateChildNameProp).ShouldBeTrue();
            privateChildNameProp.ShouldNotBeNull();
            privateChildNameProp.PropertyType.ShouldBe(typeof(string));
        }

        [Test]
        public void When_getting_invalid_property_with_given_name()
        {
            PropertyInfo someProperty;
            typeof(SampleChild).TryGetInstanceProperty("foo", out someProperty).ShouldBeFalse();
            someProperty.ShouldBeNull();
        }

        [Test]
        public void When_getting_all_public_properties()
        {
            var allProps = typeof(SampleChild).GetInstanceProperties();
            allProps.ShouldNotBeNull();
            allProps.ShouldNotBeEmpty();
            allProps.Length.ShouldBe(8);

            var declaredProps = typeof(SampleChild).GetInstanceProperties(false);
            declaredProps.ShouldNotBeNull();
            declaredProps.ShouldNotBeEmpty();
            declaredProps.Length.ShouldBe(5);
        }
    }
}