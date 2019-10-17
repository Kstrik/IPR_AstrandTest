using System;
using System.Collections.Generic;
using System.Text;

namespace Networking.ClientSide
{
    public interface IServerDataReceiver
    {
        void OnDataReceived(byte[] data);
    }
}
