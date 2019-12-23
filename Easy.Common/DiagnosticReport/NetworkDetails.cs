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
    public sealed class NetworkDetails
    {
        /// <summary>
        /// Gets the <c>Dynamic Host Configuration Protocol</c> (DHCP) scope name.
        /// </summary>
        public string DHCPScope { get; internal set; }

        /// <summary>
        /// Gets the domain in which the local computer is registered.
        /// </summary>
        public string Domain { get; internal set; }

        /// <summary>
        /// Gets the host name for the local computer.
        /// </summary>
        public string Host { get; internal set; }

        /// <summary>
        /// Gets the flag indicating whether the local computer is acting as a 
        /// <c>Windows Internet Name Service</c> (WINS) proxy.
        /// </summary>
        public bool IsWINSProxy { get; internal set; }

        /// <summary>
        /// Gets the <c>Network Basic Input/Output System</c> (NetBIOS) node type of the local computer.
        /// </summary>
        public string NodeType { get; internal set; }

        /// <summary>
        /// Gets the details of the network interfaces in the local computer.
        /// </summary>
        public NetworkInterfaceDetails[] InterfaceDetails { get; internal set; }
    }

    /// <summary>
    /// Represents the details of the network interface in the local computer.
    /// </summary>
    public sealed class NetworkInterfaceDetails
    {
        /// <summary>
        /// Gets the physical address a.k.a. <c>Media Access Control</c> address of the interface.
        /// </summary>
        public string MAC { get; internal set; }

        /// <summary>
        /// Gets the network interface.
        /// </summary>
        public NetworkInterface Interface { get; internal set; }

        /// <summary>
        /// Gets all of the <c>IP</c> addresses currently assigned to the interface.
        /// </summary>
        public IPAddressDetails[] Addresses { get; internal set; }
    }

    /// <summary>
    /// Represents the details of the IP address
    /// </summary>
    public sealed class IPAddressDetails
    {
        /// <summary>
        /// Returns a <see cref="IPAddressDetails"/> from the given <paramref name="ipAddress"/>.
        /// </summary>
        internal static IPAddressDetails From(IPAddress ipAddress) => new IPAddressDetails
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
        public AddressFamily AddressFamily { get; private set; }

        /// <summary>
        /// Gets whether the address is an IPv6 multicast global address.
        /// </summary>
        public bool IsIPv6Multicast { get; private set; }

        /// <summary>
        /// Gets whether the address is an IPv6 link local address.
        /// </summary>
        public bool IsIPv6LinkLocal { get; private set; }

        /// <summary>
        /// Gets whether the address is an IPv6 site local address.
        /// </summary>
        public bool IsIPv6SiteLocal { get; private set; }

        /// <summary>
        /// Gets whether the address is an IPv6 Teredo address.
        /// </summary>
        public bool IsIPv6Teredo { get; private set; }

        /// <summary>
        /// Gets whether the IP address is an IPv4-mapped IPv6 address.
        /// </summary>
        public bool IsIPv4MappedToIPv6 { get; private set; }

        /// <summary>
        /// Gets the IP address as string in dotted-quad notation for IPv4 and in colon-hexadecimal notation for IPv6.
        /// </summary>
        public string AsString { get; private set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        public override string ToString() => AsString;
    }
}