using System;
using System.Collections.Generic;
using System.Management;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace KeysLibrary.Services
{
    public class KeysServices : IKeysServices
    {
        public string GeneratePublicKey()
        {
            try
            {
                var cpu_id = GetCPUId();
                var mother_board_Id = GetMotherBoardId();
                return $"{cpu_id}*{mother_board_Id}";
            }
            catch(Exception ex)
            {
                throw new Exception("");
            }
        }

        private string GetMotherBoardId()
        {
            string mbInfo = String.Empty;
            ManagementScope scope = new ManagementScope("\\\\" + Environment.MachineName + "\\root\\cimv2");
            scope.Connect();
            ManagementObject wmiClass = new ManagementObject(scope, new ManagementPath("Win32_BaseBoard.Tag=\"Base Board\""), new ObjectGetOptions());

            foreach (PropertyData propData in wmiClass.Properties)
            {
                if (propData.Name == "SerialNumber")
                    return propData.Value.ToString();
            }

            return null;
        }

        private string GetCPUId()
        {
            ManagementClass managClass = new ManagementClass("win32_processor");
            ManagementObjectCollection managCollec = managClass.GetInstances();
            string cpu_id = string.Empty;

            foreach (ManagementObject managObj in managCollec)
            {
                cpu_id = managObj.Properties["processorID"].Value.ToString();
                break;
            }

            return cpu_id;
        }

        public string GenerateSerialKey()
        {
            var date_utc_ntp = GetNetworkTime();
            var date_utc = DateTime.UtcNow;
            return string.Empty;
        }

        public static DateTime GetNetworkTime()
        {
            //thanks to the user who answered the question on stackoverflow
            //https://stackoverflow.com/questions/1193955/how-to-query-an-ntp-server-using-c

            //default Windows time server
            const string ntpServer = "time.windows.com";

            // NTP message size - 16 bytes of the digest (RFC 2030)
            var ntpData = new byte[48];

            //Setting the Leap Indicator, Version Number and Mode values
            ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

            var addresses = Dns.GetHostEntry(ntpServer).AddressList;

            //The UDP port number assigned to NTP is 123
            var ipEndPoint = new IPEndPoint(addresses[0], 123);
            //NTP uses UDP

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Connect(ipEndPoint);

                //Stops code hang if NTP is blocked
                socket.ReceiveTimeout = 3000;

                socket.Send(ntpData);
                socket.Receive(ntpData);
                socket.Close();
            }

            //Offset to get to the "Transmit Timestamp" field (time at which the reply 
            //departed the server for the client, in 64-bit timestamp format."
            const byte serverReplyTime = 40;

            //Get the seconds part
            ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);

            //Get the seconds fraction
            ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

            //Convert From big-endian to little-endian
            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);

            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

            //**UTC** time
            var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);

            return networkDateTime;
        }

        // stackoverflow.com/a/3294698/162671
        static uint SwapEndianness(ulong x)
        {
            return (uint)(((x & 0x000000ff) << 24) +
                           ((x & 0x0000ff00) << 8) +
                           ((x & 0x00ff0000) >> 8) +
                           ((x & 0xff000000) >> 24));
        }
    }
}
