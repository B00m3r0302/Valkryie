using System;
using System.Management;

namespace Valkryie
{
    internal class ConnectionRemediation
    {
        public static void DisconnectUnknownConnections()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_NetworkConnection");

                foreach (ManagementObject queryObj in searcher.Get())
                {
                    string remoteAddress = queryObj["RemoteAddress"] as string;
                    string localAddress = queryObj["LocalAddress"] as string;

                    if (remoteAddress != null && !remoteAddress.Equals("127.0.0.1"))
                    {
                        string connectionId = queryObj["ConnectionID"] as string;

                        if (!string.IsNullOrEmpty(connectionId))
                        {
                            ManagementObject networkConnection = new ManagementObject("root\\CIMV2", $"Win32_NetworkConnection.ConnectionID='{connectionId}'", null);
                            networkConnection.InvokeMethod("Disconnect", null);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}