namespace Easy.Common.Tests.Unit.FileAndDirectoryExtensions
{
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public class DirectoryInfoTests : Context
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            Given_a_temp_hidden_directory();
            Given_a_temp_non_hidden_directory();
            
            When_checking_if_the_directories_are_hidden();
        }

        [Test]
        public void Then_isHidden_for_the_hidden_directory_should_be_correct()
        {
            ResultOne.ShouldBeTrue();
        }

        [Test]
        public void Then_isHidden_for_the_non_hidden_directory_should_be_correct()
        {
            ResultTwo.ShouldBeFalse();
        }
    }
}