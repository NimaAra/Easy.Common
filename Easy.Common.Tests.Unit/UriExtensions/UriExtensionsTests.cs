namespace Easy.Common.Tests.Unit.UriExtensions;

using System;
using System.Collections.Generic;
using System.Linq;
using Easy.Common.Extensions;
using NUnit.Framework;
using Shouldly;

[TestFixture]
internal sealed class UriExtensionsTests
{
    [Test]
    public void When_parsing_query_string()
    {
        var uri1 = new Uri("http://www.baz.com/abc.svc?%23name=bar&age=10");

        var queryStringParams1 = uri1.ParseQueryString().ToArray();

        queryStringParams1.ShouldNotBeNull();
        queryStringParams1.ShouldNotBeEmpty();
        queryStringParams1.Length.ShouldBe(2);
            
        queryStringParams1[0].Key.ShouldBe("#name");
        queryStringParams1[0].Value.ShouldBe("bar");
            
        queryStringParams1[1].Key.ShouldBe("age");
        queryStringParams1[1].Value.ShouldBe("10");

        var uri2 = new Uri("http://www.baz.com/abc.svc?foo=This+is+a+simple+%26+short+test.");

        var queryStringParams2 = uri2.ParseQueryString().ToArray();

        queryStringParams2.ShouldNotBeNull();
        queryStringParams2.ShouldNotBeEmpty();
        queryStringParams2.Length.ShouldBe(1);

        queryStringParams2[0].Key.ShouldBe("foo");
        queryStringParams2[0].Value.ShouldBe("This is a simple & short test.");
    }

    [Test]
    public void When_adding_empty_parameters_to_query_string_with_no_parameters()
    {
        var uri = new Uri("http://www.baz.com/abc.svc");

        var parameters = new Dictionary<string, string>();

        var newUri = uri.AddParametersToQueryString(parameters);
        newUri.Query.ShouldBe(string.Empty);

        var parsed = newUri.ParseQueryString().ToArray();
        parsed.Length.ShouldBe(0);
    }

    [Test]
    public void When_adding_empty_parameters_to_query_string_with_existing_parameters()
    {
        var uri = new Uri("http://www.baz.com/abc.svc?name=abc");

        var parameters = new Dictionary<string, string>();

        var newUri = uri.AddParametersToQueryString(parameters);
        newUri.Query.ShouldBe("?name=abc");

        var parsed = newUri.ParseQueryString().ToArray();
        parsed.Length.ShouldBe(1);

        parsed[0].Key.ShouldBe("name");
        parsed[0].Value.ShouldBe("abc");
    }

    [Test]
    public void When_adding_multiple_parameters_to_query_string_with_no_parameters()
    {
        var uri = new Uri("http://www.baz.com/abc.svc");

        var parameters = new Dictionary<string, string>
        {
            ["#foo"] = "#some value, & stuff",
            ["baz"] = "bar"
        };

        var newUri = uri.AddParametersToQueryString(parameters);
        newUri.Query.ShouldBe("?%23foo=%23some+value%2C+%26+stuff&baz=bar");

        var parsed = newUri.ParseQueryString().ToArray();
        parsed.Length.ShouldBe(2);

        parsed[0].Key.ShouldBe("#foo");
        parsed[0].Value.ShouldBe("#some value, & stuff");

        parsed[1].Key.ShouldBe("baz");
        parsed[1].Value.ShouldBe("bar");
    }

    [Test]
    public void When_adding_multiple_parameters_to_query_string_with_existing_parameters()
    {
        var uri = new Uri("http://www.baz.com/abc.svc?name=abc");

        var parameters = new Dictionary<string, string>
        {
            ["#foo"] = "#some value, & stuff",
            ["baz"] = "bar"
        };

        var newUri = uri.AddParametersToQueryString(parameters);
        newUri.Query.ShouldBe("?name=abc&%23foo=%23some+value%2C+%26+stuff&baz=bar");

        var parsed = newUri.ParseQueryString().ToArray();
        parsed.Length.ShouldBe(3);

        parsed[0].Key.ShouldBe("name");
        parsed[0].Value.ShouldBe("abc");

        parsed[1].Key.ShouldBe("#foo");
        parsed[1].Value.ShouldBe("#some value, & stuff");

        parsed[2].Key.ShouldBe("baz");
        parsed[2].Value.ShouldBe("bar");
    }

    [Test]
    public void When_adding_parameters_to_query_string_with_no_parameters()
    {
        var uri = new Uri("http://www.baz.com/abc.svc");

        const string Key = "#foo";
        const string Value = "#some value, & stuff";

        var newUri = uri.AddParametersToQueryString(Key, Value);
        newUri.Query.ShouldBe("?%23foo=%23some+value%2C+%26+stuff");

        var parsed = newUri.ParseQueryString().ToArray();
        parsed.Length.ShouldBe(1);

        parsed[0].Key.ShouldBe("#foo");
        parsed[0].Value.ShouldBe(Value);
    }

    [Test]
    public void When_adding_parameters_to_query_string_with_existing_parameters()
    {
        var uri = new Uri("http://www.baz.com/abc.svc?name=abc");

        const string Key = "#foo";
        const string Value = "#some value, & stuff";

        var newUri = uri.AddParametersToQueryString(Key, Value);
        newUri.Query.ShouldBe("?name=abc&%23foo=%23some+value%2C+%26+stuff");

        var parsed = newUri.ParseQueryString().ToArray();
        parsed.Length.ShouldBe(2);
            
        parsed[0].Key.ShouldBe("name");
        parsed[0].Value.ShouldBe("abc");

        parsed[1].Key.ShouldBe("#foo");
        parsed[1].Value.ShouldBe(Value);
    }

    [Test]
    public void When_adding_encoded_value_to_query_string()
    {
        var uri = new Uri("http://www.baz.com/abc.svc?");
            
        const string Key = "%23foo";
        const string Value = "%23some+value%2C+%26+stuff";

        var newUri = uri.AddParametersToQueryString(Key, Value);
        newUri.Query.ShouldBe("?%2523foo=%2523some%2Bvalue%252C%2B%2526%2Bstuff");

        var parsed = newUri.ParseQueryString().ToArray();
        parsed.Length.ShouldBe(1);
        parsed[0].Key.ShouldBe(Key);
        parsed[0].Value.ShouldBe(Value);
    }

    [Test]
    public void When_adding_invalid_parameters_to_query_string()
    {
        Uri uri = null;
        Should.Throw<ArgumentException>(() => uri.AddParametersToQueryString("foo", "bar"))
            .Message.ShouldBe("Value cannot be null. (Parameter 'uri')");

        uri = new Uri("http://www.foo.com");

        Should.Throw<ArgumentException>(() => uri.AddParametersToQueryString(null, "bar"))
            .Message.ShouldBe("String must not be null, empty or whitespace.");

        Should.Throw<ArgumentException>(() => uri.AddParametersToQueryString("foo", null))
            .Message.ShouldBe("String must not be null, empty or whitespace.");

        Should.Throw<ArgumentException>(() => uri.AddParametersToQueryString(string.Empty, "bar"))
            .Message.ShouldBe("String must not be null, empty or whitespace.");

        Should.Throw<ArgumentException>(() => uri.AddParametersToQueryString("foo", string.Empty))
            .Message.ShouldBe("String must not be null, empty or whitespace.");

        Should.Throw<ArgumentException>(() => uri.AddParametersToQueryString(" ", "bar"))
            .Message.ShouldBe("String must not be null, empty or whitespace.");

        Should.Throw<ArgumentException>(() => uri.AddParametersToQueryString("foo", " "))
            .Message.ShouldBe("String must not be null, empty or whitespace.");
    }
}