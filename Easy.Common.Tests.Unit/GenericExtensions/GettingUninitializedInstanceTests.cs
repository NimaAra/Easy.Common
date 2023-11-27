namespace Easy.Common.Tests.Unit.GenericExtensions;

using System;
using NUnit.Framework;
using Shouldly;

[TestFixture]
public sealed class GettingUninitializedInstanceTests
{
    [Test]
    [Obsolete("Obsolete")]
    public void Run()
    {
        ClassA classA = Extensions.GenericExtensions.GetUninitializedInstance<ClassA>();
        classA.Age.ShouldBe(0);
        classA.Name.ShouldBeNull();
        classA.Number.ShouldBe(29);
            
        var classB = Extensions.GenericExtensions.GetUninitializedInstance<ClassB>();
        classB.Age.ShouldBe(0);
        classB.Name.ShouldBeNull();

        var classBb = Extensions.GenericExtensions.GetUninitializedInstance<ClassBB>();
        classBb.Age.ShouldBe(0);
        classBb.Name.ShouldBeNull();
        classBb.Method().ShouldBe(0);

        var classC = Extensions.GenericExtensions.GetUninitializedInstance<ClassC>();
        classC.Age.ShouldBe(0);
        classC.Name.ShouldBeNull();

        Should.Throw<MemberAccessException>(Extensions.GenericExtensions.GetUninitializedInstance<ClassD>)
            .Message.ShouldBe("Cannot create an abstract class.");

        var classE = Extensions.GenericExtensions.GetUninitializedInstance<ClassE>();
        classE.GetAge().ShouldBe(666);
        classE.GetName().ShouldBe("Happy");

        var structA = Extensions.GenericExtensions.GetUninitializedInstance<StructA>();
        structA.Name.ShouldBeNull();
        structA.Age.ShouldBe(0);
    }

    private class ClassA
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public int Number => 29;

        private ClassA()
        {
            Name = "Sample";
            Age = 10;
        }

        private void Method() { }
    }

    private sealed class ClassB
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public ClassB()
        {
            Name = "Sample";
            Age = 10;
        }

        private void Method() { }
    }

    private class ClassBB
    {
        public string Name { get; set; }
        public int Age { get; set; }

        private int _age = 666;

        public int Method() { return _age; }
    }
        
    internal class ClassC
    {
        public string Name { get; set; }
        public int Age { get; set; }

        private ClassC()
        {
            Name = "Sample";
            Age = 10;
        }

        private void Method() { }
    }

    private abstract class ClassD
    {
        public string Name { get; set; }
        public int Age { get; set; }

        private ClassD()
        {
            Name = "Sample";
            Age = 10;
        }

        private void Method() { }
    }

    private class ClassE
    {
        public static string Name = "Happy";
        private static int _age = 666;

        public string GetName() { return Name; }
        public int GetAge() { return _age; }
    }

    private struct StructA
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public StructA(int i) : this()
        {
            Name = "Happy";
            Age = 123;
        }

        private void Method() { }
    }
}