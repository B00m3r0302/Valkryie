using System;
using System.Management;
using System.Runtime.InteropServices;
using System.Net;
using System.Data.Entity.Migrations.Infrastructure;

namespace Valkryie
{
    internal class ConnectionRemediation
    {

        [DllImport("iphlpapi.dll", SetLastError = true)]
        private static extern int SetTcpEntry(ref MIB_TCPROW row);

        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_TCPROW
        {
            public int dwState;
            public int dwLocalAddr;
            public int dwLocalPort;
            public int dwRemoteAddr;
            public int dwRemotePort;
        }
        public static void DisconnectUnknownConnections()
        {
            try
            {
                IPAddress localIpAddress = IPAddress.Parse("127.0.0.1"); // Localhost

                // get a list of all active TCP connections 
                MIB_TCPROW[] tcpRows = GetActiveTcpConnections();

                foreach (var row in tcpRows)
                {
                    IPAddress remoteIpAddress = new IPAddress(row.dwRemoteAddr);

                    // Check if the remote IP address is not the loopback address
                    if (!remoteIpAddress.Equals(localIpAddress))
                    {
                        // Create a copy of the row to use as a ref paramater otherwise the value of row within SetTcpEntry won't work
                        MIB_TCPROW rowCopy = row;
                        // Terminate the connection
                        SetTcpEntry(ref rowCopy);
                    }
                }

                Console.WriteLine("Disconnected non-local network connections")
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        [DllImport("iphlpapi.dll", SetLastError =true)]
        private static extern int GetTcpTable(IntPtr pTcpTable, ref int pdwSize, bool bOrder);

        public static MIB_TCPROW[] GetActiveTcpConnections()
        {
            int bufferSize = 0;
            int result = GetTcpTable(IntPtr.Zero, ref bufferSize, false);

            IntPtr tcpTablePtr = Marshal.AllocHGlobal(bufferSize);

            try
            {
                result = GetTcpTable(tcpTablePtr, ref bufferSize, false);

                if (result != 0)
                {
                    throw new Exception("Failed to retrieve TCP table");
                }

                int numEntries = Marshal.ReadInt32(tcpTablePtr);
                MIB_TCPROW[] tcpRows = new MIB_TCPROW[numEntries];

                IntPtr currentPtr = (IntPtr)(tcpTablePtr.ToInt64() + 4); // Skip the first 4 bytes

                for (int i = 0; i < numEntries; i++)
                {
                    tcpRows[i] = (MIB_TCPROW)Marshal.PtrToStructure(currentPtr, typeof(MIB_TCPROW));
                    currentPtr = (IntPtr)(currentPtr.ToInt64() + Marshal.SizeOf(typeof(MIB_TCPROW)));
                }

                return tcpRows;
            }

            finally
            {
                   Marshal.FreeHGlobal(tcpTablePtr);
            }
        }
    }
}