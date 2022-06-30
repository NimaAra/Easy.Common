﻿namespace Easy.Common.Tests.Unit.Enum;

using System.Collections.Generic;
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

        theEnum.Id.ShouldBe(0);
        theEnum.Name.ShouldBe("OptionA");
    }

    [Test]
    public void When_getting_enum_values()
    {
        IReadOnlyList<MyEnum> values = MyEnum.Values();
        values.ShouldNotBeEmpty();
        values.ShouldBe(new[] { MyEnum.OptionA, MyEnum.OptionB }, ignoreOrder: true);
    }

    sealed record class MyEnum : Enum<MyEnum>
    {
        public static readonly MyEnum OptionA = new(0, nameof(OptionA));
        public static readonly MyEnum OptionB = new(1, nameof(OptionB));

        private MyEnum(int id, string name) : base(id, name) { }
    }
}