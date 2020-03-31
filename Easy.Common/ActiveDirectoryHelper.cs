#if NETFRAMEWORK || NETSTANDARD2_0
namespace Easy.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Principal;

    /// <summary>
    /// A helper class for working with <c>Active Directory</c>.
    /// </summary>
    public static class ActiveDirectoryHelper
    {
        /// <summary>
        /// Returns the groups the current user is member of.
        /// </summary>
        public static HashSet<string> GetGroupsForCurrentUser() => 
            GetGroups(WindowsIdentity.GetCurrent());

        /// <summary>
        /// Returns the groups the given <paramref name="userPrincipalName"/> is a member of.
        /// </summary>
        public static HashSet<string> GetGroups(string userPrincipalName)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(userPrincipalName);
            return GetGroups(new WindowsIdentity(userPrincipalName));
        }

        /// <summary>
        /// Returns the groups the given <paramref name="identity"/> is a member of.
        /// </summary>
        public static HashSet<string> GetGroups(WindowsIdentity identity)
        {
            var result = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            if (identity.Groups is null) { return result; }

            var type = typeof(NTAccount);
            foreach (var group in identity.Groups)
            {
                result.Add(group.Translate(type).ToString());
            }
            return result;
        }

        /// <summary>
        /// Determines whether the given <paramref name="userLogon"/> is a member of
        /// the given <paramref name="groupName"/>.
        /// </summary>
        public static bool IsGroupMember(string userLogon, string groupName)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(userLogon);
            Ensure.NotNullOrEmptyOrWhiteSpace(groupName);

            var groups = GetGroups(userLogon);

            const StringComparison CmpPolicy = StringComparison.InvariantCultureIgnoreCase;
            if (groupName.Equals("Everyone", CmpPolicy) || groupName.Equals("Administrators", CmpPolicy))
            {
                return groups.Contains(groupName);
            }

            return groupName.Contains("\\") // if contains domain
                ? groups.Contains(groupName)
                : groups.Any(g => g.EndsWith("\\" + groupName, CmpPolicy));
        }

        /// <summary>
        /// Compares the group membership for each of the items in <paramref name="nameToUserLogonMap"/>
        /// and generates a <c>HTML</c> report indicating the commonality as well as differences 
        /// between each users.
        /// </summary>
        public static string GenerateGroupComparisonReport(IEnumerable<KeyValuePair<string, string>> nameToUserLogonMap)
        {
            var userAndRoles = nameToUserLogonMap.AsParallel()
                .ToDictionary(kv => kv.Key, kv => GetGroups(kv.Value));

            return GenerateHTML(new ComparisonResult(userAndRoles));
        }

        private static string GenerateHTML(ComparisonResult result)
        {
            const string HTML = @"
            <!DOCTYPE html>
            <html lang=""en"">
            <head>
                <style>
                    table {
                        border-collapse:collapse;
                        font-family:arial;
                        font-size:14px;
                    }
                    table, thead, th {
                        height:10px;
                        padding:10px;
                        vertical-align:middle;
                    }
                    table, th, td {
                        border:1px black solid;
                        padding-left:5px;
                        background-color:#fbf7f7;
                        text-align:center;
                    }
                    table, tr {
                        text-align:left;
                    }

                    .equalRowClass {
                        background-color:#62f762;
                        color:black;
                        text-align:left;
                    }
                    .unEqualRowClass {
                        background-color:#f74a4a;
                        color:black;
                        text-align:left;
                    }

                    .hasRoleClass {
                        background-color:#99ddff;
                    }
                    .notHasRoleClass {
                        background-color:#ffcc00;
                    }
                </style>
            </head>
            <body>
                ~~~TABLE~~~
            </body>
            </html>";
            
            var builder = StringBuilderCache.Acquire();
            builder.Append("<table><thead><th><b>Role</b></th>");
            foreach (var item in result.NamesToRolesMap.Keys)
            {
                builder.AppendFormat("<th><b>{0}</b></th>", item);
            }
            builder.Append("</thead>");

            const string EqualityRowClass = "equalRowClass";
            const string UnEqualityRowClass = "unEqualRowClass";
            const string HasRoleText = "Yes";
            const string NotHaveRoleText = "No";
            const string HasRoleClass = "hasRoleClass";
            const string NotHasRoleClass = "notHasRoleClass";

            foreach (var role in result.CommonRoles)
            {
                builder.AppendFormat("<tr><td class=\"{0}\">{1}</td>", EqualityRowClass, role);

                foreach (var _ in result.NamesToRolesMap.Keys)
                {
                    builder.AppendFormat("<td class=\"{0}\">{1}</td>", HasRoleClass, HasRoleText);
                }
                builder.Append("</tr>");
            }

            foreach (var role in result.AllRoles.Except(result.CommonRoles))
            {
                builder.AppendFormat("<tr><td class=\"{0}\">{1}</td>", UnEqualityRowClass, role);

                foreach (var name in result.NamesToRolesMap.Keys)
                {
                    builder.AppendFormat("<td class=\"{0}\">{1}</td>",
                        result.NamesToRolesMap[name].Contains(role) ? HasRoleClass : NotHasRoleClass,
                        result.NamesToRolesMap[name].Contains(role) ? HasRoleText : NotHaveRoleText);
                }
                builder.Append("</tr>");
            }

            builder.Append("</table>");

            return HTML.Replace("~~~TABLE~~~", StringBuilderCache.GetStringAndRelease(builder));
        }

        private sealed class ComparisonResult
        {
            public ComparisonResult(Dictionary<string, HashSet<string>> namesToRolesMap)
            {
                var comparer = StringComparer.InvariantCultureIgnoreCase;
                NamesToRolesMap = namesToRolesMap;
                AllRoles = new HashSet<string>(NamesToRolesMap.SelectMany(x => x.Value), comparer);

                var intersection = AllRoles;
                foreach (var pair in NamesToRolesMap)
                {
                    intersection = intersection.Intersect(pair.Value);
                }

                CommonRoles = new HashSet<string>(intersection, comparer);
            }

            public Dictionary<string, HashSet<string>> NamesToRolesMap { get; }
            public IEnumerable<string> CommonRoles { get; }
            public IEnumerable<string> AllRoles { get; }
        }
    }
}
#endif