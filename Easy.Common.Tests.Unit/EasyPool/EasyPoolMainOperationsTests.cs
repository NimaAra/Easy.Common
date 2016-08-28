namespace Easy.Common.Tests.Unit.EasyPool
{
    using System;
    using NUnit.Framework;
    using Shouldly;
    using EasyPool = Easy.Common.EasyPool;

    [TestFixture]
    public sealed class EasyPoolMainOperationsTests
    {
        private EasyPool _pool;

        [SetUp]
        public void TestSetUp()
        {
            _pool = new EasyPool();
        }

        [Test]
        public void When_creating_an_object_pool()
        {
            _pool.ShouldNotBeNull();
            _pool.TotalRegistrations.ShouldBe((uint)0);
            _pool.GetCountOfObjectsInThePool<TestPoolableObject>().ShouldBe((uint)0);

            Action gettingNonRegisteredType = () => _pool.Get<TestPoolableObject>();
            gettingNonRegisteredType.ShouldThrow<InvalidOperationException>("Because there is no registration.");
        }

        [Test]
        public void When_getting_a_poolable_object_from_the_pool()
        {
            _pool.Register(() => new TestPoolableObject(), 2);
            _pool.TotalRegistrations.ShouldBe((uint)1);

            _pool.GetCountOfObjectsInThePool<TestPoolableObject>().ShouldBe((uint)0);

            var obj1 = _pool.Get<TestPoolableObject>();
            obj1.Text.ShouldBe("Default");
            obj1.Number.ShouldBe(666);

            _pool.GetCountOfObjectsInThePool<TestPoolableObject>().ShouldBe((uint)0);

            obj1.Text = "Modified";
            obj1.Number = 12;
            obj1.Dispose();
            _pool.GetCountOfObjectsInThePool<TestPoolableObject>()
                .ShouldBe((uint)1, "Because the object has been returned to the pool.");

            var obj2 = _pool.Get<TestPoolableObject>();
            obj2.Text.ShouldBe("Default");
            obj2.Number.ShouldBe(666);

            _pool.GetCountOfObjectsInThePool<TestPoolableObject>().ShouldBe((uint)0);

            var obj3 = _pool.Get<TestPoolableObject>();
            obj3.Text.ShouldBe("Default");
            obj3.Number.ShouldBe(666);

            _pool.GetCountOfObjectsInThePool<TestPoolableObject>().ShouldBe((uint)0);

            obj2.Dispose();
            _pool.GetCountOfObjectsInThePool<TestPoolableObject>().ShouldBe((uint)1);

            obj3.Dispose();
            _pool.GetCountOfObjectsInThePool<TestPoolableObject>().ShouldBe((uint)2);

            obj1.Dispose();
            _pool.GetCountOfObjectsInThePool<TestPoolableObject>()
                .ShouldBe((uint)2, "Because the maximum number of objects allowed in the pool for that type has been reached.");
        }

        [Test]
        public void When_disposing_the_same_object_multiple_times_the_same_object_should_be_returned_to_the_pool_multiple_times()
        {
            _pool.Register(() => new TestPoolableObject(), 2);

            var obj1 = _pool.Get<TestPoolableObject>();
            obj1.Text.ShouldBe("Default");
            obj1.Number.ShouldBe(666);

            _pool.GetCountOfObjectsInThePool<TestPoolableObject>().ShouldBe((uint)0);

            obj1.Dispose();
            _pool.GetCountOfObjectsInThePool<TestPoolableObject>().ShouldBe((uint)1);

            obj1.Dispose();
            _pool.GetCountOfObjectsInThePool<TestPoolableObject>().ShouldBe((uint)2);

            var tmpObj1 = _pool.Get<TestPoolableObject>();
            var tmpObj2 = _pool.Get<TestPoolableObject>();

            _pool.GetCountOfObjectsInThePool<TestPoolableObject>().ShouldBe((uint)0);

            tmpObj1.Text = "A";
            tmpObj2.Text.ShouldBe("A");
        }

        [Test]
        public void When_disposing_the_object_pool()
        {
            _pool.Register(() => new TestPoolableObject(), 2);
            _pool.TotalRegistrations.ShouldBe((uint)1);
            
            var obj1 = _pool.Get<TestPoolableObject>();
            var obj2 = _pool.Get<TestPoolableObject>();

            _pool.GetCountOfObjectsInThePool<TestPoolableObject>().ShouldBe((uint)0);

            obj1.Dispose();
            _pool.GetCountOfObjectsInThePool<TestPoolableObject>().ShouldBe((uint)1);

            _pool.TotalRegistrations.ShouldBe((uint)1);

            _pool.Dispose();
            Action action = () => _pool.TotalRegistrations.ShouldBe((uint)0);
            action.ShouldThrow<ObjectDisposedException>();

            action = () => obj2.Dispose();
            action.ShouldThrow<ObjectDisposedException>();

            action = () => _pool.GetCountOfObjectsInThePool<TestPoolableObject>();
            action.ShouldThrow<ObjectDisposedException>();
        }

        [Test]
        public void When_getting_a_non_reset_poolable_object_from_the_pool()
        {
            _pool.Register(() => new TestNonResetPoolableObject(), 2);
            var obj1 = _pool.Get<TestNonResetPoolableObject>();

            obj1.Text.ShouldBe("Default");
            obj1.Number.ShouldBe(666);

            _pool.GetCountOfObjectsInThePool<TestNonResetPoolableObject>().ShouldBe((uint)0);

            obj1.Text = "Won't Reset";
            obj1.Number = 123;
            obj1.Dispose();
            _pool.GetCountOfObjectsInThePool<TestNonResetPoolableObject>().ShouldBe((uint)1);

            var obj2 = _pool.Get<TestNonResetPoolableObject>();

            _pool.GetCountOfObjectsInThePool<TestNonResetPoolableObject>().ShouldBe((uint)0);

            obj2.Text.ShouldBe("Won't Reset");
            obj2.Number.ShouldBe(123);

            var obj3 = _pool.Get<TestNonResetPoolableObject>();

            obj3.Text.ShouldBe("Default");
            obj3.Number.ShouldBe(666);
        }       

        [Test]
        public void When_registering_objects_with_the_pool()
        {
            _pool.Register(() => new TestPoolableObject(), 2);
            _pool.TotalRegistrations.ShouldBe((uint)1);

            Action addingDuplicateAction = () => _pool.Register(() => new TestPoolableObject(), 3);
            addingDuplicateAction.ShouldThrow<ArgumentException>("Because another registration already exists.");
        }
    }
}