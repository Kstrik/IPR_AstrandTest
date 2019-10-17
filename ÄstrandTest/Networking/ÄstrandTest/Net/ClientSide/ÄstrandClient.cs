using Networking.ClientSide;
using System;
using System.Collections.Generic;
using System.Text;

namespace Networking.ÄstrandTest.Net.ClientSide
{
    public class ÄstrandClient : IServerDataReceiver, IClientConnector
    {
        private Client client;

        private IÄstrandClientConnector connector;

        public ÄstrandClient(string ip, int port, IÄstrandClientConnector connector, ILogger logger) 
        {
            this.client = new Client(ip, port, this, this, logger);

            this.connector = connector;
        }

        public bool Connect()
        {
            return this.client.Connect();
        }

        public void Disconnect()
        {
            this.client.Disconnect();
        }

        public void OnDataReceived(byte[] data)
        {
            this.connector?.OnMessageReceived(Message.Parse(DataEncryptor.Decrypt(data, DataEncryptor.NetworkKey)));
        }

        public void OnDisconnect()
        {
            this.connector?.OnConnectionLost();
        }

        public void Transmit(Message message)
        {
            this.client.Transmit(DataEncryptor.Encrypt(message.GetBytes(), DataEncryptor.NetworkKey));
        }

        public void SetConnector(IÄstrandClientConnector connector)
        {
            this.connector = connector;
        }
    }
}
