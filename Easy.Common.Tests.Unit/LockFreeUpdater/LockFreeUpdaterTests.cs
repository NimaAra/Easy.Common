namespace Easy.Common.Tests.Unit.LockFreeUpdater
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    internal sealed class LockFreeUpdaterTests
    {
        [Test]
        public async Task When_updating_an_array_numbers_by_another_thread()
        {
            var updater = new LockFreeUpdater<int[]>(() => new [] {1, 2, 3});

            updater.Value.ShouldBe(new [] {1, 2, 3});

            await Task.Run(() =>
            {
                updater.Value.ShouldBe(new [] {1, 2, 3});
                updater.Update(arr => { arr[2] = 6; });
                updater.Value.ShouldBe(new [] {1, 2, 6});
            });

            updater.Value.ShouldBe(new [] {1, 2, 6});
        }
    }
}