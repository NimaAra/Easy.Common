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
            var dic = new DynamicDictionary();
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
            ((ICollection<object>)dic.Values).ShouldBe(new object[] { "A", "B", "C", 1, someFunc });

            ((string)dic["non-existent"]).ShouldBeNull();

            dic.foo = "foo";
            ((string)dic["foo"]).ShouldBe("foo");
            ((string)dic["Foo"]).ShouldBe("foo");
        }

        [Test]
        public void When_testing_case_sensitivity()
        {
            var caseSensetiveDic = new DynamicDictionary(false)
            {
                ["A"] = 1,
                ["Id"] = 66
            };

            caseSensetiveDic["A"].ShouldBe(1);
            caseSensetiveDic["Id"].ShouldBe(66);

            caseSensetiveDic["a"].ShouldBeNull();
            caseSensetiveDic["ID"].ShouldBeNull();
            caseSensetiveDic["iD"].ShouldBeNull();

            dynamic dynCaseSensetiveDic = caseSensetiveDic;
            dynCaseSensetiveDic.A = "sample";
            ((string)dynCaseSensetiveDic.A).ShouldBe("sample");
            ((int)dynCaseSensetiveDic.Id).ShouldBe(66);

            ((string)dynCaseSensetiveDic.a).ShouldBeNull();
            ((string)dynCaseSensetiveDic.ID).ShouldBeNull();

            var caseInSensetiveDic = new DynamicDictionary()
            {
                ["A"] = 1,
                ["Id"] = 66
            };

            caseInSensetiveDic["A"].ShouldBe(1);
            caseInSensetiveDic["Id"].ShouldBe(66);

            caseInSensetiveDic["a"].ShouldBe(1);
            caseInSensetiveDic["ID"].ShouldBe(66);
            caseInSensetiveDic["iD"].ShouldBe(66);

            dynamic dynCaseInSensetiveDic = caseInSensetiveDic;
            dynCaseInSensetiveDic.A = "sample";
            ((string)dynCaseInSensetiveDic.A).ShouldBe("sample");
            ((string)dynCaseInSensetiveDic.a).ShouldBe("sample");

            ((int)dynCaseInSensetiveDic.Id).ShouldBe(66);
            ((int)dynCaseInSensetiveDic.ID).ShouldBe(66);
            ((int)dynCaseInSensetiveDic.id).ShouldBe(66);
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
            var dic = new DynamicDictionary();
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
            var dicWithInherittedProp = model.ToDynamic();
            
            dicWithInherittedProp.ShouldNotBeNull();
            dicWithInherittedProp.Count.ShouldBe(3);
            dicWithInherittedProp["OriginalName"].ShouldBe("PaPa");
            dicWithInherittedProp["Name"].ShouldBe("Foo");
            dicWithInherittedProp["Age"].ShouldBe(10);

            var dicWithDeclaredProp = model.ToDynamic(false);

            dicWithDeclaredProp.ShouldNotBeNull();
            dicWithDeclaredProp.Count.ShouldBe(2);
            dicWithDeclaredProp["OriginalName"].ShouldBeNull();
            dicWithDeclaredProp["Name"].ShouldBe("Foo");
            dicWithDeclaredProp["Age"].ShouldBe(10);
        }

        [Test]
        public void When_getting_a_model_as_dynamic()
        {
            var model = new Child { Name = "Foo", Age = 10 };
            dynamic dicWithInherittedProp = model.ToDynamic();

            ((DynamicDictionary)dicWithInherittedProp).ShouldNotBeNull();
            ((DynamicDictionary)dicWithInherittedProp).Count.ShouldBe(3);

            ((string)dicWithInherittedProp["OriginalName"]).ShouldBe("PaPa");
            ((string)dicWithInherittedProp["Name"]).ShouldBe("Foo");
            ((int)dicWithInherittedProp["Age"]).ShouldBe(10);

            dynamic dicWithDeclaredProp = model.ToDynamic(false);

            ((DynamicDictionary)dicWithDeclaredProp).ShouldNotBeNull();
            ((DynamicDictionary)dicWithDeclaredProp).Count.ShouldBe(2);

            ((string)dicWithDeclaredProp["OriginalName"]).ShouldBeNull();
            ((string)dicWithDeclaredProp["Name"]).ShouldBe("Foo");
            ((int)dicWithDeclaredProp["Age"]).ShouldBe(10);
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