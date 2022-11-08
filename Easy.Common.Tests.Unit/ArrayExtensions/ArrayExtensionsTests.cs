namespace Easy.Common.Tests.Unit.ArrayExtensions;

using Easy.Common.Extensions;
using NUnit.Framework;
using Shouldly;

[TestFixture]
internal sealed class ArrayExtensionsTests
{
    [Test]
    public void When_filling_and_copying_an_array()
    {
        int[] source = { 1, 2, 3, 4 };

        int[] copied = source.FillAndCopy(42);
        copied.ShouldBe(new[] {42, 42, 42, 42});

        source.ShouldBe(new[] {1, 2, 3, 4});
    }

    [Test]
    public void When_filling_and_copying_an_array_from_an_index()
    {
        int[] source = { 1, 2, 3, 4 };

        int[] copied = source.FillAndCopy(24, 3);
        copied.ShouldBe(new[] {1, 2, 3, 24});

        source.ShouldBe(new[] {1, 2, 3, 4});
    }

    [Test]
    public void When_filling_an_array()
    {
        int[] source = {1, 2, 3, 4};

        int[] filled = source.Fill(42);

        filled.ShouldBe(source);
        filled.ShouldBe(new[] {42, 42, 42, 42});
    }

    [Test]
    public void When_filling_an_array_from_an_index()
    {
        int[] source = {1, 2, 3, 4};

        int[] filled = source.Fill(42, 3);

        filled.ShouldBe(source);
        filled.ShouldBe(new[] {1, 2, 3, 42});
    }

    [Test]
    public void When_mapping_an_array()
    {
        int[] source = {1, 2, 3, 4};

        int[] mapped = source.Map(x => x * 2);

        mapped.ShouldBe(source);
        mapped.ShouldBe(new[] {2, 4, 6, 8});
    }

    [Test]
    public void When_mapping_an_array_from_an_index()
    {
        int[] source = {1, 2, 3, 4};

        int[] mapped = source.Map(x => x * 2, 3);

        mapped.ShouldBe(source);
        mapped.ShouldBe(new[] {1, 2, 3, 8});
    }

    [Test]
    public void When_mapping_and_copying_an_array()
    {
        int[] source = {1, 2, 3, 4};

        int[] copied = source.MapAndCopy(x => x * 2);

        copied.ShouldBe(new[] {2, 4, 6, 8});
        source.ShouldBe(new[] {1, 2, 3, 4});
    }

    [Test]
    public void When_mapping_and_copying_an_array_from_an_index()
    {
        int[] source = {1, 2, 3, 4};

        int[] copied = source.MapAndCopy(x => x * 2, 3);

        copied.ShouldBe(new[] {1, 2, 3, 8});

        source.ShouldBe(new[] {1, 2, 3, 4});
    }
}