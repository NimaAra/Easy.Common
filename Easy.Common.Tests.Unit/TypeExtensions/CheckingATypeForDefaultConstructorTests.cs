namespace Easy.Common.Tests.Unit.TypeExtensions;

using System;
using Easy.Common.Extensions;
using NUnit.Framework;
using Shouldly;

[TestFixture]
public sealed class CheckingATypeForDefaultConstructorTests
{
    [Test]
    public void Run()
    {
        typeof(SampleClassA).HasDefaultConstructor().ShouldBeFalse();
        typeof(SampleClassAA).HasDefaultConstructor().ShouldBeTrue();
        typeof(SampleClassB).HasDefaultConstructor().ShouldBeTrue();
        typeof(SampleClassC).HasDefaultConstructor().ShouldBeTrue();
        typeof(SampleClassD).HasDefaultConstructor().ShouldBeTrue();
        typeof(SampleClassE).HasDefaultConstructor().ShouldBeTrue();

        typeof(SampleClassPA).HasDefaultConstructor().ShouldBeFalse();
        typeof(SampleClassPAA).HasDefaultConstructor().ShouldBeTrue();
        typeof(SampleClassPB).HasDefaultConstructor().ShouldBeTrue();
        typeof(SampleClassPC).HasDefaultConstructor().ShouldBeTrue();
        typeof(SampleClassPD).HasDefaultConstructor().ShouldBeTrue();
        typeof(SampleClassPE).HasDefaultConstructor().ShouldBeTrue();

        typeof(StaticClass).HasDefaultConstructor().ShouldBeFalse();

        typeof(SampleStructA).HasDefaultConstructor().ShouldBeTrue();
        typeof(SampleStructB).HasDefaultConstructor().ShouldBeTrue();
            
        typeof(DateTime).HasDefaultConstructor().ShouldBeTrue();
        typeof(TimeSpan).HasDefaultConstructor().ShouldBeTrue();
            
        typeof(string).HasDefaultConstructor().ShouldBeFalse();
    }

    private class SampleClassA
    {
        private SampleClassA()
        { }
    }

    private class SampleClassAA
    {
        static SampleClassAA()
        { }
    }

    private class SampleClassB
    { }

    private class SampleClassC
    {
        public SampleClassC()
        { }
    }

    private class SampleClassD
    {
        public SampleClassD()
        { }

        public SampleClassD(int id)
        { }
    }

    private class SampleClassE
    {
        static SampleClassE()
        { }

        public SampleClassE()
        { }

        public SampleClassE(int id)
        { }
    }

    internal class SampleClassPA
    {
        private SampleClassPA()
        { }
    }

    internal class SampleClassPAA
    {
        static SampleClassPAA()
        { }
    }

    internal class SampleClassPB
    { }

    internal class SampleClassPC
    {
        public SampleClassPC()
        { }
    }

    internal class SampleClassPD
    {
        public SampleClassPD()
        { }

        public SampleClassPD(int id)
        { }
    }

    internal class SampleClassPE
    {
        static SampleClassPE()
        { }

        public SampleClassPE()
        { }

        public SampleClassPE(int id)
        { }
    }

    private static class StaticClass
    { }

    private struct SampleStructA
    { }

    private struct SampleStructB
    {
        public SampleStructB(int age)
        {

        }
    }
}