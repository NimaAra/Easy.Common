namespace Easy.Common.Tests.Unit.RestClient
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Easy.Common.Extensions;
    using Easy.Common.Interfaces;
    using NUnit.Framework;
    using Shouldly;
    using Ensure = Easy.Common.Ensure;
    using RestClient = Easy.Common.RestClient;

    [TestFixture]
    internal sealed class RestClientTests
    {
        [Test]
        public void When_creating_a_client_with_default_constructor()
        {
            using (var client = new RestClient())
            {
                client.DefaultRequestHeaders.ShouldBeEmpty();
                client.Endpoints.ShouldBeEmpty();
                client.MaxResponseContentBufferSize.ShouldBe((uint)int.MaxValue);
                client.Timeout.ShouldBe(new TimeSpan(0, 0, 1, 40));
            }
        }

        [Test]
        public void When_creating_a_client_with_custom_constructor()
        {
            var defaultHeaders = new Dictionary<string, string>
            {
                {HttpRequestHeader.Accept.ToString(), "application/json"},
                {HttpRequestHeader.UserAgent.ToString(), "foo-bar"}
            };

            using (IRestClient client = new RestClient(defaultHeaders, timeout: 15.Seconds(), maxResponseContentBufferSize: 10))
            {
                client.DefaultRequestHeaders.Count.ShouldBe(defaultHeaders.Count);
                client.DefaultRequestHeaders["Accept"].ShouldBe("application/json");
                client.DefaultRequestHeaders["UserAgent"].ShouldBe("foo-bar");

                client.Endpoints.ShouldBeEmpty();
                client.MaxResponseContentBufferSize.ShouldBe((uint)10);
                client.Timeout.ShouldBe(15.Seconds());
            }
        }

        [Test]
        public void When_sending_a_request()
        {
            var endpoint = new Uri("http://localhost/api");
            ServicePointManager.FindServicePoint(endpoint).ConnectionLeaseTimeout.ShouldBe(-1);

            using (IRestClient client = new RestClient())
            {
                client.SendAsync(new HttpRequestMessage(HttpMethod.Get, endpoint));

                ServicePointManager.FindServicePoint(endpoint)
                    .ConnectionLeaseTimeout
                    .ShouldBe((int)1.Minutes().TotalMilliseconds);

                client.Endpoints.Length.ShouldBe(1);
                client.Endpoints[0].ShouldBe(endpoint);

                client.ClearEndpoints();
                client.Endpoints.ShouldBeEmpty();
            }
        }

        [Test]
        public void When_sending_a_request_with_string_uri()
        {
            var endpoint = "http://localhost:33/api";
            var endpointUri = new Uri(endpoint);
            ServicePointManager.FindServicePoint(endpointUri).ConnectionLeaseTimeout.ShouldBe(-1);

            using (IRestClient client = new RestClient())
            {
                client.SendAsync(new HttpRequestMessage(HttpMethod.Get, endpoint));

                ServicePointManager.FindServicePoint(endpointUri)
                    .ConnectionLeaseTimeout
                    .ShouldBe((int)1.Minutes().TotalMilliseconds);

                client.Endpoints.Length.ShouldBe(1);
                client.Endpoints[0].OriginalString.ShouldBe(endpoint);

                client.ClearEndpoints();
                client.Endpoints.ShouldBeEmpty();
            }
        }

        [Test]
        public async Task When_sending_a_get_request()
        {

            var endpoint = new Uri("http://localhost:1/api/");

            using (IRestClient client = new RestClient())
            using (var server = new SimpleHttpListener(endpoint))
            {
                server.OnRequest += (sender, context) =>
                {
                    if (context.Request.HttpMethod != HttpMethod.Get.ToString())
                    {
                        context.Response.StatusCode = (int) HttpStatusCode.NotAcceptable;
                        context.Response.Close();
                    } else
                    {
                        var reply = Encoding.UTF8.GetBytes("Hello There!");
                        context.Response.StatusCode = (int) HttpStatusCode.OK;
                        context.Response.OutputStream.Write(reply, 0, reply.Length);
                        context.Response.Close();
                    }
                };

                await server.ListenAsync();

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = endpoint
                };
                request.Headers.Add("Foo", "Bar");

                var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
                response.EnsureSuccessStatusCode();
                var respStr = await response.Content.ReadAsStringAsync();
                respStr.ShouldBe("Hello There!");
            }
        }

        [Test]
        public async Task When_sending_a_put_request_via_explicit_put_with_uri()
        {
            var endpoint = new Uri("http://localhost:2/api/");

            using (IRestClient client = new RestClient())
            using (var server = new SimpleHttpListener(endpoint))
            {
                server.OnRequest += (sender, context) =>
                {
                    if (context.Request.HttpMethod != HttpMethod.Put.ToString())
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        context.Response.Close();
                    }
                    else
                    {
                        var reply = Encoding.UTF8.GetBytes("Hello There!");
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.OutputStream.Write(reply, 0, reply.Length);
                        context.Response.Close();
                    }
                };

                await server.ListenAsync();

                var response = await client.PutAsync(endpoint,new MultipartFormDataContent());
                response.EnsureSuccessStatusCode();
                var respStr = await response.Content.ReadAsStringAsync();
                respStr.ShouldBe("Hello There!");
            }
        }

        [Test]
        public async Task When_sending_a_put_request_via_explicit_put_with_string()
        {
            var endpoint = "http://localhost:3/api/";

            using (IRestClient client = new RestClient())
            using (var server = new SimpleHttpListener(new Uri(endpoint)))
            {
                server.OnRequest += (sender, context) =>
                {
                    if (context.Request.HttpMethod != HttpMethod.Put.ToString())
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        context.Response.Close();
                    }
                    else
                    {
                        var reply = Encoding.UTF8.GetBytes("Hello There!");
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.OutputStream.Write(reply, 0, reply.Length);
                        context.Response.Close();
                    }
                };

                await server.ListenAsync();

                var response = await client.PutAsync(endpoint, new MultipartFormDataContent());
                response.EnsureSuccessStatusCode();
                var respStr = await response.Content.ReadAsStringAsync();
                respStr.ShouldBe("Hello There!");
            }
        }

        [Test]
        public async Task When_sending_a_put_request_via_explicit_put_with_uri_with_cancellation()
        {
            var endpoint = new Uri("http://localhost:4/api/");

            using (IRestClient client = new RestClient())
            using (var server = new SimpleHttpListener(endpoint))
            {

                await server.ListenAsync();

                var cts = new CancellationTokenSource(1.Seconds());
                Should.Throw<TaskCanceledException>(async () => await client.PutAsync(endpoint, new MultipartFormDataContent(), cts.Token));
               
            }
        }

        [Test]
        public async Task When_sending_a_put_request_via_explicit_put_with_string_with_cancellation()
        {
            var endpoint = "http://localhost:5/api/";

            using (IRestClient client = new RestClient())
            using (var server = new SimpleHttpListener(new Uri(endpoint)))
            {

                await server.ListenAsync();

                var cts = new CancellationTokenSource(1.Seconds());
                Should.Throw<TaskCanceledException>(async () => await client.PutAsync(endpoint, new MultipartFormDataContent(), cts.Token));

            }
        }

        [Test]
        public async Task When_sending_a_post_request()
        {
            var endpoint = new Uri("http://localhost:6/api/");
            using (IRestClient client = new RestClient())
            using (var server = new SimpleHttpListener(endpoint))
            {
                server.OnRequest += (sender, context) =>
                {
                    if (context.Request.HttpMethod != HttpMethod.Post.ToString())
                    {
                        context.Response.StatusCode = (int) HttpStatusCode.NotAcceptable;
                        context.Response.Close();
                    } else
                    {
                        var reply = Encoding.UTF8.GetBytes("Hello There!");
                        context.Response.StatusCode = (int) HttpStatusCode.OK;
                        context.Response.OutputStream.Write(reply, 0, reply.Length);
                        context.Response.Close();
                    }
                };

                await server.ListenAsync();

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = endpoint
                };
                request.Headers.Add("Foo", "Bar");

                var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
                response.EnsureSuccessStatusCode();
                var respStr = await response.Content.ReadAsStringAsync();
                respStr.ShouldBe("Hello There!");
            }
        }

        [Test]
        public async Task When_sending_a_post_request_via_explicit_post_with_uri()
        {
            var endpoint = new Uri("http://localhost:7/api/");
            using (IRestClient client = new RestClient())
            using (var server = new SimpleHttpListener(endpoint))
            {
                server.OnRequest += (sender, context) =>
                {
                    if (context.Request.HttpMethod != HttpMethod.Post.ToString())
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        context.Response.Close();
                    }
                    else
                    {
                        var reply = Encoding.UTF8.GetBytes("Hello There!");
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.OutputStream.Write(reply, 0, reply.Length);
                        context.Response.Close();
                    }
                };

                await server.ListenAsync();

                var content = new MultipartFormDataContent();

                var response = await client.PostAsync(endpoint, content);
                response.EnsureSuccessStatusCode();
                var respStr = await response.Content.ReadAsStringAsync();
                respStr.ShouldBe("Hello There!");
            }
        }

        [Test]
        public async Task When_sending_a_post_request_via_explicit_post_with_string()
        {
            var endpoint = "http://localhost:8/api/";
            using (IRestClient client = new RestClient())
            using (var server = new SimpleHttpListener(new Uri(endpoint)))
            {
                server.OnRequest += (sender, context) =>
                {
                    if (context.Request.HttpMethod != HttpMethod.Post.ToString())
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        context.Response.Close();
                    }
                    else
                    {
                        var reply = Encoding.UTF8.GetBytes("Hello There!");
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.OutputStream.Write(reply, 0, reply.Length);
                        context.Response.Close();
                    }
                };

                await server.ListenAsync();

                var content = new MultipartFormDataContent();

                var response = await client.PostAsync(endpoint, content);
                response.EnsureSuccessStatusCode();
                var respStr = await response.Content.ReadAsStringAsync();
                respStr.ShouldBe("Hello There!");
            }
        }

        [Test]
        public async Task When_sending_a_post_request_via_explicit_post_with_uri_with_cancellation_token()
        {
            var endpoint = new Uri("http://localhost:9/api/");
            using (IRestClient client = new RestClient())
            using (var server = new SimpleHttpListener(endpoint))
            {
                await server.ListenAsync();


                var cts = new CancellationTokenSource(1.Seconds());
                Should.Throw<TaskCanceledException>(async () => await client.PostAsync(endpoint, new MultipartFormDataContent(),cts.Token));
            }
        }

        [Test]
        public async Task When_sending_a_post_request_via_explicit_post_with_string_with_cancellation_token()
        {
            var endpoint = "http://localhost:10/api/";
            using (IRestClient client = new RestClient())
            using (var server = new SimpleHttpListener(new Uri(endpoint)))
            {
                await server.ListenAsync();


                var cts = new CancellationTokenSource(1.Seconds());
                Should.Throw<TaskCanceledException>(async () => await client.PostAsync(endpoint, new MultipartFormDataContent(), cts.Token));
            }
        }

        [Test]
        public async Task When_sending_a_get_request_via_explicit_get_with_uri()
        {

            var endpoint = new Uri("http://localhost:11/api/");

            using (IRestClient client = new RestClient())
            using (var server = new SimpleHttpListener(endpoint))
            {
                server.OnRequest += (sender, context) =>
                {
                    if (context.Request.HttpMethod != HttpMethod.Get.ToString())
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        context.Response.Close();
                    }
                    else
                    {
                        var reply = Encoding.UTF8.GetBytes("Hello There!");
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.OutputStream.Write(reply, 0, reply.Length);
                        context.Response.Close();
                    }
                };

                await server.ListenAsync();

                var response = await client.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();
                var respStr = await response.Content.ReadAsStringAsync();
                respStr.ShouldBe("Hello There!");
            }
        }

        [Test]
        public async Task When_sending_a_get_request_via_explicit_get_with_string()
        {

            var endpoint = "http://localhost:12/api/";

            using (IRestClient client = new RestClient())
            using (var server = new SimpleHttpListener(new Uri(endpoint)))
            {
                server.OnRequest += (sender, context) =>
                {
                    if (context.Request.HttpMethod != HttpMethod.Get.ToString())
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        context.Response.Close();
                    }
                    else
                    {
                        var reply = Encoding.UTF8.GetBytes("Hello There!");
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.OutputStream.Write(reply, 0, reply.Length);
                        context.Response.Close();
                    }
                };

                await server.ListenAsync();

                var response = await client.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();
                var respStr = await response.Content.ReadAsStringAsync();
                respStr.ShouldBe("Hello There!");
            }
        }

        [Test]
        public async Task When_sending_a_get_request_via_explicit_get_with_uri_with_cancellation_token()
        {

            var endpoint = new Uri("http://localhost:13/api/");

            using (IRestClient client = new RestClient())
            using (var server = new SimpleHttpListener(endpoint))
            {
                await server.ListenAsync();

                var cts = new CancellationTokenSource(1.Seconds());
                Should.Throw<TaskCanceledException>(async () => await client.GetAsync(endpoint,cts.Token));
            }
        }

        [Test]
        public async Task When_sending_a_get_request_via_explicit_get_with_string_with_cancellation_token()
        {

            var endpoint = "http://localhost:14/api/";

            using (IRestClient client = new RestClient())
            using (var server = new SimpleHttpListener(new Uri(endpoint)))
            {
                await server.ListenAsync();

                var cts = new CancellationTokenSource(1.Seconds());
                Should.Throw<TaskCanceledException>(async () => await client.GetAsync(endpoint, cts.Token));
            }
        }

        [Test]
        public async Task When_sending_a_get_request_via_explicit_get_with_uri_with_completion_option()
        {

            var endpoint = new Uri("http://localhost:15/api/");

            using (IRestClient client = new RestClient())
            using (var server = new SimpleHttpListener(endpoint))
            {
                server.OnRequest += (sender, context) =>
                {
                    if (context.Request.HttpMethod != HttpMethod.Get.ToString())
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        context.Response.Close();
                    }
                    else
                    {
                        var reply = Encoding.UTF8.GetBytes("Hello There!");
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.OutputStream.Write(reply, 0, reply.Length);
                        context.Response.Close();
                    }
                };

                await server.ListenAsync();

                var response = await client.GetAsync(endpoint,HttpCompletionOption.ResponseContentRead);
                response.EnsureSuccessStatusCode();
                var respStr = await response.Content.ReadAsStringAsync();
                respStr.ShouldBe("Hello There!");
            }
        }

        [Test]
        public async Task When_sending_a_get_request_via_explicit_get_with_string_with_completion_option()
        {

            var endpoint = "http://localhost:16/api/";

            using (IRestClient client = new RestClient())
            using (var server = new SimpleHttpListener(new Uri(endpoint)))
            {
                server.OnRequest += (sender, context) =>
                {
                    if (context.Request.HttpMethod != HttpMethod.Get.ToString())
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        context.Response.Close();
                    }
                    else
                    {
                        var reply = Encoding.UTF8.GetBytes("Hello There!");
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.OutputStream.Write(reply, 0, reply.Length);
                        context.Response.Close();
                    }
                };

                await server.ListenAsync();

                var response = await client.GetAsync(endpoint, HttpCompletionOption.ResponseContentRead);
                response.EnsureSuccessStatusCode();
                var respStr = await response.Content.ReadAsStringAsync();
                respStr.ShouldBe("Hello There!");
            }
        }

        [Test]
        public async Task When_sending_a_get_request_via_explicit_get_with_uri_with_completion_option_with_cancellation_token()
        {

            var endpoint = new Uri("http://localhost:17/api/");

            using (IRestClient client = new RestClient())
            using (var server = new SimpleHttpListener(endpoint))
            {
                await server.ListenAsync();
                var cts = new CancellationTokenSource(1.Seconds());
                Should.Throw<TaskCanceledException>(async () => await client.GetAsync(endpoint, HttpCompletionOption.ResponseContentRead, cts.Token));
            }
        }

        [Test]
        public async Task When_sending_a_get_request_via_explicit_get_with_string_with_completion_option_with_cancelation_token()
        {

            var endpoint = "http://localhost:18/api/";

            using (IRestClient client = new RestClient())
            using (var server = new SimpleHttpListener(new Uri(endpoint)))
            {
                await server.ListenAsync();
                var cts = new CancellationTokenSource(1.Seconds());
                Should.Throw<TaskCanceledException>(async () => await client.GetAsync(endpoint, HttpCompletionOption.ResponseContentRead, cts.Token));
            }
        }

        [Test]
        public async Task When_sending_a_delete_request()
        {
            var endpoint = new Uri("http://localhost:19/api/");
            using (IRestClient client = new RestClient())
            using (var server = new SimpleHttpListener(endpoint))
            {
                server.OnRequest += (sender, context) =>
                {
                    if (context.Request.HttpMethod != HttpMethod.Delete.ToString())
                    {
                        context.Response.StatusCode = (int) HttpStatusCode.NotAcceptable;
                        context.Response.Close();
                    } else
                    {
                        var reply = Encoding.UTF8.GetBytes("Hello There!");
                        context.Response.StatusCode = (int) HttpStatusCode.OK;
                        context.Response.OutputStream.Write(reply, 0, reply.Length);
                        context.Response.Close();
                    }
                };

                await server.ListenAsync();

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = endpoint
                };
                request.Headers.Add("Foo", "Bar");

                var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
                response.EnsureSuccessStatusCode();
                var respStr = await response.Content.ReadAsStringAsync();
                respStr.ShouldBe("Hello There!");
            }
        }

        [Test]
        public async Task When_sending_a_delete_request_via_explicit_delete_with_uri()
        {
            var endpoint = new Uri("http://localhost:20/api/");
            using (IRestClient client = new RestClient())
            using (var server = new SimpleHttpListener(endpoint))
            {
                server.OnRequest += (sender, context) =>
                {
                    if (context.Request.HttpMethod != HttpMethod.Delete.ToString())
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        context.Response.Close();
                    }
                    else
                    {
                        var reply = Encoding.UTF8.GetBytes("Hello There!");
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.OutputStream.Write(reply, 0, reply.Length);
                        context.Response.Close();
                    }
                };

                await server.ListenAsync();

                var response = await client.DeleteAsync(endpoint);
                response.EnsureSuccessStatusCode();
                var respStr = await response.Content.ReadAsStringAsync();
                respStr.ShouldBe("Hello There!");
            }
        }

        [Test]
        public async Task When_sending_a_delete_request_via_explicit_delete_with_string()
        {
            var endpoint ="http://localhost:21/api/";
            using (IRestClient client = new RestClient())
            using (var server = new SimpleHttpListener(new Uri(endpoint)))
            {
                server.OnRequest += (sender, context) =>
                {
                    if (context.Request.HttpMethod != HttpMethod.Delete.ToString())
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        context.Response.Close();
                    }
                    else
                    {
                        var reply = Encoding.UTF8.GetBytes("Hello There!");
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.OutputStream.Write(reply, 0, reply.Length);
                        context.Response.Close();
                    }
                };

                await server.ListenAsync();

                var response = await client.DeleteAsync(endpoint);
                response.EnsureSuccessStatusCode();
                var respStr = await response.Content.ReadAsStringAsync();
                respStr.ShouldBe("Hello There!");
            }
        }

        [Test]
        public async Task When_sending_a_delete_request_via_explicit_delete_with_uri_with_cancellation_token()
        {
            var endpoint = new Uri("http://localhost:22/api/");
            using (IRestClient client = new RestClient())
            using (var server = new SimpleHttpListener(endpoint))
            {
                await server.ListenAsync();

                var cts = new CancellationTokenSource(1.Seconds());
                Should.Throw<TaskCanceledException>(async () => await client.DeleteAsync(endpoint, cts.Token));
            }
        }

        [Test]
        public async Task When_sending_a_delete_request_via_explicit_delete_with_string_with_cancellation_token()
        {
            var endpoint = "http://localhost:23/api/";
            using (IRestClient client = new RestClient())
            using (var server = new SimpleHttpListener(new Uri(endpoint)))
            {
                await server.ListenAsync();
                var cts = new CancellationTokenSource(1.Seconds());
                Should.Throw<TaskCanceledException>(async () => await client.DeleteAsync(endpoint, cts.Token));
            }
        }

        [Test]
        public async Task When_sending_a_request_with_cancellation_and_completion_option()
        {
            var endpoint = new Uri("http://localhost:24/api/");
            using (IRestClient client = new RestClient())
            using (var server = new SimpleHttpListener(endpoint))
            {
                await server.ListenAsync();

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = endpoint
                };
                request.Headers.Add("Foo", "Bar");

                var cts = new CancellationTokenSource(1.Seconds());
                Should.Throw<TaskCanceledException>(
                    async () => await client.SendAsync(request, HttpCompletionOption.ResponseContentRead, cts.Token));
            }
        }

        [Test]
        public async Task When_sending_a_request_with_cancellation()
        {
            var endpoint = new Uri("http://localhost:25/api/");
            using (IRestClient client = new RestClient())
            using (var server = new SimpleHttpListener(endpoint))
            {
                await server.ListenAsync();

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = endpoint
                };
                request.Headers.Add("Foo", "Bar");

                var cts = new CancellationTokenSource(1.Seconds());
                Should.Throw<TaskCanceledException>(async () => await client.SendAsync(request, cts.Token));
            }
        }

        [Test]
        public async Task When_sending_a_request_then_cancelling_all_pending_requests()
        {
            var endpoint = new Uri("http://localhost:26/api/");
            using (IRestClient client = new RestClient())
            using (var server = new SimpleHttpListener(endpoint))
            {
                await server.ListenAsync();

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = endpoint
                };
                request.Headers.Add("Foo", "Bar");

                var copy = client;
#pragma warning disable 4014
                Task.Delay(1.Seconds()).ContinueWith(_ => copy.CancelPendingRequests());
#pragma warning restore 4014
                Should.Throw<TaskCanceledException>(async () => await client.SendAsync(request));
            }
        }

        [Test]
        public async Task When_sending_a_get_string_request_with_uri()
        {
            var endpoint = new Uri("http://localhost:27/api/");
            using (IRestClient client = new RestClient())
            using (var server = new SimpleHttpListener(endpoint))
            {
                server.OnRequest += (sender, context) =>
                {
                    if (context.Request.HttpMethod != HttpMethod.Get.ToString())
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        context.Response.Close();
                    }
                    else
                    {
                        var reply = Encoding.UTF8.GetBytes("Hello There!");
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.OutputStream.Write(reply, 0, reply.Length);
                        context.Response.Close();
                    }
                };
                await server.ListenAsync();
                var response = await client.GetStringAsync(endpoint);

                response.ShouldBe("Hello There!");
            }
        }

        [Test]
        public async Task When_sending_a_get_string_request_with_string()
        {
            var endpoint = "http://localhost:27/api/";
            using (IRestClient client = new RestClient())
            using (var server = new SimpleHttpListener(new Uri(endpoint)))
            {
                server.OnRequest += (sender, context) =>
                {
                    if (context.Request.HttpMethod != HttpMethod.Get.ToString())
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        context.Response.Close();
                    }
                    else
                    {
                        var reply = Encoding.UTF8.GetBytes("Hello There!");
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.OutputStream.Write(reply, 0, reply.Length);
                        context.Response.Close();
                    }
                };
                await server.ListenAsync();
                var response = await client.GetStringAsync(endpoint);

                response.ShouldBe("Hello There!");
            }
        }

        [Test]
        public async Task When_sending_a_get_stream_request_with_uri()
        {
            var endpoint = new Uri("http://localhost:28/api/");
            using (IRestClient client = new RestClient())
            using (var server = new SimpleHttpListener(endpoint))
            {
                server.OnRequest += (sender, context) =>
                {
                    if (context.Request.HttpMethod != HttpMethod.Get.ToString())
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        context.Response.Close();
                    }
                    else
                    {
                        var reply = Encoding.UTF8.GetBytes("Hello There!");
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.OutputStream.Write(reply, 0, reply.Length);
                        context.Response.Close();
                    }
                };
                await server.ListenAsync();
                var response = await client.GetStreamAsync(endpoint);
                using (var streamReader = new StreamReader(response, Encoding.UTF8))
                {
                    var responseString = await streamReader.ReadToEndAsync();
                    responseString.ShouldBe("Hello There!");
                }
            }
        }

        [Test]
        public async Task When_sending_a_get_stream_request_with_string()
        {
            var endpoint = "http://localhost:29/api/";
            using (IRestClient client = new RestClient())
            using (var server = new SimpleHttpListener(new Uri(endpoint)))
            {
                server.OnRequest += (sender, context) =>
                {
                    if (context.Request.HttpMethod != HttpMethod.Get.ToString())
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        context.Response.Close();
                    }
                    else
                    {
                        var reply = Encoding.UTF8.GetBytes("Hello There!");
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.OutputStream.Write(reply, 0, reply.Length);
                        context.Response.Close();
                    }
                };
                await server.ListenAsync();
                var response = await client.GetStreamAsync(endpoint);
                using (var streamReader = new StreamReader(response,Encoding.UTF8))
                {
                    var responseString = await streamReader.ReadToEndAsync();
                    responseString.ShouldBe("Hello There!");
                }
            }
        }

        [Test]
        public async Task When_sending_a_get_byte_array_request_with_uri()
        {
            var endpoint = new Uri("http://localhost:30/api/");
            using (IRestClient client = new RestClient())
            using (var server = new SimpleHttpListener(endpoint))
            {
                server.OnRequest += (sender, context) =>
                {
                    if (context.Request.HttpMethod != HttpMethod.Get.ToString())
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        context.Response.Close();
                    }
                    else
                    {
                        var reply = Encoding.UTF8.GetBytes("Hello There!");
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.OutputStream.Write(reply, 0, reply.Length);
                        context.Response.Close();
                    }
                };
                await server.ListenAsync();
                var response = await client.GetByteArrayAsync(endpoint);
                var responseString = Encoding.UTF8.GetString(response);
                responseString.ShouldBe("Hello There!");
            }
        }

        [Test]
        public async Task When_sending_a_get_byte_array_request_with_string()
        {
            var endpoint = "http://localhost:31/api/";
            using (IRestClient client = new RestClient())
            using (var server = new SimpleHttpListener(new Uri(endpoint)))
            {
                server.OnRequest += (sender, context) =>
                {
                    if (context.Request.HttpMethod != HttpMethod.Get.ToString())
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        context.Response.Close();
                    }
                    else
                    {
                        var reply = Encoding.UTF8.GetBytes("Hello There!");
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.OutputStream.Write(reply, 0, reply.Length);
                        context.Response.Close();
                    }
                };
                await server.ListenAsync();
                var response = await client.GetByteArrayAsync(endpoint);
                var responseString = Encoding.UTF8.GetString(response);
                responseString.ShouldBe("Hello There!");
            }
        }

        [Test]
        public async Task CallFakeRequest()
        {
            var fakeResponseHandler = new FakeResponseHandler();
            fakeResponseHandler.AddFakeResponse(new Uri("http://example.org/test"), new HttpResponseMessage(HttpStatusCode.OK));

            var httpClient = new HttpClient(fakeResponseHandler);

            var response1 = await httpClient.GetAsync("http://example.org/notthere");
            var response2 = await httpClient.GetAsync("http://example.org/test");

            response1.StatusCode.ShouldBe(HttpStatusCode.NotFound);
            response2.StatusCode.ShouldBe(HttpStatusCode.OK);
        }
    }

    /// <summary>
    /// <see href="http://chimera.labs.oreilly.com/books/1234000001708/ch14.html#_creating_resuable_response_handlers"/>.
    /// </summary>
    public class FakeResponseHandler : DelegatingHandler
    {
        private readonly Dictionary<Uri, HttpResponseMessage> _fakeResponses = new Dictionary<Uri, HttpResponseMessage>();

        public void AddFakeResponse(Uri uri, HttpResponseMessage responseMessage)
        {
            _fakeResponses.Add(uri, responseMessage);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_fakeResponses.ContainsKey(request.RequestUri))
            {
                return await Task.FromResult(_fakeResponses[request.RequestUri]);
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound) { RequestMessage = request };
        }
    }

    internal sealed class SimpleHttpListener : IDisposable
    {
        private readonly HttpListener _listener;

        internal EventHandler<HttpListenerContext> OnRequest;
        internal EventHandler<HttpListenerException> OnError;

        internal SimpleHttpListener(params Uri[] prefixes)
        {
            Ensure.NotNull(prefixes, nameof(prefixes));
            _listener = new HttpListener();

            if (prefixes == null || prefixes.Length == 0)
            {
                _listener.Prefixes.Add("http://*:80/");
            } else
            {
                Array.ForEach(prefixes, p =>
                {
                    _listener.Prefixes.Add(p.AbsoluteUri);
                });
            }
        }

        internal Task ListenAsync()
        {
            ListenAsyncImpl();
            return Task.FromResult(false);
        }

        public void Dispose()
        {
            _listener.Stop();
        }
        
        private async void ListenAsyncImpl()
        {
            _listener.Start();
            while (_listener.IsListening)
            {
                HttpListenerContext ctx = null;

                try
                {
                    ctx = await _listener.GetContextAsync();
                } catch (HttpListenerException e)
                {
                    OnError?.Invoke(this, e);
                }

                if (ctx == null) { return; }

                OnRequest?.Invoke(this, ctx);
            }
        }
    }
}