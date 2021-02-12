// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Easy.Common
{
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;

    /// <summary>
    /// Represents the details of the network available in the local computer.
    /// </summary>
    public sealed record NetworkDetails
    {
        /// <summary>
        /// Gets the <c>Dynamic Host Configuration Protocol</c> (DHCP) scope name.
        /// </summary>
        public string DHCPScope { get; init; }

        /// <summary>
        /// Gets the domain in which the local computer is registered.
        /// </summary>
        public string Domain { get; init; }

        /// <summary>
        /// Gets the host name for the local computer.
        /// </summary>
        public string Host { get; init; }

        /// <summary>
        /// Gets the flag indicating whether the local computer is acting as a 
        /// <c>Windows Internet Name Service</c> (WINS) proxy.
        /// </summary>
        public bool IsWINSProxy { get; init; }

        /// <summary>
        /// Gets the <c>Network Basic Input/Output System</c> (NetBIOS) node type of the local computer.
        /// </summary>
        public string NodeType { get; init; }

        /// <summary>
        /// Gets the details of the network interfaces in the local computer.
        /// </summary>
        public NetworkInterfaceDetails[] InterfaceDetails { get; init; }
    }

    /// <summary>
    /// Represents the details of the network interface in the local computer.
    /// </summary>
    public sealed record NetworkInterfaceDetails
    {
        /// <summary>
        /// Gets the physical address a.k.a. <c>Media Access Control</c> address of the interface.
        /// </summary>
        public string MAC { get; init; }

        /// <summary>
        /// Gets the network interface.
        /// </summary>
        public NetworkInterface Interface { get; init; }

        /// <summary>
        /// Gets all of the <c>IP</c> addresses currently assigned to the interface.
        /// </summary>
        public IPAddressDetails[] Addresses { get; init; }
    }

    /// <summary>
    /// Represents the details of the IP address
    /// </summary>
    public sealed record IPAddressDetails
    {
        /// <summary>
        /// Returns a <see cref="IPAddressDetails"/> from the given <paramref name="ipAddress"/>.
        /// </summary>
        internal static IPAddressDetails From(IPAddress ipAddress) => new()
        {
            AddressFamily = ipAddress.AddressFamily,
            IsIPv6Multicast = ipAddress.IsIPv6Multicast,
            IsIPv6LinkLocal = ipAddress.IsIPv6LinkLocal,
            IsIPv6SiteLocal = ipAddress.IsIPv6SiteLocal,
            IsIPv6Teredo = ipAddress.IsIPv6Teredo,
            IsIPv4MappedToIPv6 = ipAddress.IsIPv4MappedToIPv6,
            AsString = ipAddress.ToString()
        };

        /// <summary>
        /// Gets the address family of the IP address.
        /// </summary>
        public AddressFamily AddressFamily { get; init; }

        /// <summary>
        /// Gets whether the address is an IPv6 multicast global address.
        /// </summary>
        public bool IsIPv6Multicast { get; init; }

        /// <summary>
        /// Gets whether the address is an IPv6 link local address.
        /// </summary>
        public bool IsIPv6LinkLocal { get; init; }

        /// <summary>
        /// Gets whether the address is an IPv6 site local address.
        /// </summary>
        public bool IsIPv6SiteLocal { get; init; }

        /// <summary>
        /// Gets whether the address is an IPv6 Teredo address.
        /// </summary>
        public bool IsIPv6Teredo { get; init; }

        /// <summary>
        /// Gets whether the IP address is an IPv4-mapped IPv6 address.
        /// </summary>
        public bool IsIPv4MappedToIPv6 { get; init; }

        /// <summary>
        /// Gets the IP address as string in dotted-quad notation for IPv4 and in colon-hexadecimal notation for IPv6.
        /// </summary>
        public string AsString { get; init; }
    }
}