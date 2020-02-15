namespace Easy.Common.Tests.Unit.AsyncLazy
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;
    using static System.Threading.Tasks.Task;

    [TestFixture]
    internal sealed class AsyncLazyTests
    {
        [Test]
        public async Task When_creating_an_instance_with_a_factory()
        {
            int counter = 0;

            int Factory()
            {
                counter++;
                return 42;
            }

            var lazy = new AsyncLazy<int>(Factory);

            var lazyResult = await lazy;

            lazy.IsValueCreated.ShouldBeTrue();

            lazyResult.ShouldBe(42);
            lazy.Value.Result.ShouldBe(42);
            
            counter.ShouldBe(1);
        }

        [Test]
        public async Task When_creating_an_instance_with_a_factory_and_getting_value_multiple_times()
        {
            int counter = 0;

            int Factory()
            {
                counter++;
                return 42;
            }

            var lazy = new AsyncLazy<int>(Factory);

            var lazyResultOne = await lazy;
            var lazyResultTwo = lazy.Value;
            var lazyResultThree = await lazy;

            lazyResultOne.ShouldBe(42);
            lazyResultTwo.Result.ShouldBe(42);
            lazyResultThree.ShouldBe(42);

            lazy.Value.Result.ShouldBe(42);

            counter.ShouldBe(1);
        }

        [Test]
        public async Task When_creating_an_instance_with_a_factory_returning_task()
        {
            int counter = 0;

            async Task<int> Factory()
            {
                await Delay(1);
                
                counter++;
                return 42;
            }

            var lazy = new AsyncLazy<int>(Factory);

            var lazyResult = await lazy;

            lazy.IsValueCreated.ShouldBeTrue();

            lazyResult.ShouldBe(42);
            lazy.Value.Result.ShouldBe(42);

            counter.ShouldBe(1);
        }

        [Test]
        public async Task When_creating_an_instance_with_a_factory_returning_task_and_getting_value_multiple_times()
        {
            int counter = 0;

            async Task<int> Factory()
            {
                await Delay(1);

                counter++;
                return 42;
            }

            var lazy = new AsyncLazy<int>(Factory);

            var lazyResultOne = await lazy;
            var lazyResultTwo = lazy.Value;
            var lazyResultThree = await lazy;

            lazyResultOne.ShouldBe(42);
            lazyResultTwo.Result.ShouldBe(42);
            lazyResultThree.ShouldBe(42);

            lazy.Value.Result.ShouldBe(42);

            counter.ShouldBe(1);
        }
    }
}