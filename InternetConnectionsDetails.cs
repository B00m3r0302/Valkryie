using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;

namespace Valkryie
{
    public class InternetConnectionsDetails
    {
        public string Protocol { get; }
        public string LocalAddress { get; }
        public string ForeignAddress { get; }
        public int Port { get; }

        public InternetConnectionsDetails(
            string protocol,
            string localAddress,
            string foreignAddress,
            int port)
        {
            Protocol = protocol;
            LocalAddress = localAddress;
            ForeignAddress = foreignAddress;
            Port = port;
        }

        public static InternetConnectionsDetails[] GetActiveConnections()
        {
            var activeConnections = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections();

            var connectionDetails = new InternetConnectionsDetails[activeConnections.Length];

            for (int i = 0; i < activeConnections.Length; i++)
            {
                var connection = activeConnections[i];
                connectionDetails[i] = new InternetConnectionsDetails(
                    "TCP",
                    connection.LocalEndPoint.Address.ToString(),
                    connection.RemoteEndPoint.Address.ToString(),
                    connection.LocalEndPoint.Port
                    );
            }

            return connectionDetails;
        }
    }
}
