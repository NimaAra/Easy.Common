// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Easy.Common
{
    using System.Net;
    using System.Net.NetworkInformation;

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
        public IPAddress[] Addresses { get; internal set; }
    }
}