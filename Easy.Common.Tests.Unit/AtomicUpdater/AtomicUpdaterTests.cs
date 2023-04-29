namespace Easy.Common.Tests.Unit.AtomicUpdater;

using NUnit.Framework;
using Shouldly;
using System.Threading.Tasks;

[TestFixture]
internal sealed class AtomicUpdaterTests
{
    [Test]
    public async Task When_updating_an_array_of_numbers_by_another_thread()
    {
        int[] primary = new[] { 1, 2, 3 };
        int[] secondary = new[] { 1, 2, 3 };
        AtomicUpdater<int[]> updater = new (primary, secondary);

        updater.Value.ShouldBeSameAs(primary);
        updater.Value.ShouldNotBeSameAs(secondary);

        await Task.Run(() => updater.Update(arr =>
        {
            arr[2] = 6;
            return arr;
        }));

        updater.Value.ShouldBe(new[] { 1, 2, 6 });
    }
}