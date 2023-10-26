namespace Easy.Common.Tests.Unit.TypeExtensions;

using System;
using Easy.Common.Extensions;
using NUnit.Framework;
using Shouldly;

[TestFixture]
public sealed class CheckingATypeIsSimpleTests
{
    [Test]
    public void Run()
    {
        typeof (byte[]).IsSimpleType().ShouldBeTrue();
        typeof (byte).IsSimpleType().ShouldBeTrue();
        typeof (sbyte).IsSimpleType().ShouldBeTrue();
        typeof (short).IsSimpleType().ShouldBeTrue();
        typeof (ushort).IsSimpleType().ShouldBeTrue();
        typeof (int).IsSimpleType().ShouldBeTrue();
        typeof (uint).IsSimpleType().ShouldBeTrue();
        typeof (long).IsSimpleType().ShouldBeTrue();
        typeof (ulong).IsSimpleType().ShouldBeTrue();
        typeof (float).IsSimpleType().ShouldBeTrue();
        typeof (double).IsSimpleType().ShouldBeTrue();
        typeof (decimal).IsSimpleType().ShouldBeTrue();
        typeof (bool).IsSimpleType().ShouldBeTrue();
        typeof (string).IsSimpleType().ShouldBeTrue();
        typeof (char).IsSimpleType().ShouldBeTrue();
        typeof (Guid).IsSimpleType().ShouldBeTrue();
        typeof (DateTime).IsSimpleType().ShouldBeTrue();
        typeof (DateTimeOffset).IsSimpleType().ShouldBeTrue();
        typeof (TimeSpan).IsSimpleType().ShouldBeTrue();

        typeof(MyEnum).IsSimpleType().ShouldBeTrue();

        typeof(MyClass).IsSimpleType().ShouldBeFalse();
        typeof(MyStruct).IsSimpleType().ShouldBeFalse();
        typeof(IMyInterface).IsSimpleType().ShouldBeFalse();
    }

    private enum MyEnum
    {
        None = 0
    }

    private class MyClass
    {}

    private struct MyStruct
    {}

    private interface IMyInterface
    {}
}