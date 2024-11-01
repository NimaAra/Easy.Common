﻿namespace Easy.Common.Tests.Unit.Enum;

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Easy.Common.Interfaces;
using NUnit.Framework;
using Shouldly;

[TestFixture]
internal sealed class EnumTests
{
    [Test]
    public void When_creating_an_enum()
    {
        MyEnum.OptionA.ShouldBe(MyEnum.OptionA);

        MyEnum.OptionA.ShouldNotBe(MyEnum.OptionB);

        IEnum theEnum = MyEnum.OptionA;

        theEnum.ShouldNotBeNull();

        theEnum.Id.ShouldBe((uint)0);
        theEnum.Name.ShouldBe("OptionA");
    }

    [Test]
    public void When_getting_enum_values()
    {
        IReadOnlyList<MyEnum> values = MyEnum.Values();
        values.ShouldNotBeEmpty();
        values.ShouldBe([MyEnum.OptionA, MyEnum.OptionB], ignoreOrder: true);
    }

    [Test]
    public void When_calling_toString()
    {
        MyEnum.OptionB.ToString().ShouldBe("[1] OptionB");
    }

    sealed record class MyEnum : Enum<MyEnum>
    {
        public static readonly MyEnum OptionA = new();
        public static readonly MyEnum OptionB = new();

        private MyEnum([CallerMemberName] string name = default!) : base(name!) { }
    }
}