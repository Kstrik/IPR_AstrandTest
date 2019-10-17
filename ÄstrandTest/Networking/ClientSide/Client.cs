using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Networking.ClientSide
{
    public class Client
    {
        private bool isReady;
        private bool isConnected;

        private IPAddress host;
        private int port;

        private TcpClient client;
        private NetworkStream stream;

        private IServerDataReceiver receiver;
        private IClientConnector connector;
        private ILogger logger;

        private Thread listenerThread;

        public Client(string ip, int port, IServerDataReceiver receiver, IClientConnector connector, ILogger logger)
        {
            this.isReady = IPAddress.TryParse(ip, out host);
            this.isConnected = false;

            this.port = port;
            this.receiver = receiver;
            this.connector = connector;
            this.logger = logger;
        }

        private void InitilizeListenerThread()
        {
            this.listenerThread = new Thread(() =>
            {
                while (this.isConnected)
                {
                    byte[] bytes = Receive();
                    if (bytes.Length == 0)
                    {
                        Disconnect();
                        this.connector?.OnDisconnect();
                    }
                    else
                        this.receiver?.OnDataReceived(bytes);
                }
            });
        }

        public bool Connect()
        {
            try
            {
                if (this.isReady && !this.isConnected)
                {
                    this.isConnected = true;
                    this.client = new TcpClient(this.host.ToString(), this.port);
                    this.stream = this.client.GetStream();

                    InitilizeListenerThread();
                    this.listenerThread.Start();

                    this.logger?.Log($"Client connected to {this.host} on port {this.port}\n");
                    return true;
                }
                else if (!this.isReady)
                {
                    this.logger?.Log("Client could not connect due to invalid ip!\n");
                }
                else
                {
                    this.logger?.Log("Client is already connected!\n");
                }
            }
            catch (Exception e) { }
            return false;
        }

        public void Disconnect()
        {
            if (this.isConnected)
            {
                this.isConnected = false;
                this.client.Close();
                this.stream.Close();
                this.logger?.Log($"Client disconnected on {this.host} using port {this.port}\n");
            }
        }

        public void Transmit(byte[] data)
        {
            if (data != null && this.isConnected)
            {
                byte[] sizeinfo = new byte[4];

                sizeinfo[0] = (byte)data.Length;
                sizeinfo[1] = (byte)(data.Length >> 8);
                sizeinfo[2] = (byte)(data.Length >> 16);
                sizeinfo[3] = (byte)(data.Length >> 24);

                this.stream.Write(sizeinfo, 0, sizeinfo.Length);
                this.stream.Write(data, 0, data.Length);
            }
        }

        public byte[] Receive()
        {
            try
            {
                byte[] sizeinfo = new byte[4];
                int totalread = 0, currentread = 0;

                currentread = totalread = this.stream.Read(sizeinfo, 0, sizeinfo.Length);

                while (totalread < sizeinfo.Length && currentread > 0)
                {
                    currentread = this.stream.Read(sizeinfo, totalread, sizeinfo.Length - totalread);
                    totalread += currentread;
                }

                int messagesize = 0;
                messagesize |= sizeinfo[0];
                messagesize |= (((int)sizeinfo[1]) << 8);
                messagesize |= (((int)sizeinfo[2]) << 16);
                messagesize |= (((int)sizeinfo[3]) << 24);

                byte[] buffer = new byte[messagesize];
                totalread = 0;

                currentread = totalread = this.stream.Read(buffer, totalread, buffer.Length - totalread);

                while (totalread < messagesize && currentread > 0)
                {
                    currentread = this.stream.Read(buffer, totalread, buffer.Length - totalread);
                    totalread += currentread;
                }

                if (this.logger != null)
                {
                    string responseData = Encoding.UTF8.GetString(buffer, 0, totalread);
                    if (responseData.Length != 0)
                        this.logger.Log($"Received: {responseData}\n");
                }

                return buffer;
            }
            catch (Exception e)
            {
                return new byte[0];
            }
        }
    }
}
