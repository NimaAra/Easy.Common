namespace Easy.Common.Tests.Unit.LazyExtensions
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;
    using Easy.Common.Extensions;

    [TestFixture]
    internal sealed class LazyExtensionsTests
    {
        [Test]
        public async Task When_asynchronously_waiting_for_the_result()
        {
            var lazy = new Lazy<Task<int>>(() => Task.FromResult(42));
            var result = await lazy;
            result.ShouldBe(42);
            lazy.Value.Result.ShouldBe(42);
        }
    }
}