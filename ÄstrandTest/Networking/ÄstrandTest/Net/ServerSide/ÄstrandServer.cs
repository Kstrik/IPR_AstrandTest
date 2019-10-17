using Networking.ServerSide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Networking.ÄstrandTest.Net.ServerSide
{
    public class ÄstrandServer : IClientDataReceiver, IServerConnector
    {
        private Server server;
        public List<UserAccount> ConnectedUsers;

        private IÄstrandServerConnector connector;

        public ÄstrandServer(string ip, int port, IÄstrandServerConnector connector, ILogger logger) 
        {
            this.server = new Server(ip, port, this, this, logger);
            this.ConnectedUsers = new List<UserAccount>();

            this.connector = connector;
        }

        public bool Start()
        {
            return this.server.Start();
        }

        public void Stop()
        {
            this.server.Stop();
        }

        public void Transmit(Message message, UserAccount user)
        {
            this.server.Transmit(DataEncryptor.Encrypt(message.GetBytes(), DataEncryptor.NetworkKey), user.Connection);
        }

        public void Broadcast(Message message)
        {
            this.server.Broadcast(message.GetBytes());
        }

        public void OnDataReceived(byte[] data, ClientConnection connection)
        {
            UserAccount userAccount = GetUser(connection);
            this.connector?.OnMessageReceived(Message.Parse(DataEncryptor.Decrypt(data, DataEncryptor.NetworkKey)), userAccount);
        }

        public void OnClientConnected(ClientConnection connection)
        {
            UserAccount userAccount = new UserAccount(connection);
            this.ConnectedUsers.Add(userAccount);
            this.connector?.OnUserConnected(userAccount);
        }

        public void OnClientDisconnected(ClientConnection connection)
        {
            UserAccount userAccount = GetUser(connection);
            this.ConnectedUsers.Remove(userAccount);
            this.connector?.OnUserDisconnected(userAccount);
        }

        private UserAccount GetUser(ClientConnection connection)
        {
            return this.ConnectedUsers.Where(u => u.Connection == connection)?.First();
        }
    }
}
