namespace Easy.Common.Tests.Unit.TypeExtensions
{
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public sealed class CheckingATypeImplementsTests
    {
        [Test]
        public void Run()
        {
            typeof (MyClassBase).Implements<MyClassBase>().ShouldBeTrue();
            typeof (MyClassBase).Implements<MyClassA>().ShouldBeFalse();
            typeof (MyClassBase).Implements<IMyInterface>().ShouldBeFalse();

            typeof (MyClassA).Implements<MyClassBase>().ShouldBeTrue();
            typeof (MyClassA).Implements<MyClassA>().ShouldBeTrue();
            typeof (MyClassB).Implements<MyClassA>().ShouldBeTrue();
            typeof (MyClassA).Implements<MyClassB>().ShouldBeFalse();

            typeof(IMyInterface).Implements<IMyInterface>().ShouldBeTrue();
            typeof(MyClassC).Implements<IMyInterface>().ShouldBeTrue();
            typeof(IMyInterface).Implements<MyClassC>().ShouldBeFalse();
            typeof(MyClassD).Implements<IMyInterface>().ShouldBeTrue();
            typeof(MyClassA).Implements<IMyInterface>().ShouldBeFalse();
            
            typeof(IMyInterface).Implements<MyClassBase>().ShouldBeFalse();
            typeof(MyClassD).Implements<MyClassBase>().ShouldBeTrue();
            typeof(MyClassD).Implements<MyClassC>().ShouldBeFalse();

            typeof(MyStructA).Implements<MyStructA>().ShouldBeTrue();
            typeof(MyStructB).Implements<IMyInterface>().ShouldBeTrue();
            typeof(MyStructB).Implements<MyStructA>().ShouldBeFalse();
            typeof(MyStructA).Implements<MyStructB>().ShouldBeFalse();
        }

        private class MyClassBase {} 
        private class MyClassA : MyClassBase{}
        private class MyClassB : MyClassA {}
        private interface IMyInterface {}
        private class MyClassC : IMyInterface {}
        private class MyClassD : MyClassA, IMyInterface {}
        private struct MyStructA {}
        private struct MyStructB : IMyInterface {}
    }
}