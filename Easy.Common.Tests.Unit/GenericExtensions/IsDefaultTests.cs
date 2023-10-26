namespace Easy.Common.Tests.Unit.GenericExtensions;

using System;
using Easy.Common.Extensions;
using NUnit.Framework;
using Shouldly;

[TestFixture]
public sealed class IsDefaultTests
{
    [Test]
    public void When_checking_default_value_of_different_types()
    {
        int integer = 0;
        integer.IsDefault().ShouldBeTrue();

        integer = -1;
        integer.IsDefault().ShouldBeFalse();

        integer = 1;
        integer.IsDefault().ShouldBeFalse();

        integer = 0;
        integer.IsDefault().ShouldBeTrue();


        double doubleNo = 0;
        doubleNo.IsDefault().ShouldBeTrue();

        doubleNo = -1.1d;
        doubleNo.IsDefault().ShouldBeFalse();

        doubleNo = double.NaN;
        doubleNo.IsDefault().ShouldBeFalse();

        doubleNo = double.NegativeInfinity;
        doubleNo.IsDefault().ShouldBeFalse();

        doubleNo = double.MinValue;
        doubleNo.IsDefault().ShouldBeFalse();

        doubleNo = 0d;
        doubleNo.IsDefault().ShouldBeTrue();

        TimeSpan someTimeSpan = default(TimeSpan);
        someTimeSpan.IsDefault().ShouldBeTrue();

        someTimeSpan = someTimeSpan.Add(1.Seconds());
        someTimeSpan.IsDefault().ShouldBeFalse();

        someTimeSpan = 1.Days();
        someTimeSpan.IsDefault().ShouldBeFalse();

        DateTime someTime = default(DateTime);
        someTime.IsDefault().ShouldBeTrue();

        someTime = someTime.AddTicks(1);
        someTime.IsDefault().ShouldBeFalse();

        false.IsDefault().ShouldBeTrue();

        true.IsDefault().ShouldBeFalse();

        ((string) null).IsDefault().ShouldBeTrue();

        var someString = string.Empty;
        someString.IsDefault().ShouldBeFalse();

        someString = "ABC";
        someString.IsDefault().ShouldBeFalse();

        ((MyClass) null).IsDefault().ShouldBeTrue();

        var someClass = new MyClass(1);
        someClass.IsDefault().ShouldBeFalse();

        var someStruct = new MyStruct();
        someStruct.IsDefault().ShouldBeTrue();

        someStruct = new MyStruct(1);
        someStruct.IsDefault().ShouldBeFalse();

        someStruct = new MyStruct();
        someStruct.IsDefault().ShouldBeTrue();
    }

    private struct MyStruct
    {
        // ReSharper disable once NotAccessedField.Local
        private int _id;

        public MyStruct(int id)
        {
            _id = id;
        }
    }

    private class MyClass
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly int _id;

        public MyClass(int id)
        {
            _id = id;
        }
    }
}