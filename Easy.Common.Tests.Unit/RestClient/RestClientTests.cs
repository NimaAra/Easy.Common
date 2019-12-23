namespace Easy.Common.Tests.Unit.RestClient
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Easy.Common.Extensions;
    using Easy.Common.Interfaces;
    using NUnit.Framework;
    using Shouldly;
    using RestClient = Easy.Common.RestClient;

    [TestFixture]
    internal sealed class RestClientTests
    {
        const string ExpectedMessage = "Hello There!";
        
        public HttpResponseMessage StringResponseMessage => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(ExpectedMessage)
        };
        
        public HttpResponseMessage BytesResponseMessage => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(Encoding.UTF8.GetBytes(ExpectedMessage))
        };
        
        public HttpResponseMessage StreamResponseMessage => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(ExpectedMessage)))
        };
        
        [Test]
        public void When_creating_a_client_with_default_constructor()
        {
            using var client = new RestClient();
            client.BaseAddress.ShouldBeNull();
            client.DefaultRequestHeaders.ShouldBeEmpty();
            client.MaxResponseContentBufferSize.ShouldBe((uint)int.MaxValue);
            client.Timeout.ShouldBe(Timeout.InfiniteTimeSpan);
        }

        [Test]
        public void When_creating_a_client_with_custom_constructor()
        {
            var defaultHeaders = new Dictionary<string, IEnumerable<string>>
            {
                { HttpRequestHeader.Accept.ToString(), new [] { "application/json" } },
                { HttpRequestHeader.UserAgent.ToString(), new [] { "foo-bar" } }
            };

            var baseAddress = new Uri("https://foo.bar:1234/");
            using IRestClient client = new RestClient(
                defaultHeaders, 
                timeout: 15.Seconds(), 
                maxResponseContentBufferSize: 10, 
                baseAddress: baseAddress);
            var headers = client.DefaultRequestHeaders.ToArray();

            headers.Length.ShouldBe(defaultHeaders.Count);
                
            client.DefaultRequestHeaders.Count.ShouldBe(defaultHeaders.Count);
            client.DefaultRequestHeaders["Accept"].Single().ShouldBe("application/json");
            client.DefaultRequestHeaders["UserAgent"].Single().ShouldBe("foo-bar");

            client.BaseAddress.ShouldBe(baseAddress);
            client.MaxResponseContentBufferSize.ShouldBe((uint)10);
            client.Timeout.ShouldBe(15.Seconds());
        }

        [Test]
        public void When_sending_a_request()
        {
            var endpoint = new Uri("http://foo.org/api");
            ServicePointManager.FindServicePoint(endpoint).ConnectionLeaseTimeout.ShouldBe(-1);

            using IRestClient client = new RestClient();
            client.SendAsync(new HttpRequestMessage(HttpMethod.Get, endpoint));

            ServicePointManager.FindServicePoint(endpoint).ConnectionLeaseTimeout
                .ShouldBe(-1);
        }

        [Test]
        public void When_sending_a_request_with_string_uri()
        {
            const string Endpoint = "http://example.org:33/api";
            var endpointUri = new Uri(Endpoint);
            ServicePointManager.FindServicePoint(endpointUri).ConnectionLeaseTimeout.ShouldBe(-1);

            using IRestClient client = new RestClient();
            client.SendAsync(new HttpRequestMessage(HttpMethod.Get, Endpoint));

            ServicePointManager.FindServicePoint(endpointUri).ConnectionLeaseTimeout
                .ShouldBe(-1);
        }

        [Test]
        public async Task When_sending_a_request_with_base_address_and_uri()
        {
            var endpoint1 = new Uri("http://foo.bar/api/1");
            var resp1 = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("A") };
            
            var endpoint2 = new Uri("http://foo.bar/api/2");
            var resp2 = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("B") };

            var endpoint3 = new Uri("http://foo.bar/api/3");
            var resp3 = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("C") };

            using var handler = new FakeResponseHandler();
            using (resp1)
            using (resp2)
            using (resp3)
            {
                handler.AddFakeResponse(endpoint1, resp1);
                handler.AddFakeResponse(endpoint2, resp2);
                handler.AddFakeResponse(endpoint3, resp3);

                var baseAddress = new Uri("http://foo.bar");
                using IRestClient client = new RestClient(handler: handler, baseAddress: baseAddress);
                client.BaseAddress.ShouldBe(baseAddress);

                (await client.GetStringAsync(endpoint1)).ShouldBe("A");
                (await client.GetStringAsync(endpoint2)).ShouldBe("B");
                (await client.GetStringAsync(endpoint3)).ShouldBe("C");
            }
        }

        [Test]
        public async Task When_sending_a_request_with_base_address_and_full_url()
        {
            var endpoint1 = new Uri("http://foo.bar/api/1");
            var resp1 = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("A") };

            var endpoint2 = new Uri("http://foo.bar/api/2");
            var resp2 = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("B") };

            var endpoint3 = new Uri("http://foo.bar/api/3");
            var resp3 = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("C") };

            using var handler = new FakeResponseHandler();
            using (resp1)
            using (resp2)
            using (resp3)
            {
                handler.AddFakeResponse(endpoint1, resp1);
                handler.AddFakeResponse(endpoint2, resp2);
                handler.AddFakeResponse(endpoint3, resp3);

                var baseAddress = new Uri("http://foo.bar");
                using IRestClient client = new RestClient(handler: handler, baseAddress: baseAddress);
                client.BaseAddress.ShouldBe(baseAddress);

                (await client.GetStringAsync(endpoint1.OriginalString)).ShouldBe("A");
                (await client.GetStringAsync(endpoint2.OriginalString)).ShouldBe("B");
                (await client.GetStringAsync(endpoint3.OriginalString)).ShouldBe("C");
            }
        }

        [Test]
        public async Task When_sending_a_request_with_base_address_and_route()
        {
            var endpoint1 = new Uri("http://foo.bar/api/1");
            var resp1 = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("A") };

            var endpoint2 = new Uri("http://foo.bar/api/2");
            var resp2 = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("B") };

            var endpoint3 = new Uri("http://foo.bar/api/3");
            var resp3 = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("C") };

            using var handler = new FakeResponseHandler();
            using (resp1)
            using (resp2)
            using (resp3)
            {
                handler.AddFakeResponse(endpoint1, resp1);
                handler.AddFakeResponse(endpoint2, resp2);
                handler.AddFakeResponse(endpoint3, resp3);

                var baseAddress = new Uri("http://foo.bar");
                using IRestClient client = new RestClient(handler: handler, baseAddress: baseAddress);
                client.BaseAddress.ShouldBe(baseAddress);

                (await client.GetStringAsync("api/1")).ShouldBe("A");
                (await client.GetStringAsync("api/2")).ShouldBe("B");
                (await client.GetStringAsync("api/3")).ShouldBe("C");
            }
        }

        [Test]
        public async Task When_sending_a_get_request()
        {
            using var handler = new FakeResponseHandler();
            using var expectedResponse = StringResponseMessage;
            var endpoint = new Uri("http://example.org/api/1");
            handler.AddFakeResponse(endpoint, expectedResponse);

            using IRestClient client = new RestClient(handler: handler);
            var request = new HttpRequestMessage { Method = HttpMethod.Get, RequestUri = endpoint };
                    
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
                
            var respStr = await response.Content.ReadAsStringAsync();
            respStr.ShouldBe(ExpectedMessage);
        }

        [Test]
        public async Task When_sending_an_explicit_put_with_uri()
        {
            using var handler = new FakeResponseHandler();
            using var expectedResponse = StreamResponseMessage;
            var endpoint = new Uri("http://example.org/api/2");
            handler.AddFakeResponse(endpoint, expectedResponse);

            using IRestClient client = new RestClient(handler: handler);
            var response = await client.PutAsync(endpoint, new MultipartFormDataContent());
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
                
            var respStr = await response.Content.ReadAsStringAsync();
            respStr.ShouldBe(ExpectedMessage);
        }

        [Test]
        public async Task When_sending_an_explicit_put_with_string()
        {
            using var handler = new FakeResponseHandler();
            using var expectedResponse = StreamResponseMessage;
            var endpoint = new Uri("http://example.org/api/3");
            handler.AddFakeResponse(endpoint, expectedResponse);

            using (IRestClient client = new RestClient(handler: handler))
            {
                var response = await client.PutAsync(endpoint.OriginalString, new MultipartFormDataContent());
                response.EnsureSuccessStatusCode();
                response.StatusCode.ShouldBe(HttpStatusCode.OK);
                
                var respStr = await response.Content.ReadAsStringAsync();
                respStr.ShouldBe(ExpectedMessage);
            }
        }

        [Test]
        public void When_sending_an_explicit_put_with_uri_and_cancellation()
        {
            using var handler = new DelayedResponseHandler(5.Seconds());
            using IRestClient client = new RestClient(handler: handler);
            var cts = new CancellationTokenSource(1.Seconds());
            Should.Throw<TaskCanceledException>(
                async () => await client.PutAsync(
                    new Uri("http://example.org/api/4"), new MultipartFormDataContent(), cts.Token));
        }

        [Test]
        public void When_sending_an_explicit_put_with_string_and_cancellation()
        {
            using var handler = new DelayedResponseHandler(5.Seconds());
            using IRestClient client = new RestClient(handler: handler);
            var cts = new CancellationTokenSource(1.Seconds());
            Should.Throw<TaskCanceledException>(
                async () => await client.PutAsync(
                    "http://example.org/api/5", new MultipartFormDataContent(), cts.Token));
        }

        [Test]
        public async Task When_sending_a_post_request()
        {
            using var handler = new FakeResponseHandler();
            using var expectedResponse = StringResponseMessage;
            var endpoint = new Uri("http://example.org/api/6");
            handler.AddFakeResponse(endpoint, expectedResponse);

            using IRestClient client = new RestClient(handler: handler);
            var request = new HttpRequestMessage { Method = HttpMethod.Post, RequestUri = endpoint };

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
                
            var respStr = await response.Content.ReadAsStringAsync();
            respStr.ShouldBe(ExpectedMessage);
        }

        [Test]
        public async Task When_sending_an_explicit_post_with_uri()
        {
            using var handler = new FakeResponseHandler();
            using var expectedResponse = StringResponseMessage;
            var endpoint = new Uri("http://example.org/api/7");
            handler.AddFakeResponse(endpoint, expectedResponse);

            using IRestClient client = new RestClient(handler: handler);
            var response = await client.PostAsync(endpoint, new MultipartFormDataContent());
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
                
            var respStr = await response.Content.ReadAsStringAsync();
            respStr.ShouldBe(ExpectedMessage);
        }

        [Test]
        public async Task When_sending_an_explicit_post_with_string()
        {
            using var handler = new FakeResponseHandler();
            using var expectedResponse = StringResponseMessage;
            var endpoint = new Uri("http://example.org/api/8");
            handler.AddFakeResponse(endpoint, expectedResponse);

            using IRestClient client = new RestClient(handler: handler);
            var response = await client.PostAsync(endpoint.OriginalString, new MultipartFormDataContent());
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
                
            var respStr = await response.Content.ReadAsStringAsync();
            respStr.ShouldBe(ExpectedMessage);
        }

        [Test]
        public void When_sending_an_explicit_post_with_uri_and_cancellation()
        {
            using var handler = new DelayedResponseHandler(5.Seconds());
            using IRestClient client = new RestClient(handler: handler);
            var cts = new CancellationTokenSource(1.Seconds());
            Should.Throw<TaskCanceledException>(
                async () => await client.PostAsync(new Uri("http://example.org/api/9"), new MultipartFormDataContent(), cts.Token));
        }

        [Test]
        public void When_sending_an_explicit_post_with_string_and_cancellation()
        {
            using var handler = new DelayedResponseHandler(5.Seconds());
            using IRestClient client = new RestClient(handler: handler);
            var cts = new CancellationTokenSource(1.Seconds());
            Should.Throw<TaskCanceledException>(
                async () => await client.PostAsync("http://example.org/api/10", new MultipartFormDataContent(), cts.Token));
        }

        [Test]
        public async Task When_sending_an_explicit_get_with_uri()
        {
            using var handler = new FakeResponseHandler();
            using var expectedResponse = StringResponseMessage;
            var endpoint = new Uri("http://example.org/api/11");
            handler.AddFakeResponse(endpoint, expectedResponse);

            using IRestClient client = new RestClient(handler: handler);
            var response = await client.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        
            var respStr = await response.Content.ReadAsStringAsync();
            respStr.ShouldBe(ExpectedMessage);
        }

        [Test]
        public async Task When_sending_an_explicit_get_with_string()
        {
            using var handler = new FakeResponseHandler();
            using var expectedResponse = StringResponseMessage;
            var endpoint = new Uri("http://example.org/api/12");
            handler.AddFakeResponse(endpoint, expectedResponse);

            using (IRestClient client = new RestClient(handler: handler))
            {
                var response = await client.GetAsync(endpoint.OriginalString);
                response.EnsureSuccessStatusCode();
                response.StatusCode.ShouldBe(HttpStatusCode.OK);
        
                var respStr = await response.Content.ReadAsStringAsync();
                respStr.ShouldBe(ExpectedMessage);
            }
        }

        [Test]
        public void When_sending_an_explicit_get_with_uri_and_cancellation()
        {
            using var handler = new DelayedResponseHandler(5.Seconds());
            using IRestClient client = new RestClient(handler: handler);
            var cts = new CancellationTokenSource(1.Seconds());
            Should.Throw<TaskCanceledException>(
                async () => await client.GetAsync(new Uri("http://example.org/api/13"), cts.Token));
        }

        [Test]
        public void When_sending_an_explicit_get_with_string_and_cancellation()
        {
            using var handler = new DelayedResponseHandler(5.Seconds());
            using IRestClient client = new RestClient(handler: handler);
            var cts = new CancellationTokenSource(1.Seconds());
            Should.Throw<TaskCanceledException>(
                async () => await client.GetAsync("http://example.org/api/14", cts.Token));
        }

        [Test]
        public async Task When_sending_an_explicit_get_with_uri_and_completion_option()
        {
            using var handler = new FakeResponseHandler();
            using var expectedResponse = StringResponseMessage;
            var endpoint = new Uri("http://example.org/api/15");
            handler.AddFakeResponse(endpoint, expectedResponse);

            using IRestClient client = new RestClient(handler: handler);
            var response = await client.GetAsync(endpoint, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        
            var respStr = await response.Content.ReadAsStringAsync();
            respStr.ShouldBe(ExpectedMessage);
        }

        [Test]
        public async Task When_sending_an_explicit_get_with_string_and_completion_option()
        {
            using var handler = new FakeResponseHandler();
            using var expectedResponse = StringResponseMessage;
            var endpoint = new Uri("http://example.org/api/16");
            handler.AddFakeResponse(endpoint, expectedResponse);

            using IRestClient client = new RestClient(handler: handler);
            var response = await client.GetAsync(endpoint.OriginalString, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        
            var respStr = await response.Content.ReadAsStringAsync();
            respStr.ShouldBe(ExpectedMessage);
        }

        [Test]
        public void When_sending_an_explicit_get_with_uri_and_completion_option_and_cancellation()
        {
            using var handler = new DelayedResponseHandler(5.Seconds());
            using IRestClient client = new RestClient(handler: handler);
            var cts = new CancellationTokenSource(1.Seconds());
            Should.Throw<TaskCanceledException>(
                async () => await client.GetAsync(new Uri("http://example.org/api/17"), HttpCompletionOption.ResponseContentRead, cts.Token));
        }

        [Test]
        public void When_sending_an_explicit_get_with_string_and_completion_option_and_cancellation()
        {
            using var handler = new DelayedResponseHandler(5.Seconds());
            using IRestClient client = new RestClient(handler: handler);
            var cts = new CancellationTokenSource(1.Seconds());
            Should.Throw<TaskCanceledException>(
                async () => await client.GetAsync("http://example.org/api/18", HttpCompletionOption.ResponseContentRead, cts.Token));
        }

        [Test]
        public async Task When_sending_a_delete_request()
        {
            using var handler = new FakeResponseHandler();
            using var expectedResponse = StringResponseMessage;
            var endpoint = new Uri("http://example.org/api/19");
            handler.AddFakeResponse(endpoint, expectedResponse);

            using IRestClient client = new RestClient(handler: handler);
            var request = new HttpRequestMessage { Method = HttpMethod.Delete, RequestUri = endpoint };
            
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        
            var respStr = await response.Content.ReadAsStringAsync();
            respStr.ShouldBe(ExpectedMessage);
        }

        [Test]
        public async Task When_sending_an_explicit_delete_with_uri()
        {
            using var handler = new FakeResponseHandler();
            using var expectedResponse = StringResponseMessage;
            var endpoint = new Uri("http://example.org/api/20");
            handler.AddFakeResponse(endpoint, expectedResponse);

            using IRestClient client = new RestClient(handler: handler);
            var response = await client.DeleteAsync(endpoint);
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var respStr = await response.Content.ReadAsStringAsync();
            respStr.ShouldBe(ExpectedMessage);
        }

        [Test]
        public async Task When_sending_an_explicit_delete_with_string()
        {
            using var handler = new FakeResponseHandler();
            using var expectedResponse = StringResponseMessage;
            var endpoint = new Uri("http://example.org/api/21");
            handler.AddFakeResponse(endpoint, expectedResponse);

            using IRestClient client = new RestClient(handler: handler);
            var response = await client.DeleteAsync(endpoint.OriginalString);
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var respStr = await response.Content.ReadAsStringAsync();
            respStr.ShouldBe(ExpectedMessage);
        }

        [Test]
        public void When_sending_an_explicit_delete_with_uri_and_cancellation()
        {
            using var handler = new DelayedResponseHandler(5.Seconds());
            using IRestClient client = new RestClient(handler: handler);
            var cts = new CancellationTokenSource(1.Seconds());
            Should.Throw<TaskCanceledException>(
                async () => await client.DeleteAsync(new Uri("http://example.org/api/22"), cts.Token));
        }

        [Test]
        public void When_sending_an_explicit_delete_with_string_and_cancellation()
        {
            using var handler = new DelayedResponseHandler(5.Seconds());
            using IRestClient client = new RestClient(handler: handler);
            var cts = new CancellationTokenSource(1.Seconds());
            Should.Throw<TaskCanceledException>(
                async () => await client.DeleteAsync("http://example.org/api/23", cts.Token));
        }

        [Test]
        public void When_sending_a_request_with_cancellation_and_completion_option()
        {
            using var handler = new DelayedResponseHandler(5.Seconds());
            using IRestClient client = new RestClient(handler: handler);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get, RequestUri = new Uri("http://example.org/api/24")
            };

            var cts = new CancellationTokenSource(1.Seconds());
            Should.Throw<TaskCanceledException>(
                async () => await client.SendAsync(
                    request, HttpCompletionOption.ResponseContentRead, cts.Token));
        }

        [Test]
        public void When_sending_a_request_with_cancellation()
        {
            using var handler = new DelayedResponseHandler(5.Seconds());
            using IRestClient client = new RestClient(handler: handler);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get, RequestUri = new Uri("http://example.org/api/25")
            };

            var cts = new CancellationTokenSource(1.Seconds());
            Should.Throw<TaskCanceledException>(async () => await client.SendAsync(request, cts.Token));
        }

        [Test]
        public void When_sending_a_request_then_cancelling_all_pending_requests()
        {
            using var handler = new DelayedResponseHandler(5.Seconds());
            using IRestClient client = new RestClient(handler: handler);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get, RequestUri = new Uri("http://example.org/api/26")
            };

            var copy = client;
#pragma warning disable 4014
            Task.Delay(1.Seconds()).ContinueWith(_ => copy.CancelPendingRequests());
#pragma warning restore 4014
            Should.Throw<TaskCanceledException>(async () => await client.SendAsync(request));
        }

        [Test]
        public async Task When_sending_a_get_string_with_uri()
        {
            using var handler = new FakeResponseHandler();
            using var expectedResponse = StringResponseMessage;
            var endpoint = new Uri("http://example.org/api/27");
            handler.AddFakeResponse(endpoint, expectedResponse);

            using IRestClient client = new RestClient(handler: handler);
            var response = await client.GetStringAsync(endpoint);
            response.ShouldBe(ExpectedMessage);
        }

        [Test]
        public async Task When_sending_a_get_string_with_string()
        {
            using var handler = new FakeResponseHandler();
            using var expectedResponse = StringResponseMessage;
            var endpoint = new Uri("http://example.org/api/28");
            handler.AddFakeResponse(endpoint, expectedResponse);

            using (IRestClient client = new RestClient(handler: handler))
            {
                var response = await client.GetStringAsync(endpoint.OriginalString);
                response.ShouldBe(ExpectedMessage);
            }
        }

        [Test]
        public async Task When_sending_a_get_stream_with_uri()
        {
            using var handler = new FakeResponseHandler();
            using var expectedResponse = StreamResponseMessage;
            var endpoint = new Uri("http://example.org/api/29");
            handler.AddFakeResponse(endpoint, expectedResponse);

            using IRestClient client = new RestClient(handler: handler);
            var response = await client.GetStreamAsync(endpoint);
            using var streamReader = new StreamReader(response, Encoding.UTF8);
            var responseString = await streamReader.ReadToEndAsync();
            responseString.ShouldBe(ExpectedMessage);
        }

        [Test]
        public async Task When_sending_a_get_stream_with_string()
        {
            using var handler = new FakeResponseHandler();
            using var expectedResponse = StreamResponseMessage;
            var endpoint = new Uri("http://example.org/api/30");
            handler.AddFakeResponse(endpoint, expectedResponse);

            using IRestClient client = new RestClient(handler: handler);
            var response = await client.GetStreamAsync(endpoint.OriginalString);
            using var streamReader = new StreamReader(response, Encoding.UTF8);
            var responseString = await streamReader.ReadToEndAsync();
            responseString.ShouldBe(ExpectedMessage);
        }

        [Test]
        public async Task When_sending_a_get_byte_array_with_uri()
        {
            using var handler = new FakeResponseHandler();
            using var expectedResponse = BytesResponseMessage;
            var endpoint = new Uri("http://example.org/api/31");
            handler.AddFakeResponse(endpoint, expectedResponse);

            using IRestClient client = new RestClient(handler: handler);
            var response = await client.GetByteArrayAsync(endpoint);
            var responseString = Encoding.UTF8.GetString(response);
            responseString.ShouldBe(ExpectedMessage);
        }

        [Test]
        public async Task When_sending_a_get_byte_array_with_string()
        {
            using var handler = new FakeResponseHandler();
            using var expectedResponse = BytesResponseMessage;
            var endpoint = new Uri("http://example.org/api/32");
            handler.AddFakeResponse(endpoint, expectedResponse);

            using IRestClient client = new RestClient(handler: handler);
            var response = await client.GetByteArrayAsync(endpoint.OriginalString);
            var responseString = Encoding.UTF8.GetString(response);
            responseString.ShouldBe(ExpectedMessage);
        }

        [Test]
        public async Task When_calling_a_fake_handler()
        {
            using var handler = new FakeResponseHandler();
            handler.AddFakeResponse(
                new Uri("http://example.org/test"), new HttpResponseMessage(HttpStatusCode.OK));

            using IRestClient client = new RestClient(handler: handler);
            var response1 = await client.GetAsync("http://example.org/notthere");
            var response2 = await client.GetAsync("http://example.org/test");

            response1.StatusCode.ShouldBe(HttpStatusCode.NotFound);
            response2.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Test]
        public async Task When_sending_an_explicit_get_with_timeout()
        {
            var timeout = 3.Seconds();

            using var handler = new FakeResponseHandler();
            using var expectedResponse = StringResponseMessage;
            var endpoint = new Uri("http://example.org/api/33");
            handler.AddFakeResponse(endpoint, expectedResponse);

            using IRestClient client = new RestClient(handler: handler);
            var resp1 = await client.GetAsync(endpoint, timeout);
            resp1.EnsureSuccessStatusCode();
            resp1.StatusCode.ShouldBe(HttpStatusCode.OK);
            (await resp1.Content.ReadAsStringAsync()).ShouldBe(ExpectedMessage);

            var resp2 = await client.GetAsync(endpoint.OriginalString, timeout);
            resp2.EnsureSuccessStatusCode();
            resp2.StatusCode.ShouldBe(HttpStatusCode.OK);
            (await resp2.Content.ReadAsStringAsync()).ShouldBe(ExpectedMessage);
        }

        [Test]
        public void When_sending_an_explicit_get_which_times_out()
        {
            var timeout = 3.Seconds();
            var endpoint = new Uri("http://example.org/api/34");

            using var handler = new DelayedResponseHandler(10.Seconds());
            using IRestClient client = new RestClient(handler: handler);
            Should.Throw<TaskCanceledException>(
                async () => await client.GetAsync(endpoint, timeout));
                
            Should.Throw<TaskCanceledException>(
                async () => await client.GetAsync(endpoint.OriginalString, timeout));
        }

        [Test]
        public async Task When_sending_a_get_string_with_timeout()
        {
            var timeout = 3.Seconds();

            using var handler = new FakeResponseHandler();
            using var expectedResponse = StringResponseMessage;
            var endpoint = new Uri("http://example.org/api/34");
            handler.AddFakeResponse(endpoint, expectedResponse);

            using IRestClient client = new RestClient(handler: handler);
            (await client.GetStringAsync(endpoint, timeout)).ShouldBe(ExpectedMessage);
            (await client.GetStringAsync(endpoint.OriginalString, timeout)).ShouldBe(ExpectedMessage);
        }

        [Test]
        public void When_sending_a_get_string_which_times_out()
        {
            var timeout = 3.Seconds();
            var endpoint = new Uri("http://example.org/api/35");

            using var handler = new DelayedResponseHandler(10.Seconds());
            using IRestClient client = new RestClient(handler: handler);
            Should.Throw<TaskCanceledException>(
                async () => await client.GetStringAsync(endpoint, timeout));
                
            Should.Throw<TaskCanceledException>(
                async () => await client.GetStringAsync(endpoint.OriginalString, timeout));
        }

        [Test]
        public async Task When_sending_a_get_stream_with_timeout_uri()
        {
            var timeout = 3.Seconds();

            using var handler = new FakeResponseHandler();
            using var expectedResponse = StringResponseMessage;
            var endpoint = new Uri("http://example.org/api/34");
            handler.AddFakeResponse(endpoint, expectedResponse);

            using IRestClient client = new RestClient(handler: handler);
            var resp = await client.GetStreamAsync(endpoint, timeout);
            using var streamReader = new StreamReader(resp, Encoding.UTF8);
            (await streamReader.ReadToEndAsync()).ShouldBe(ExpectedMessage);
        }

        [Test]
        public async Task When_sending_a_get_stream_with_timeout_string()
        {
            var timeout = 3.Seconds();

            using var handler = new FakeResponseHandler();
            using var expectedResponse = StringResponseMessage;
            var endpoint = new Uri("http://example.org/api/35");
            handler.AddFakeResponse(endpoint, expectedResponse);

            using IRestClient client = new RestClient(handler: handler);
            var resp = await client.GetStreamAsync(endpoint.OriginalString, timeout);
            using var streamReader = new StreamReader(resp, Encoding.UTF8);
            (await streamReader.ReadToEndAsync()).ShouldBe(ExpectedMessage);
        }

        [Test]
        public void When_sending_a_get_stream_which_times_out()
        {
            var timeout = 3.Seconds();
            var endpoint = new Uri("http://example.org/api/36");

            using var handler = new DelayedResponseHandler(10.Seconds());
            using IRestClient client = new RestClient(handler: handler);
            Should.Throw<TaskCanceledException>(
                async () => await client.GetStreamAsync(endpoint, timeout));
                
            Should.Throw<TaskCanceledException>(
                async () => await client.GetStreamAsync(endpoint.OriginalString, timeout));
        }

        [Test]
        public async Task When_sending_a_get_byte_array_with_timeout()
        {
            var timeout = 3.Seconds();

            using var handler = new FakeResponseHandler();
            using var expectedResponse = StringResponseMessage;
            var endpoint = new Uri("http://example.org/api/37");
            handler.AddFakeResponse(endpoint, expectedResponse);

            using IRestClient client = new RestClient(handler: handler);
            var expectedBytes = Encoding.UTF8.GetBytes(ExpectedMessage);
            (await client.GetByteArrayAsync(endpoint, timeout)).ShouldBe(expectedBytes);

            (await client.GetByteArrayAsync(endpoint.OriginalString, timeout)).ShouldBe(expectedBytes);
        }

        [Test]
        public void When_sending_a_get_byte_array_which_times_out()
        {
            var timeout = 3.Seconds();
            var endpoint = new Uri("http://example.org/api/38");

            using var handler = new DelayedResponseHandler(10.Seconds());
            using IRestClient client = new RestClient(handler: handler);
            Should.Throw<TaskCanceledException>(
                async () => await client.GetByteArrayAsync(endpoint, timeout));
                
            Should.Throw<TaskCanceledException>(
                async () => await client.GetByteArrayAsync(endpoint.OriginalString, timeout));
        }

        [Test]
        public async Task When_sending_a_delete_with_timeout()
        {
            var timeout = 3.Seconds();

            using var handler = new FakeResponseHandler();
            using var expectedResponse = StringResponseMessage;
            var endpoint = new Uri("http://example.org/api/39");
            handler.AddFakeResponse(endpoint, expectedResponse);

            using IRestClient client = new RestClient(handler: handler);
            var resp1 = await client.DeleteAsync(endpoint, timeout);
            resp1.EnsureSuccessStatusCode();
            (await resp1.Content.ReadAsStringAsync()).ShouldBe(ExpectedMessage);

            var resp2 = await client.DeleteAsync(endpoint.OriginalString, timeout);
            resp2.EnsureSuccessStatusCode();
            (await resp2.Content.ReadAsStringAsync()).ShouldBe(ExpectedMessage);
        }

        [Test]
        public void When_sending_a_delete_which_times_out()
        {
            var timeout = 3.Seconds();
            var endpoint = new Uri("http://example.org/api/40");

            using var handler = new DelayedResponseHandler(10.Seconds());
            using IRestClient client = new RestClient(handler: handler);
            Should.Throw<TaskCanceledException>(
                async () => await client.DeleteAsync(endpoint, timeout));
                
            Should.Throw<TaskCanceledException>(
                async () => await client.DeleteAsync(endpoint.OriginalString, timeout));
        }

        [Test]
        public async Task When_sending_a_post_with_timeout()
        {
            var timeout = 3.Seconds();

            using var handler = new FakeResponseHandler();
            using var expectedResponse = StringResponseMessage;
            var endpoint = new Uri("http://example.org/api/41");
            handler.AddFakeResponse(endpoint, expectedResponse);

            using IRestClient client = new RestClient(handler: handler);
            var resp1 = await client.PostAsync(endpoint, new MultipartFormDataContent(), timeout);
            resp1.EnsureSuccessStatusCode();
            (await resp1.Content.ReadAsStringAsync()).ShouldBe(ExpectedMessage);

            var resp2 = await client.PostAsync(endpoint.OriginalString, new MultipartFormDataContent(), timeout);
            resp2.EnsureSuccessStatusCode();
            (await resp2.Content.ReadAsStringAsync()).ShouldBe(ExpectedMessage);
        }

        [Test]
        public void When_sending_a_post_which_times_out()
        {
            var timeout = 3.Seconds();
            var endpoint = new Uri("http://example.org/api/42");

            using var handler = new DelayedResponseHandler(10.Seconds());
            using IRestClient client = new RestClient(handler: handler);
            Should.Throw<TaskCanceledException>(
                async () => await client.PostAsync(endpoint, new MultipartFormDataContent(), timeout));
                
            Should.Throw<TaskCanceledException>(
                async () => await client.PostAsync(endpoint.OriginalString, new MultipartFormDataContent(), timeout));
        }

        [Test]
        public async Task When_sending_a_put_with_timeout()
        {
            var timeout = 3.Seconds();

            using var handler = new FakeResponseHandler();
            using var expectedResponse = StringResponseMessage;
            var endpoint = new Uri("http://example.org/api/43");
            handler.AddFakeResponse(endpoint, expectedResponse);

            using IRestClient client = new RestClient(handler: handler);
            var resp1 = await client.PutAsync(endpoint, new MultipartFormDataContent(), timeout);
            resp1.EnsureSuccessStatusCode();
            (await resp1.Content.ReadAsStringAsync()).ShouldBe(ExpectedMessage);

            var resp2 = await client.PutAsync(endpoint.OriginalString, new MultipartFormDataContent(), timeout);
            resp2.EnsureSuccessStatusCode();
            (await resp2.Content.ReadAsStringAsync()).ShouldBe(ExpectedMessage);
        }

        [Test]
        public void When_sending_a_put_which_times_out()
        {
            var timeout = 3.Seconds();
            var endpoint = new Uri("http://example.org/api/44");

            using var handler = new DelayedResponseHandler(10.Seconds());
            using IRestClient client = new RestClient(handler: handler);
            Should.Throw<TaskCanceledException>(
                async () => await client.PutAsync(endpoint, new MultipartFormDataContent(), timeout));
                
            Should.Throw<TaskCanceledException>(
                async () => await client.PutAsync(endpoint.OriginalString, new MultipartFormDataContent(), timeout));
        }
    }

    /// <summary>
    /// <see href="http://chimera.labs.oreilly.com/books/1234000001708/ch14.html#_creating_resuable_response_handlers"/>.
    /// </summary>
    internal sealed class FakeResponseHandler : DelegatingHandler
    {
        private readonly Dictionary<string, HttpResponseMessage> _responses 
            = new Dictionary<string, HttpResponseMessage>();

        public void AddFakeResponse(Uri uri, HttpResponseMessage response) 
            => _responses.Add(uri.OriginalString, response);

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cToken) 
                => Task.FromResult(
                    _responses.TryGetValue(request.RequestUri.OriginalString, out var response) 
                        ? response : new HttpResponseMessage(HttpStatusCode.NotFound) { RequestMessage = request });
    }

    internal sealed class DelayedResponseHandler : DelegatingHandler
    {
        private readonly TimeSpan _delay;

        public DelayedResponseHandler(TimeSpan delay) => _delay = delay;
        
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cToken)
        {
            await Task.Delay(_delay, cToken).ConfigureAwait(false);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}