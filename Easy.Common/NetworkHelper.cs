namespace Easy.Common
{
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;

    /// <summary>
    /// Provides a set of methods to help work with network related activities.
    /// </summary>
    public static class NetworkHelper
    {
        /// <summary>
        /// Returns the LocalHost Fully Qualified Domain Name
        /// <see href="http://stackoverflow.com/questions/804700/how-to-find-fqdn-of-local-machine-in-c-net"/>
        /// </summary>
        /// <returns>The localhost Fully Qualified Domain Name</returns>
        public static string GetFQDN()
        {
            var domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;
            var hostName = Dns.GetHostName();

            domainName = "." + domainName;
            if (!hostName.EndsWith(domainName))
            {
                hostName += domainName;
            }

            return hostName;
        }

        /// <summary>
        /// Gets the local IP address for the machine or VM running the code.
        /// <remarks>
        /// <see href="http://stackoverflow.com/a/27376368"/>
        /// </remarks>
        /// </summary>
        /// <returns>The local IP address</returns>
        public static IPAddress GetLocalIpAddress()
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("10.0.2.4", 65530);
                var endPoint = socket.LocalEndPoint as IPEndPoint;
                // ReSharper disable once PossibleNullReferenceException
                return IPAddress.Parse(endPoint.Address.ToString());
            }
        }
    }
}