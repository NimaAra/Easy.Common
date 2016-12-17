namespace Easy.Common.Tests.Unit.DynamicDictionary
{
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

            dic.Keys.ShouldBe(new [] {"A", "B", "C", "D"});
            dic.Values.ShouldBe(new object [] {"A", "B", "C", 1});

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

            ((int)dic.Count).ShouldBe(4);
            ((string)dic["A"]).ShouldBe("A");
            ((string)dic["a"]).ShouldBe("A");
            ((string)dic["B"]).ShouldBe("B");
            ((string)dic["b"]).ShouldBe("B");
            ((string)dic["C"]).ShouldBe("C");
            ((string)dic["c"]).ShouldBe("C");
            ((int)dic["D"]).ShouldBe(1);
            ((int)dic["d"]).ShouldBe(1);

            ((string)dic.A).ShouldBe("A");
            ((string)dic.a).ShouldBe("A");
            ((int)dic.D).ShouldBe(1);
            ((int)dic.d).ShouldBe(1);

            ((string[])dic.Keys).ShouldBe(new[] { "A", "B", "C", "D" });
            ((object[])dic.Values).ShouldBe(new object[] { "A", "B", "C", 1 });

            ((string)dic["non-existent"]).ShouldBeNull();

            dic.foo = "foo";
            ((string)dic["foo"]).ShouldBe("foo");
            ((string)dic["Foo"]).ShouldBe("foo");
        }

        [Test]
        public void When_testing_case_sensitivity()
        {
            var dic = new DynamicDictionary(false);
            dic["A"] = 1;
            dic["A"].ShouldBe(1);
            dic["a"].ShouldBeNull();

            dynamic dyn = dic;
            dyn.A = "sample";
            ((string)dyn.A).ShouldBe("sample");
            ((string)dyn.a).ShouldBeNull();
        }
    }
}