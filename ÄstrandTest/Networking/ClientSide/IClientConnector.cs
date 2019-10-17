using System;
using System.Collections.Generic;
using System.Text;

namespace Networking.ClientSide
{
    public interface IClientConnector
    {
        void OnDisconnect();
    }
}
