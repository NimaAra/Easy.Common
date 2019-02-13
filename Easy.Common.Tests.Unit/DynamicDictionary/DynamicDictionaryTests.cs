namespace Easy.Common.Tests.Unit.DynamicDictionary
{
    using System;
    using System.Collections.Generic;
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;
    using DynamicDictionary = Easy.Common.DynamicDictionary;

    [TestFixture]
    internal sealed class DynamicDictionaryTests
    {
        [Test]
        public void When_testing_as_concrete_type()
        {
            DynamicDictionary dic = new DynamicDictionary();
            dic.Count.ShouldBe(0);

            dic["A"] = "A";
            dic["B"] = "B";
            dic["C"] = "C";
            dic["D"] = 1;

            dic.Count.ShouldBe(4);
            dic["A"].ShouldBe("A");
            dic["a"].ShouldBe("A");
            dic["B"].ShouldBe("B");
            dic["b"].ShouldBe("B");
            dic["C"].ShouldBe("C");
            dic["c"].ShouldBe("C");
            dic["D"].ShouldBe(1);
            dic["d"].ShouldBe(1);

            dic.Keys.ShouldBe(new[] { "A", "B", "C", "D" });
            dic.GetDynamicMemberNames().ShouldBe(new[] { "A", "B", "C", "D" });
            
            dic.Values.ShouldBe(new object[] { "A", "B", "C", 1 });

            dic["non-existent"].ShouldBeNull();
        }

        [Test]
        public void When_testing_as_dynamic_type()
        {
            dynamic dic = new DynamicDictionary();
            ((int)dic.Count).ShouldBe(0);

            dic["A"] = "A";
            dic["B"] = "B";
            dic["C"] = "C";
            dic["D"] = 1;

            Func<int> someFunc = () => 1234;
            dic.action = someFunc;

            ((int)dic.Count).ShouldBe(5);
            ((string)dic["A"]).ShouldBe("A");
            ((string)dic["a"]).ShouldBe("A");
            ((string)dic["B"]).ShouldBe("B");
            ((string)dic["b"]).ShouldBe("B");
            ((string)dic["C"]).ShouldBe("C");
            ((string)dic["c"]).ShouldBe("C");
            ((int)dic["D"]).ShouldBe(1);
            ((int)dic["d"]).ShouldBe(1);

            ((int)dic.action()).ShouldBe(1234);
            ((int)dic.ACTION()).ShouldBe(1234);

            ((string)dic.A).ShouldBe("A");
            ((string)dic.a).ShouldBe("A");
            ((int)dic.D).ShouldBe(1);
            ((int)dic.d).ShouldBe(1);

            ((ICollection<string>)dic.Keys).ShouldBe(new[] { "A", "B", "C", "D", "action" });
            ((DynamicDictionary)dic).GetDynamicMemberNames().ShouldBe(new[] { "A", "B", "C", "D", "action" });

            ((ICollection<object>)dic.Values).ShouldBe(new object[] { "A", "B", "C", 1, someFunc });

            ((string)dic["non-existent"]).ShouldBeNull();

            dic.foo = "foo";
            ((string)dic["foo"]).ShouldBe("foo");
            ((string)dic["Foo"]).ShouldBe("foo");
        }

        [Test]
        public void When_testing_case_sensitivity()
        {
            DynamicDictionary caseSensitiveDic = new DynamicDictionary(false)
            {
                ["A"] = 1,
                ["Id"] = 66
            };

            caseSensitiveDic["A"].ShouldBe(1);
            caseSensitiveDic["Id"].ShouldBe(66);

            caseSensitiveDic["a"].ShouldBeNull();
            caseSensitiveDic["ID"].ShouldBeNull();
            caseSensitiveDic["iD"].ShouldBeNull();

            dynamic dynCaseSensitiveDic = caseSensitiveDic;
            dynCaseSensitiveDic.A = "sample";
            ((string)dynCaseSensitiveDic.A).ShouldBe("sample");
            ((int)dynCaseSensitiveDic.Id).ShouldBe(66);

            ((string)dynCaseSensitiveDic.a).ShouldBeNull();
            ((string)dynCaseSensitiveDic.ID).ShouldBeNull();

            ((DynamicDictionary)dynCaseSensitiveDic).GetDynamicMemberNames()
                .ShouldBe(new[] { "A", "Id" });

            DynamicDictionary caseInSensitiveDic = new DynamicDictionary()
            {
                ["A"] = 1,
                ["Id"] = 66
            };

            caseInSensitiveDic["A"].ShouldBe(1);
            caseInSensitiveDic["Id"].ShouldBe(66);

            caseInSensitiveDic["a"].ShouldBe(1);
            caseInSensitiveDic["ID"].ShouldBe(66);
            caseInSensitiveDic["iD"].ShouldBe(66);

            dynamic dynCaseInSensitiveDic = caseInSensitiveDic;
            dynCaseInSensitiveDic.A = "sample";
            ((string)dynCaseInSensitiveDic.A).ShouldBe("sample");
            ((string)dynCaseInSensitiveDic.a).ShouldBe("sample");

            ((int)dynCaseInSensitiveDic.Id).ShouldBe(66);
            ((int)dynCaseInSensitiveDic.ID).ShouldBe(66);
            ((int)dynCaseInSensitiveDic.id).ShouldBe(66);

            ((DynamicDictionary)dynCaseInSensitiveDic).GetDynamicMemberNames()
                .ShouldBe(new[] { "A", "Id" });
        }

        [Test]
        public void When_enumerating_as_dynamic()
        {
            dynamic dic = new DynamicDictionary();
            ((int)dic.Count).ShouldBe(0);

            dic["A"] = "1";
            dic["B"] = "2";
            dic["C"] = "3";
            dic["D"] = 66;

            ((DynamicDictionary)dic).GetDynamicMemberNames().ShouldBe(new[] { "A", "B", "C", "D" });

            foreach (KeyValuePair<string, object> pair in dic)
            {
                pair.Key.ShouldBeOfType<string>();

                if (pair.Key == "A")
                {
                    pair.Value.ShouldBeOfType<string>();
                    pair.Value.ShouldBe("1");
                }

                if (pair.Key == "B")
                {
                    pair.Value.ShouldBeOfType<string>();
                    pair.Value.ShouldBe("2");
                }

                if (pair.Key == "C")
                {
                    pair.Value.ShouldBeOfType<string>();
                    pair.Value.ShouldBe("3");
                }

                if (pair.Key == "D")
                {
                    pair.Value.ShouldBeOfType<int>();
                    pair.Value.ShouldBe(66);
                }
            }
        }

        [Test]
        public void When_enumerating_as_original_type()
        {
            DynamicDictionary dic = new DynamicDictionary();
            dic.Count.ShouldBe(0);

            dic["A"] = "1";
            dic["B"] = "2";
            dic["C"] = "3";
            dic["D"] = 66;

            foreach (var pair in dic)
            {
                pair.Key.ShouldBeOfType<string>();

                if (pair.Key == "A")
                {
                    pair.Value.ShouldBeOfType<string>();
                    pair.Value.ShouldBe("1");
                }

                if (pair.Key == "B")
                {
                    pair.Value.ShouldBeOfType<string>();
                    pair.Value.ShouldBe("2");
                }

                if (pair.Key == "C")
                {
                    pair.Value.ShouldBeOfType<string>();
                    pair.Value.ShouldBe("3");
                }

                if (pair.Key == "D")
                {
                    pair.Value.ShouldBeOfType<int>();
                    pair.Value.ShouldBe(66);
                }
            }
        }

        [Test]
        public void When_getting_a_model_as_dynamic_dictionary()
        {
            var model = new Child { Name = "Foo", Age = 10 };
            DynamicDictionary dicWithInheritedProp = model.ToDynamic();
            
            dicWithInheritedProp.ShouldNotBeNull();
            dicWithInheritedProp.Count.ShouldBe(3);
            dicWithInheritedProp["OriginalName"].ShouldBe("PaPa");
            dicWithInheritedProp["Name"].ShouldBe("Foo");
            dicWithInheritedProp["Age"].ShouldBe(10);

            dicWithInheritedProp.GetDynamicMemberNames().ShouldBe(new[] { "Name", "Age", "OriginalName" });

            var dicWithDeclaredProp = model.ToDynamic(false);

            dicWithDeclaredProp.ShouldNotBeNull();
            dicWithDeclaredProp.Count.ShouldBe(2);
            dicWithDeclaredProp["OriginalName"].ShouldBeNull();
            dicWithDeclaredProp["Name"].ShouldBe("Foo");
            dicWithDeclaredProp["Age"].ShouldBe(10);

            dicWithDeclaredProp.GetDynamicMemberNames().ShouldBe(new[] { "Name", "Age" });
        }

        [Test]
        public void When_getting_a_model_as_dynamic()
        {
            var model = new Child { Name = "Foo", Age = 10 };
            dynamic dicWithInheritedProp = model.ToDynamic();

            ((DynamicDictionary)dicWithInheritedProp).ShouldNotBeNull();
            ((DynamicDictionary)dicWithInheritedProp).Count.ShouldBe(3);

            ((string)dicWithInheritedProp["OriginalName"]).ShouldBe("PaPa");
            ((string)dicWithInheritedProp["Name"]).ShouldBe("Foo");
            ((int)dicWithInheritedProp["Age"]).ShouldBe(10);

            ((DynamicDictionary)dicWithInheritedProp).GetDynamicMemberNames()
                .ShouldBe(new[] { "Name", "Age", "OriginalName" });

            dynamic dicWithDeclaredProp = model.ToDynamic(false);

            ((DynamicDictionary)dicWithDeclaredProp).ShouldNotBeNull();
            ((DynamicDictionary)dicWithDeclaredProp).Count.ShouldBe(2);

            ((string)dicWithDeclaredProp["OriginalName"]).ShouldBeNull();
            ((string)dicWithDeclaredProp["Name"]).ShouldBe("Foo");
            ((int)dicWithDeclaredProp["Age"]).ShouldBe(10);

            ((DynamicDictionary)dicWithDeclaredProp).GetDynamicMemberNames()
                .ShouldBe(new[] { "Name", "Age" });
        }

        private class Base
        {
            public string OriginalName => "PaPa";
        }

        private sealed class Child : Base
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
    }
}