namespace Easy.Common.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Provides a set of helper methods for working with <see cref="Uri"/>.
    /// </summary>
    public static class UriExtensions
    {
        private static readonly Regex QueryStringRegex = new Regex(@"[?|&]([\w\.-]+)=([^?|^&]+)", RegexOptions.Compiled);

        /// <summary>
        /// Extracts Parameters and Values from the Query String.
        /// <remarks>
        /// This method also correctly <c>URL-decode</c>s the parsed values.
        /// </remarks>
        /// </summary>
        public static IEnumerable<KeyValuePair<string, string>> ParseQueryString(this Uri uri)
        {
            Ensure.NotNull(uri, nameof(uri));

            var match = QueryStringRegex.Match(uri.OriginalString);
            while (match.Success)
            {
                yield return new KeyValuePair<string, string>(
                    match.Groups[1].Value,
                    WebUtility.UrlDecode(match.Groups[2].Value));
                match = match.NextMatch();
            }
        }

        /// <summary>
        /// Adds or appends parameters and values to the query-string.
        /// <remarks>
        /// This method also correctly <c>URL-encode</c>s the given <paramref name="value"/>.
        /// </remarks>
        /// </summary>
        public static Uri AddParametersToQueryString(this Uri uri, string parameter, string value)
        {
            Ensure.NotNull(uri, nameof(uri));
            Ensure.NotNullOrEmptyOrWhiteSpace(parameter);
            Ensure.NotNullOrEmptyOrWhiteSpace(value);

            var queryToAppend = string.Concat(parameter, "=", WebUtility.UrlEncode(value));
            return AddOrAppendToQueryString(uri, queryToAppend);
        }

        private static Uri AddOrAppendToQueryString(Uri uri, string query)
        {
            var baseUri = new UriBuilder(uri);

            if (baseUri.Query.Length > 1)
            {
                baseUri.Query = uri.Query.Substring(1) + "&" + query;
            } else
            {
                baseUri.Query = query;
            }

            return baseUri.Uri;
        }
    }
}