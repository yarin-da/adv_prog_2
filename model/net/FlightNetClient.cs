using System;
using System.Net;
using System.Net.Sockets;

namespace Adv_Prog_2.model.net
{
    class FlightNetClient : INetClient
    {
        private Socket socket = null;

        public bool IsConnected { get { return socket != null && socket.Connected; } }

        public void Connect(int port, string server)
        {
            // setup the socket
            if (socket == null)
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            }
            // set port and server
            System.Net.IPAddress serverIP = System.Net.IPAddress.Parse(server);
            System.Net.IPEndPoint remoteEP = new IPEndPoint(serverIP, port);
            socket.Connect(remoteEP);
        }

        public void Disconnect()
        {
            if (socket != null)
            {
                socket.Close();
                // mark the socket as unusable
                socket = null;
            }
        }

        public void Send(string data)
        {
            try
            {
                // add a CRLF delimiter at the end of the data
                const string CRLF = "\r\n";
                byte[] byData = System.Text.Encoding.ASCII.GetBytes(data + CRLF);
                socket.Send(byData);
            }
            catch (Exception ex) { Console.WriteLine(ex); }
        }
    }
}
