namespace Easy.Common.Tests.Unit.FileAndDirectoryExtensions
{
    using System.IO;
    using Easy.Common.Extensions;
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

        [Test]
        public void When_getting_size_of_a_non_existing_directory()
        {
            var someDirectory = new DirectoryInfo(Path.GetRandomFileName());
            Should.Throw<DirectoryNotFoundException>(() =>
            {
                someDirectory.GetSizeInBytes();
            })
            .Message.ShouldBe($"Could not find a part of the path '{someDirectory.FullName}'.");
        }

        [Test]
        public void When_getting_size_of_an_empty_directory()
        {
            DirectoryInfo someDirectory = null;
            try
            {
                someDirectory = new DirectoryInfo(Path.GetRandomFileName());
                someDirectory.Create();

                someDirectory.GetSizeInBytes().ShouldBe(0);
            } finally
            {
                someDirectory?.Delete(true);
            }
        }

        [Test]
        public void When_getting_size_of_a_non_empty_directory()
        {
            DirectoryInfo someDirectory = null;
            try
            {
                someDirectory = new DirectoryInfo(Path.GetRandomFileName());
                someDirectory.Create();

                var file0 = new FileInfo(Path.Combine(someDirectory.FullName, "foo"));
                using (file0.OpenWrite()) { }
                
                someDirectory.GetSizeInBytes().ShouldBe(0);

                using (var file1Sr = new FileInfo(Path.Combine(someDirectory.FullName, "foo")).OpenWrite())
                {
                    file1Sr.WriteByte(1);
                }
                
                someDirectory.GetSizeInBytes().ShouldBe(1);

                using (var file2Sr = new FileInfo(Path.Combine(someDirectory.FullName, "bar")).OpenWrite())
                {
                    for (var i = 0; i < 5; i++)
                    {
                        file2Sr.WriteByte(1);
                    }
                }

                someDirectory.GetSizeInBytes().ShouldBe(6);
            } finally
            {
                someDirectory?.Delete(true);
            }
        }

        [Test]
        public void When_getting_size_of_a_non_empty_directories()
        {
            DirectoryInfo someDirectory = null;
            try
            {
                someDirectory = new DirectoryInfo(Path.GetRandomFileName());
                someDirectory.Create();

                var file0 = new FileInfo(Path.Combine(someDirectory.FullName, "foo"));
                using (file0.OpenWrite()) { }
                
                someDirectory.GetSizeInBytes().ShouldBe(0);

                using (var file1Sr = new FileInfo(Path.Combine(someDirectory.FullName, "foo")).OpenWrite())
                {
                    file1Sr.WriteByte(1);
                }
                
                someDirectory.GetSizeInBytes().ShouldBe(1);

                using (var file2Sr = new FileInfo(Path.Combine(someDirectory.FullName, "bar")).OpenWrite())
                {
                    for (var i = 0; i < 5; i++)
                    {
                        file2Sr.WriteByte(1);
                    }
                }

                someDirectory.GetSizeInBytes().ShouldBe(6);

                var innerDirectory = someDirectory.CreateSubdirectory("inner");
                using (var file3Sr = new FileInfo(Path.Combine(innerDirectory.FullName, "foo")).OpenWrite())
                {
                    for (var i = 0; i < 10; i++)
                    {
                        file3Sr.WriteByte(1);
                    }
                }
                
                someDirectory.GetSizeInBytes().ShouldBe(16);
            } finally
            {
                someDirectory?.Delete(true);
            }
        }
    }
}