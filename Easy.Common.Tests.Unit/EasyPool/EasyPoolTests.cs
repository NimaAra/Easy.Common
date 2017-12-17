namespace Easy.Common.Tests.Unit.EasyPool
{
    using System;
    using Easy.Common.Interfaces;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    internal sealed class EasyPoolTests
    {
        [Test]
        public void When_creating_a_pool_with_null_factory()
        {
            var ex = Should.Throw<ArgumentNullException>(() => new EasyPool<string>(null, null, 10));
            ex.Message.ShouldBe("Value cannot be null.\r\nParameter name: factory");
            ex.ParamName.ShouldBe("factory");
        }

        [Test]
        public void When_returning_more_than_max_pool_size()
        {
            Func<SomeObject> factory = () => new SomeObject();
            const int MaxCount = 1;
            using (IEasyPool<SomeObject> pool = new EasyPool<SomeObject>(factory, null, MaxCount))
            {
                pool.Count.ShouldBe((uint)0);

                var obj1 = new SomeObject();
                
                pool.Return(obj1).ShouldBeTrue();

                pool.Count.ShouldBe((uint) 1);

                var obj2 = new SomeObject();

                pool.Return(obj2).ShouldBeFalse();

                pool.Count.ShouldBe((uint)1);

                var pooledObj = pool.Rent();
                pooledObj.ShouldBe(obj1);

                pool.Count.ShouldBe((uint)0);
            }
        }

        [Test]
        public void When_getting_and_renting_items_from_the_pool_with_null_reset()
        {
            Func<SomeObject> factory = () => new SomeObject { Name = "Foo", Number = 1 };
            
            using (IEasyPool<SomeObject> pool = new EasyPool<SomeObject>(factory, null, 5))
            {
                pool.Count.ShouldBe((uint)0);

                var obj1 = pool.Rent();

                obj1.ShouldNotBeNull();
                obj1.Name.ShouldBe("Foo");
                obj1.Number.ShouldBe(1);

                pool.Count.ShouldBe((uint)0);

                pool.Return(obj1).ShouldBeTrue();

                pool.Count.ShouldBe((uint)1);

                var obj2 = pool.Rent();

                obj2.ShouldNotBeNull();
                obj2.ShouldBe(obj1);

                obj2.Name.ShouldBe("Foo");
                obj2.Number.ShouldBe(1);
            }
        }

        [Test]
        public void When_getting_and_renting_items_from_the_pool_with_reset()
        {
            Func<SomeObject> factory = () => new SomeObject { Name = "Foo", Number = 1};
            Action<SomeObject> reset = o =>
            {
                o.Name = null;
                o.Number = 0;
            };

            using (IEasyPool<SomeObject> pool = new EasyPool<SomeObject>(factory, reset, 5))
            {
                pool.Count.ShouldBe((uint)0);

                var obj1 = pool.Rent();

                obj1.ShouldNotBeNull();
                obj1.Name.ShouldBe("Foo");
                obj1.Number.ShouldBe(1);

                pool.Count.ShouldBe((uint) 0);

                pool.Return(obj1).ShouldBeTrue();

                pool.Count.ShouldBe((uint) 1);

                var obj2 = pool.Rent();

                obj2.ShouldNotBeNull();
                obj2.ShouldBe(obj1);

                obj2.Name.ShouldBeNull();
                obj2.Number.ShouldBe(0);
            }
        }

        [Test]
        public void When_getting_and_renting_items_from_the_pool_with_no_reset_on_return()
        {
            Func<SomeObject> factory = () => new SomeObject { Name = "Foo", Number = 1 };
            Action<SomeObject> reset = o =>
            {
                o.Name = null;
                o.Number = 0;
            };

            using (IEasyPool<SomeObject> pool = new EasyPool<SomeObject>(factory, reset, 5))
            {
                pool.Count.ShouldBe((uint)0);

                var obj1 = pool.Rent();

                obj1.ShouldNotBeNull();
                obj1.Name.ShouldBe("Foo");
                obj1.Number.ShouldBe(1);

                pool.Count.ShouldBe((uint)0);

                pool.Return(obj1, false).ShouldBeTrue();

                pool.Count.ShouldBe((uint)1);

                var obj2 = pool.Rent();

                obj2.ShouldNotBeNull();
                obj2.ShouldBe(obj1);

                obj2.Name.ShouldBe("Foo");
                obj2.Number.ShouldBe(1);
            }
        }

        [Test]
        public void When_disposing_the_pool()
        {
            Func<SomeObject> factory = () => new SomeObject();
            const int MaxCount = 1;
            IEasyPool<SomeObject> pool = new EasyPool<SomeObject>(factory, null, MaxCount);
            var obj1 = new SomeObject();

            pool.Return(obj1).ShouldBeTrue();
            pool.Count.ShouldBe((uint)1);

            pool.Dispose();
            pool.Count.ShouldBe((uint)0);
        }

        private sealed class SomeObject
        {
            public string Name { get; set; }
            public int Number { get; set; }
        }
    }
}