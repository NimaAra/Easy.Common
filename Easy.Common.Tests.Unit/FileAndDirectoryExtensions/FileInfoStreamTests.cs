namespace Easy.Common.Tests.Unit.FileAndDirectoryExtensions;

using System.IO;
using Easy.Common.Extensions;
using NUnit.Framework;
using Shouldly;

[TestFixture]
internal sealed class FileInfoStreamTests
{
    [Test]
    public void When_opening_or_creating_sequential_read_stream_for_a_non_existing_file()
    {
        FileInfo file = null;
        try
        {
            file = new FileInfo(Path.GetRandomFileName());
            file.Exists.ShouldBeFalse();

            using (var stream = file.OpenOrCreateSequentialRead())
            {
                file.Refresh();
                file.Exists.ShouldBeTrue();
                stream.CanRead.ShouldBeTrue();
                stream.CanWrite.ShouldBeFalse();
                stream.Position.ShouldBe(0);
            }
        } finally
        {
            file?.Delete();
        }
    }

    [Test]
    public void When_opening_or_creating_sequential_read_stream_for_an_existing_file()
    {
        FileInfo file = null;
        try
        {
            file = new FileInfo(Path.GetTempFileName());
            file.Exists.ShouldBeTrue();

            using (var stream = file.OpenOrCreateSequentialRead())
            {
                file.Refresh();
                file.Exists.ShouldBeTrue();
                stream.CanRead.ShouldBeTrue();
                stream.CanWrite.ShouldBeFalse();
                stream.Position.ShouldBe(0);
            }
        } finally
        {
            file?.Delete();
        }
    }

    [Test]
    public void When_opening_or_creating_sequential_write_stream_for_a_non_existing_file()
    {
        FileInfo file = null;
        try
        {
            file = new FileInfo(Path.GetRandomFileName());
            file.Exists.ShouldBeFalse();

            using (var stream = file.OpenOrCreateSequentialWrite())
            {
                file.Refresh();
                file.Exists.ShouldBeTrue();
                stream.CanRead.ShouldBeFalse();
                stream.CanWrite.ShouldBeTrue();
                stream.Position.ShouldBe(0);
            }
        } finally
        {
            file?.Delete();
        }
    }

    [Test]
    public void When_opening_or_creating_sequential_write_stream_for_an_existing_file()
    {
        FileInfo file = null;
        try
        {
            file = new FileInfo(Path.GetTempFileName());
            file.Exists.ShouldBeTrue();

            using (var stream = file.OpenOrCreateSequentialWrite())
            {
                file.Refresh();
                file.Exists.ShouldBeTrue();
                stream.CanRead.ShouldBeFalse();
                stream.CanWrite.ShouldBeTrue();
                stream.Position.ShouldBe(0);
            }
        } finally
        {
            file?.Delete();
        }
    }

    [Test]
    public void When_opening_or_creating_sequential_read_and_write_stream_for_a_non_existing_file()
    {
        FileInfo file = null;
        try
        {
            file = new FileInfo(Path.GetRandomFileName());
            file.Exists.ShouldBeFalse();

            using (var stream = file.OpenOrCreateSequentialReadWrite())
            {
                file.Refresh();
                file.Exists.ShouldBeTrue();
                stream.CanRead.ShouldBeTrue();
                stream.CanWrite.ShouldBeTrue();
                stream.Position.ShouldBe(0);
            }
        } finally
        {
            file?.Delete();
        }
    }

    [Test]
    public void When_opening_or_creating_sequential_read_and_write_stream_for_an_existing_file()
    {
        FileInfo file = null;
        try
        {
            file = new FileInfo(Path.GetTempFileName());
            file.Exists.ShouldBeTrue();

            using (var stream = file.OpenOrCreateSequentialReadWrite())
            {
                file.Refresh();
                file.Exists.ShouldBeTrue();
                stream.CanRead.ShouldBeTrue();
                stream.CanWrite.ShouldBeTrue();
                stream.Position.ShouldBe(0);
            }
        } finally
        {
            file?.Delete();
        }
    }

    [Test]
    public void When_opening_sequential_read_stream_for_a_non_existing_file()
    {
        var file = new FileInfo(Path.GetRandomFileName());
        Should.Throw<FileNotFoundException>(() =>
            {
                file.Exists.ShouldBeFalse();
                file.OpenSequentialRead();
            })
            .Message.ShouldBe($"Could not find file '{file.FullName}'.");
    }

    [Test]
    public void When_opening_sequential_read_stream_for_an_existing_file()
    {
        FileInfo file = null;
        try
        {
            file = new FileInfo(Path.GetTempFileName());
            file.Exists.ShouldBeTrue();

            using (var stream = file.OpenSequentialRead())
            {
                file.Refresh();
                file.Exists.ShouldBeTrue();
                stream.CanRead.ShouldBeTrue();
                stream.CanWrite.ShouldBeFalse();
                stream.Position.ShouldBe(0);
            }
        } finally
        {
            file?.Delete();
        }
    }
}