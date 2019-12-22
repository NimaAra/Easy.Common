namespace Easy.Common.Tests.Unit.AtomicUpdater
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    internal sealed class AtomicUpdaterTests
    {
        [Test]
        public async Task When_updating_an_array_of_numbers_by_another_thread()
        {
            var primary = new[] { 1, 2, 3 };
            var secondary = new[] { 1, 2, 3 };
            var updater = new AtomicUpdater<int[]>(primary, secondary);

            updater.Value.ShouldBeSameAs(primary);
            updater.Value.ShouldNotBeSameAs(secondary);

            await Task.Run(() =>
            {
                updater.Update(arr =>
                {
                    arr[2] = 6;
                    return arr;
                });
            });

            updater.Value.ShouldBe(new[] { 1, 2, 6 });
        }
    }
}