namespace Easy.Common.Tests.Unit.ActiveDirectoryHelper
{
    using NUnit.Framework;
    using Shouldly;
    using ActiveDirectoryHelper = Easy.Common.ActiveDirectoryHelper;

    [TestFixture]
    internal sealed class ActiveDirectoryHelperTests
    {
        [Test]
        public void When_getting_groups_for_the_current_user()
        {
            var groups = ActiveDirectoryHelper.GetGroupsForCurrentUser();
            groups.ShouldNotBeNull();
            groups.Count.ShouldBeGreaterThan(1);
            groups.ShouldContain("Everyone");
        }
    }
}