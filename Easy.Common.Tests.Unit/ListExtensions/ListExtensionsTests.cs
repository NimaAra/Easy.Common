namespace Easy.Common.Tests.Unit.ListExtensions
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Shouldly;
    using Easy.Common.Extensions;

    [TestFixture]
    internal sealed class ListExtensionsTests
    {
        [Test]
        public void When_initializing_a_list_with_multiple_items()
        {
            var list = new List<int>
            {
                1,
                new[] {2, 3, 4, 5},
                6
            };

            list.Count.ShouldBe(6);
            list.ShouldBe(new [] {1, 2, 3, 4, 5, 6});
        }
    }
}