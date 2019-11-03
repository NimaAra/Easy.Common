namespace Easy.Common.Tests.Unit.ExpressionExtensions
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    internal sealed class ExpressionExtensionsTests
    {
        [Test]
        public void When_getting_property_name_from_expression()
        {
            Expression<Func<Person, int>> ageExp = p => p.Age;
            Expression<Func<Person, string>> nameExp = p => p.Name;

            string agePropName = ageExp.GetPropertyName();
            agePropName.ShouldNotBeNull();
            agePropName.ShouldBe("Age");

            var namePropName = nameExp.GetPropertyName();
            namePropName.ShouldNotBeNull();
            namePropName.ShouldBe("Name");
        }

        [Test]
        public void When_getting_property_info_from_expression()
        {
            Expression<Func<Person, int>> ageExp = p => p.Age;
            Expression<Func<Person, string>> nameExp = p => p.Name;

            var instance = new Person();

            PropertyInfo ageProp = ageExp.GetProperty(instance);
            ageProp.ShouldNotBeNull();
            ageProp.Name.ShouldBe("Age");
            ageProp.PropertyType.ShouldBe(typeof(int));

            var namePropName = nameExp.GetProperty(instance);
            namePropName.ShouldNotBeNull();
            namePropName.Name.ShouldBe("Name");
            namePropName.PropertyType.ShouldBe(typeof(string));
        }

        private sealed class Person
        {
            public string Name { get; }
            public int Age { get; }
        }
    }
}