namespace Easy.Common.Tests.Unit.FileAndDirectoryExtensions
{
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public class FileIsHiddenTests : Context
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            Given_a_temp_hidden_file();
            Given_a_temp_non_hidden_file();

            When_checking_if_the_files_are_hidden();
        }

        [Test]
        public void Then_isHidden_for_the_hidden_file_should_be_correct()
        {
            ResultOne.ShouldBeTrue();
        }
        
        [Test]
        public void Then_isHidden_for_the_non_hidden_file_should_be_correct()
        {
            ResultTwo.ShouldBeFalse();
        }
    }
}