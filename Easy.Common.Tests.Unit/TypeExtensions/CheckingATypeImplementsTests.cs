namespace Easy.Common.Tests.Unit.TypeExtensions
{
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;
    using System;

    [TestFixture]
    public sealed class CheckingATypeImplementsTests
    {
        [Test]
        public void Run()
        {
            Assert<MyClassBase, MyClassBase>(true);
            Assert<MyClassA, MyClassA>(true);
            Assert<MyClassB, MyClassA>(true);
            Assert<IMyInterface, IMyInterface>(true);
            Assert<MyClassD, IMyInterface>(true);
            Assert<MyClassD, MyClassBase>(true);
            Assert<MyStructA, MyStructA>(true);
            Assert<MyStructB, IMyInterface>(true);
            Assert<MyGenericId, IMyGenericInterface<int>>(true);
            Assert<MyChildGenericId, IMyGenericInterface<int>>(true);
            Assert<MyChildGenericId, MyGenericId>(true);
            Assert<IMyGenericInterface<int>, IMyGenericInterface<int>>(true);
            Assert<MyGenericId, MyGenericId>(true);
            Assert<MyChildGenericId, MyChildGenericId>(true);
            Assert<MyGenericId, IMyInterface>(true);
            Assert<MyChildGenericId, IMyInterface>(true);
            Assert<MyGenericClass<int>, IMyGenericInterface<int>>(true);
            Assert<MyChildGenericClass, MyGenericClass<int>>(true);

            Assert<MyClassBase, MyClassA>(false);
            Assert<MyClassBase, IMyInterface>(false);
            Assert<IMyInterface, MyClassC>(false);
            Assert<MyClassA, IMyInterface>(false);
            Assert<IMyInterface, MyClassBase>(false);
            Assert<MyClassD, MyClassC>(false);
            Assert<MyStructB, MyStructA>(false);
            Assert<MyStructA, MyStructB>(false);
            Assert<MyGenericId, IMyGenericInterface<string>>(false);
            Assert<MyClassA, MyGenericId>(false);
            Assert<MyClassC, IMyGenericInterface<int>>(false);
            Assert<MyChildGenericClass, MyGenericClass<Guid>>(false);
            Assert<MyGenericClass<int>, IMyGenericInterface<string>>(false);

            static void Assert<TLeft, TRight>(bool expectedValue)
            {
                typeof(TLeft).Implements<TRight>().ShouldBe(expectedValue);
                typeof(TRight).IsBaseTypeOf<TLeft>().ShouldBe(expectedValue);
            
                typeof(TLeft).Implements(typeof(TRight)).ShouldBe(expectedValue);
                typeof(TRight).IsBaseTypeOf(typeof(TLeft)).ShouldBe(expectedValue);
            }
        }

        private class MyClassBase { }
        private class MyClassA : MyClassBase { }
        private class MyClassB : MyClassA { }
        private interface IMyInterface { }
        private class MyClassC : IMyInterface { }
        private class MyClassD : MyClassA, IMyInterface { }
        private struct MyStructA { }
        private struct MyStructB : IMyInterface { }
        private interface IMyGenericInterface<T> { }
        private record class MyGenericId : IMyGenericInterface<int>, IMyInterface;
        private sealed record class MyChildGenericId : MyGenericId;
        private record class MyGenericClass<T> : IMyGenericInterface<T>;
        private record MyChildGenericClass : MyGenericClass<int>;
    }
}