namespace Easy.Common
{
    using System.Collections;
    using System.Collections.Generic;
    using System.DirectoryServices;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Provides methods for obtaining user roles and details from <c>ActiveDirectory</c>.
    /// </summary>
    public static class ActiveDirectoryHelper
    {
        private static readonly Regex Parser = new Regex("CN=(?<CN>[^,]*)", RegexOptions.Compiled);

        /// <summary>
        /// Retrieves user details from <c>Active Directory</c>
        /// </summary>
        /// <param name="userName">The user's user-name</param>
        /// <param name="domainName">The domain name e.g EUR</param>
        /// <returns>Dictionary containing user details</returns>
        public static Dictionary<string, string[]> GetUserDetails(string userName, string domainName)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(userName);
            Ensure.NotNullOrEmptyOrWhiteSpace(domainName);

            var domain = "LDAP://" + domainName;

            using (var dirEntry = new DirectoryEntry(domain))
            using (var adSearch = new DirectorySearcher(dirEntry))
            {
                /*
                    You can get more details
                 
                    adSearch.PropertiesToLoad.Add("sn");
                    adSearch.PropertiesToLoad.Add("cn");
                    adSearch.PropertiesToLoad.Add("givenName");
                    adSearch.PropertiesToLoad.Add("mail");
                    adSearch.PropertiesToLoad.Add("telephoneNumber");
                */
                var filter = "sAMAccountName=" + userName;
                adSearch.Filter = filter;
                var searchResult = adSearch.FindOne();

                var result = new Dictionary<string, string[]>(searchResult.Properties.Count);
                foreach (DictionaryEntry prop in searchResult.Properties)
                {
                    var key = prop.Key.ToString();
                    var vals = searchResult.Properties[key];
                    result[key] = new string[vals.Count];

                    var array = result[key];
                    for (var i = 0; i < vals.Count; i++)
                    {
                        array[i] = vals[i] as string;
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Retrieves user roles from a list of domains.
        /// </summary>
        /// <param name="userName">The user's user-name</param>
        /// <param name="domains">The list of domains to look for the user's roles</param>
        /// <returns>The user roles</returns>
        public static IEnumerable<string> GetRoles(string userName, IEnumerable<string> domains)
        {
            foreach (var domain in domains)
            {
                var query = "LDAP://" + domain;
                using (var dirEntry = new DirectoryEntry(query))
                using (var search = new DirectorySearcher(dirEntry, string.Concat("(sAMAccountName=", userName, ")")))
                {
                    var result = search.FindOne();

                    if (result == null) continue;

                    using (var obUser = new DirectoryEntry(result.Path))
                    {
                        // Invoke Groups method
                        var obGroups = obUser.Invoke("Groups");
                        foreach (var obGroup in (IEnumerable)obGroups)
                        {
                            using (var obGpEntry = new DirectoryEntry(obGroup))
                            {
                                var x = Parser.Match(obGpEntry.Name);
                                if (!x.Success) continue;

                                var grp = x.Groups["CN"].Value;
                                yield return grp;
                            }
                        }
                    }
                }
            }
        }
    }

}