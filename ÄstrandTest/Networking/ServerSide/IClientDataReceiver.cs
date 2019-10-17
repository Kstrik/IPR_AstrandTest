using System;
using System.Collections.Generic;
using System.Text;

namespace Networking.ServerSide
{
    public interface IClientDataReceiver
    {
        void OnDataReceived(byte[] data, ClientConnection connection);
    }
}
