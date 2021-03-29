using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Adv_Prog_2
{
    class FlightNetClient : INetClient
    {
        private Socket socket = null;

        public bool IsConnected { 
            get { return socket != null && socket.Connected; }
        }

        public void Connect(int port, string server)
        {
            // setup the socket
            if (socket == null)
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            // set port and server
            System.Net.IPAddress serverIP = System.Net.IPAddress.Parse(server);
            System.Net.IPEndPoint remoteEP = new IPEndPoint(serverIP, port);
            try
            {
                socket.Connect(remoteEP);
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
            }
        }

        public void Disconnect()
        {
            socket.Close();
        }

        public void Send(string data)
        {
            // add a CRLF delimiter at the end of the data
            byte[] byData = System.Text.Encoding.ASCII.GetBytes(data + "\r\n");
            socket.Send(byData);
        }
    }
}
