namespace Easy.Common.Tests.Unit.RestClient
{
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    internal sealed class JSONContentTests
    {
        [Test]
        public async Task When_creating_with_default_constructor()
        {
            var content = new JSONContent("Foo");

            content.Headers.ShouldNotBeNull();
            content.Headers.ShouldHaveSingleItem();

            content.Headers.ContentType
                .ShouldBe(MediaTypeHeaderValue.Parse("application/json; charset=utf-8"));

            var json = await content.ReadAsStringAsync();
            json.ShouldBe("Foo");
        }

        [Test]
        public async Task When_creating_with_given_encoding()
        {
            var content = new JSONContent("Foo", Encoding.UTF32);

            content.Headers.ShouldNotBeNull();
            content.Headers.ShouldHaveSingleItem();

            content.Headers.ContentType
                .ShouldBe(MediaTypeHeaderValue.Parse("application/json; charset=utf-32"));

            var json = await content.ReadAsStringAsync();
            json.ShouldBe("Foo");
        }
    }
}