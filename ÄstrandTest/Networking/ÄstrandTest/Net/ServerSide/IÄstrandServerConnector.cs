using System;
using System.Collections.Generic;
using System.Text;

namespace Networking.ÄstrandTest.Net.ServerSide
{
    public interface IÄstrandServerConnector
    {
        void OnMessageReceived(Message message, UserAccount user);
        void OnUserConnected(UserAccount user);
        void OnUserDisconnected(UserAccount user);
    }
}
