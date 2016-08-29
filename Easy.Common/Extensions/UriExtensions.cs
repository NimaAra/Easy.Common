namespace Easy.Common.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Provides a set of helper methods for working with <see cref="Uri"/>.
    /// </summary>
    public static class UriExtensions
    {
        private static readonly Regex QueryStringRegex = new Regex(@"[?|&]([\w\.-]+)=([^?|^&]+)", RegexOptions.Compiled);

        /// <summary>
        /// Extracts Parameters and Values from the Query String
        /// </summary>
        public static IEnumerable<KeyValuePair<string, string>> ParseQueryString(this Uri uri)
        {
            Ensure.NotNull(uri, nameof(uri));

            var match = QueryStringRegex.Match(uri.OriginalString);
            while (match.Success)
            {
                yield return new KeyValuePair<string, string>(match.Groups[1].Value, match.Groups[2].Value);
                match = match.NextMatch();
            }
        }

        /// <summary>
        /// Adds or appends parameters and values to the query-string.
        /// </summary>
        public static Uri AddParametersToQueryString(this Uri uri, string parameter, string value)
        {
            Ensure.NotNull(uri, nameof(uri));
            Ensure.NotNullOrEmptyOrWhiteSpace(parameter);
            Ensure.NotNullOrEmptyOrWhiteSpace(value);

            var queryToAppend = string.Concat(parameter, "=", value);
            return AddOrAppendToQueryString(uri, queryToAppend);
        }

        /// <summary>
        /// Adds or appends parameters and values to the query-string.
        /// </summary>
        public static Uri AddParametersToQueryString(this Uri uri, string parameter, int value)
        {
            return uri.AddParametersToQueryString(parameter, (long)value);
        }

        /// <summary>
        /// Adds or appends parameters and values to the query-string.
        /// </summary>
        public static Uri AddParametersToQueryString(this Uri uri, string parameter, long value)
        {
            return uri.AddParametersToQueryString(parameter, value.ToString());
        }

        /// <summary>
        /// Applies the specified <paramref name="modification"/> to the <paramref name="uri"/> query-string.
        /// </summary>
        public static string WithModifiedQuerystring(this Uri uri, Action<IEnumerable<KeyValuePair<string, string>>> modification)
        {
            Ensure.NotNull(uri, nameof(uri));
            Ensure.NotNull(modification, nameof(modification));

            var query = uri.ParseQueryString();
            modification(query);

            return string.Concat(uri.GetLeftPart(UriPartial.Path), "?", query.ToString());
        }

        private static Uri AddOrAppendToQueryString(Uri uri, string query)
        {
            var baseUri = new UriBuilder(uri);

            if (baseUri.Query.Length > 1)
            {
                baseUri.Query = uri.Query.Substring(1) + "&" + query;
            }
            else
            {
                baseUri.Query = query;
            }

            return baseUri.Uri;
        }
    }
}