namespace Easy.Common.Tests.Unit.TypeExtensions
{
    using System.Linq;
    using System.Reflection;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture(true)]
    [TestFixture(false)]
    public sealed class GettingParentPropertiesWithAttributes : Context
    {
        private readonly bool _inherit;

        public GettingParentPropertiesWithAttributes(bool inherit)
        {
            _inherit = inherit;
        }

        [OneTimeSetUp]
        public void SetUp()
        {
            When_getting_properties_with_specific_attribute_for_type<SampleParent, MyAttribute>(_inherit);
        }

        [Test]
        public void Then_property_infos_should_not_be_null_or_empty()
        {
            PropertesWithAttribute.ShouldNotBeNull();
            PropertesWithAttribute.ShouldNotBeEmpty();
        }

        [Test]
        public void Then_property_infos_should_have_correct_count()
        {
            PropertesWithAttribute.Count().ShouldBe(1);
        }

        [Test]
        public void Then_correct_attributes_should_have_been_returned()
        {
            PropertesWithAttribute.Single(p => p.Name.Equals("ParentId"))
                .GetCustomAttribute<MyAttribute>()
                .Name.ShouldBe("_parentId");
        }
    }
}