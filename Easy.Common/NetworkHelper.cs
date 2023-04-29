﻿namespace Easy.Common;

using System;
using System.Collections.Generic;
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
    /// <returns>The <c>localhost</c> Fully Qualified Domain Name</returns>
    public static string GetFQDN()
    {
        string domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;
        string hostName = Dns.GetHostName();

        domainName = "." + domainName;
        if (!hostName.EndsWith(domainName, StringComparison.InvariantCultureIgnoreCase))
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
    public static IPAddress GetLocalIPAddress()
    {
        using Socket socket = new(AddressFamily.InterNetwork, SocketType.Dgram, 0);
            
        try
        {
            // can be any address
            socket.Connect("10.0.2.4", 65530);
            IPEndPoint? endPoint = socket.LocalEndPoint as IPEndPoint;
            return IPAddress.Parse(endPoint!.Address.ToString());
        } catch (SocketException)
        {
            return IPAddress.Parse("127.0.0.1");
        }
    }

    /// <summary>
    /// Gets all the IP (v4 and not v6) addresses of the local computer together with 
    /// the interface to which the IP belongs.
    /// <see href="https://blog.stephencleary.com/2009/05/getting-local-ip-addresses.html"/>
    /// </summary>
    public static IDictionary<IPAddress, string> GetLocalIPAddresses()
    {
        // Get a list of all network interfaces (usually one per network card, dial-up, and VPN connection)
        NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

        Dictionary<IPAddress, string> result = new();
        foreach (NetworkInterface nic in nics)
        {
            foreach (IPAddress item in GetLocalIPAddresses(nic))
            {
                result.Add(item, nic.Name);
            }
        }
        return result;
    }

    internal static IEnumerable<IPAddress> GetLocalIPAddresses(NetworkInterface nic)
    {
        // Read the IP configuration for each network
        IPInterfaceProperties properties = nic.GetIPProperties();

        // Each network interface may have multiple IP addresses
        foreach (UnicastIPAddressInformation address in properties.UnicastAddresses)
        {
            // We're only interested in IPv4 addresses for now
            if (address.Address.AddressFamily != AddressFamily.InterNetwork) { continue; }

            // Ignore loopback addresses (e.g., 127.0.0.1)
            if (IPAddress.IsLoopback(address.Address)) { continue; }

            yield return address.Address;
        }
    }
}