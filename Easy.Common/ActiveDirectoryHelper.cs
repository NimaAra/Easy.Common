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
        public static HashSet<string> GetGroupsForCurrentUser() 
            => GetGroupsImpl(WindowsIdentity.GetCurrent());

        /// <summary>
        /// Returns the groups the given <paramref name="userPrincipalName"/> is a member of.
        /// </summary>
        public static HashSet<string> GetGroupsForUser(string userPrincipalName)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(userPrincipalName);
            return GetGroupsImpl(new WindowsIdentity(userPrincipalName));
        }

        /// <summary>
        /// Compares the group membership for each of the items in <paramref name="userLogons"/>
        /// and generates a <c>HTML</c> report indicating the commonality as well as differences 
        /// between each users.
        /// </summary>
        public static string GenerateGroupComparisonReport(params string[] userLogons)
        {
            Ensure.NotNull(userLogons, nameof(userLogons));

            var userAndRoles = userLogons
                .AsParallel()
                .WithDegreeOfParallelism(userLogons.Length)
                .ToDictionary(u => u, GetGroupsForUser);

            return GenerateHTML(new ComparisonResult(userAndRoles));
        }

        private static HashSet<string> GetGroupsImpl(WindowsIdentity identity)
        {
            var result = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            if (identity.Groups == null) { return result; }

            var type = typeof(NTAccount);
            foreach (var group in identity.Groups)
            {
                result.Add(group.Translate(type).ToString());
            }
            return result;
        }

        // ReSharper disable once InconsistentNaming
        private static string GenerateHTML(ComparisonResult result)
        {
            var builder = StringBuilderCache.Acquire();
            builder.Append("<table><thead><th><b>Role</b></th>");

            foreach (var userLogon in result.NamesToRolesMap.Keys)
            {
                builder.AppendFormat("<th><b>{0}</b></th>", userLogon);
            }
            builder.Append("</thead>");

            const string EqualityRowStyle = "style=\"background-color:#1aff1a:color:black\"";
            const string UnEqualityRowStyle = "style=\"background-color:#ff471a:color:black\"";
            const string HasRoleText = "Yes";
            const string NotHaveRoleText = "No";
            const string HasRoleStyle = "style=\"background-color:#99ddff\"";
            const string NotHaveRoleStyle = "style=\"background-color:#ffcc00\"";

            foreach (var role in result.CommonRoles)
            {
                builder.AppendFormat("<tr><td {0}>{1}</td>", EqualityRowStyle, role);

                foreach (var _ in result.NamesToRolesMap.Keys)
                {
                    builder.AppendFormat("<td {0}>{1}</td>", HasRoleStyle, HasRoleText);
                }
                builder.Append("</tr>");
            }

            foreach (var role in result.AllRoles.Except(result.CommonRoles))
            {
                builder.AppendFormat("<tr><td {0}>{1}</td>", UnEqualityRowStyle, role);

                foreach (var name in result.NamesToRolesMap.Keys)
                {
                    builder.AppendFormat("<td {0}>{1}</td>",
                        result.NamesToRolesMap[name].Contains(role) ? HasRoleStyle : NotHaveRoleStyle,
                        result.NamesToRolesMap[name].Contains(role) ? HasRoleText : NotHaveRoleText);
                }
                builder.Append("</tr>");
            }

            return builder.Append("</table>").ToString();
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