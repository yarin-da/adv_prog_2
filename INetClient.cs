﻿namespace Adv_Prog_2
{
    interface INetClient
    {
        public bool IsConnected { get; }
        public void Connect(int port, string server);
        public void Disconnect();
        public void Send(string data);
    }
}
