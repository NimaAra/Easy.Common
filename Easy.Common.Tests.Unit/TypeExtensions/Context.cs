namespace Easy.Common.Tests.Unit.TypeExtensions;

using System;
using System.Collections.Generic;
using System.Reflection;
using Easy.Common;
using Easy.Common.Extensions;

public class Context
{
    protected IEnumerable<PropertyInfo> PropertesWithAttribute;

    protected void When_getting_properties_with_specific_attribute_for_type<T, TA>(bool inherit) where TA : Attribute
    {
        PropertesWithAttribute = typeof(T).GetPropertiesWithAttribute<TA>(inherit);
    }

    protected class SampleParent
    {
        [My("_parentId")]
        public int ParentId { get; set; }

        public string ParentName { get; set; }

        [My("_privateParentName")]
        private string PrivateParentName { get; set; }

        [Obsolete("Just a test")]
        public double ParentAge { get; set; }
    }

    protected class SampleChild : SampleParent
    {
        [My("_childId")]
        public int ChildId { get; set; }

        [My("_childAge")]
        public double ChildAge { get; set; }

        public string ChildName { get; set; }

        [My("_privateChildName")]
        private string PrivateChildName { get; set; }

        [Obsolete("Just another test")]
        public decimal Salary { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    protected sealed class MyAttribute : Attribute
    {
        public string Name { get; private set; }

        public MyAttribute(string name)
        {
            Ensure.NotNull(name, "name");
            Name = name;
        }
    }
}