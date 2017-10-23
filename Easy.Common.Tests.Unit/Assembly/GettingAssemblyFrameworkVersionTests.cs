namespace Easy.Common.Tests.Unit.Assembly
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public sealed class GettingAssemblyFrameworkVersionTests
    {
        private readonly string _currentDirectory = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

        [Test]
        public void When_getting_framework_version_for_a_dot_net_2_assembly()
        {
            var path = Path.Combine(_currentDirectory, @"Assembly\DotNet2.dll");
            Assembly.LoadFrom(path).GetFrameworkVersion()
                .ShouldBe(".NET 2, 3 or 3.5");
        }

        [Test]
        public void When_getting_framework_version_for_a_dot_net_3_assembly()
        {
            var path = Path.Combine(_currentDirectory, @"Assembly\DotNet3.dll");
            Assembly.LoadFrom(path).GetFrameworkVersion()
                .ShouldBe(".NET 2, 3 or 3.5");
        }

        [Test]
        public void When_getting_framework_version_for_a_dot_net_35_assembly()
        {
            var path = Path.Combine(_currentDirectory, @"Assembly\DotNet35.dll");
            Assembly.LoadFrom(path).GetFrameworkVersion()
                .ShouldBe(".NET 2, 3 or 3.5");
        }

        [Test]
        public void When_getting_framework_version_for_a_dot_net_4_assembly()
        {
            var path = Path.Combine(_currentDirectory, @"Assembly\DotNet4.dll");
            Assembly.LoadFrom(path).GetFrameworkVersion()
                .ShouldBe(".NETFramework,Version=v4.0");
        }

        [Test]
        public void When_getting_framework_version_for_a_dot_net_45_assembly()
        {
            var path = Path.Combine(_currentDirectory, @"Assembly\DotNet45.dll");
            Assembly.LoadFrom(path).GetFrameworkVersion()
                .ShouldBe(".NETFramework,Version=v4.5");
        }

        [Test]
        public void When_getting_framework_version_for_a_dot_net_451_assembly()
        {
            var path = Path.Combine(_currentDirectory, @"Assembly\DotNet451.dll");
            Assembly.LoadFrom(path).GetFrameworkVersion()
                .ShouldBe(".NETFramework,Version=v4.5.1");
        }

        [Test]
        public void When_getting_framework_version_for_a_dot_net_452_assembly()
        {
            var path = Path.Combine(_currentDirectory, @"Assembly\DotNet452.dll");
            Assembly.LoadFrom(path).GetFrameworkVersion()
                .ShouldBe(".NETFramework,Version=v4.5.2");
        }

        [Test]
        public void When_getting_framework_version_for_a_dot_net_46_assembly()
        {
            var path = Path.Combine(_currentDirectory, @"Assembly\DotNet46.dll");
            Assembly.LoadFrom(path).GetFrameworkVersion()
                .ShouldBe(".NETFramework,Version=v4.6");
        }

        [Test]
        public void When_getting_framework_version_for_a_dot_net_461_assembly()
        {
            var path = Path.Combine(_currentDirectory, @"Assembly\DotNet461.dll");
            Assembly.LoadFrom(path).GetFrameworkVersion()
                .ShouldBe(".NETFramework,Version=v4.6.1");
        }
    }
}