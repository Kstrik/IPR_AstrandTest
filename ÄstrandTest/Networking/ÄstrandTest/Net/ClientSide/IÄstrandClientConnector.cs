using System;
using System.Collections.Generic;
using System.Text;

namespace Networking.ÄstrandTest.Net.ClientSide
{
    public interface IÄstrandClientConnector
    {
        void OnMessageReceived(Message message);
        void OnConnectionLost();
    }
}
