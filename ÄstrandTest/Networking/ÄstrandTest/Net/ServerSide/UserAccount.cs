using Networking.ServerSide;
using System;
using System.Collections.Generic;
using System.Text;

namespace Networking.ÄstrandTest.Net.ServerSide
{
    public class UserAccount
    {
        public ClientConnection Connection;

        public string Username;

        public bool IsAuthorized;

        public UserAccount(ClientConnection connection)
        {
            this.Connection = connection;
        }
    }
}
