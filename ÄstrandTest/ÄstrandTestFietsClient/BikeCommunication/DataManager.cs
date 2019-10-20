using Networking.ÄstrandTest;
using Networking.ÄstrandTest.Net.ClientSide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ÄstrandTestFietsClient.BikeCommunication
{
    public class DataManager : IBikeDataReceiver, IHeartrateDataReceiver, IClientMessageReceiver
    {
        private ClientMessage clientMessage;
        private ÄstrandClient astrandClient;

        private HeartrateMonitor heartrateMonitor;

        private IClientMessageReceiver receiver;

        public DataManager(ÄstrandClient astrandClient, IClientMessageReceiver receiver) //current observer is datamanager itself, rather than the client window
        {
            this.clientMessage = new ClientMessage();

            this.astrandClient = astrandClient;
            this.heartrateMonitor = new HeartrateMonitor(this);

            this.receiver = receiver;
        }

        public void AddPage25(int cadence)
        {
            if (this.clientMessage.HasPage25)
                PushMessage();

            this.clientMessage.Cadence = (byte)cadence;
            this.clientMessage.HasPage25 = true;
        }

        public void AddPage16(int speed, int distance)
        {
            if (this.clientMessage.HasPage16)
                PushMessage();

            this.clientMessage.Distance = distance;
            this.clientMessage.Speed = (byte)speed;
            this.clientMessage.HasPage16 = true;
        }

        public void AddHeartbeat(byte heartbeat)
        {
            if (this.clientMessage.HasHeartbeat)
                PushMessage();
            this.clientMessage.Heartbeat = heartbeat;
            this.clientMessage.HasHeartbeat = true;
        }

        private void PushMessage()
        {
            this.receiver?.HandleClientMessage(this.clientMessage);
            //HandleClientMessage(this.clientMessage);
            this.clientMessage = new ClientMessage();
            this.clientMessage.HasHeartbeat = false;
            this.clientMessage.HasPage16 = false;
            this.clientMessage.HasPage25 = false;
        }

        //Upon receiving data from the bike and Heartbeat Sensor, try to place in a Struct. 
        //Once struct is full or data would be overwritten, it is sent to the server
        public void ReceiveBikeData(byte[] data, Bike bike)
        {
            Dictionary<string, int> translatedData = TacxTranslator.Translate(BitConverter.ToString(data).Split('-'));
            int PageID;
            translatedData.TryGetValue("PageID", out PageID); //hier moet ik van overgeven maar het kan niet anders
            if (25 == PageID)
            {
                int cadence;
                translatedData.TryGetValue("InstantaneousCadence", out cadence);
                AddPage25(cadence);
            }
            else if (16 == PageID)
            {
                int speed;
                translatedData.TryGetValue("speed", out speed);
                int distance;
                translatedData.TryGetValue("distance", out distance);
                AddPage16(speed, distance);
            }
        }

        /// <summary>
        /// Parses a complete ClientMessage into a packet to be sent via TCP
        /// </summary>
        public void HandleClientMessage(ClientMessage clientMessage)
        {
            Message toSend = new Message(Message.ID.BIKEDATA, Message.State.NONE, clientMessage.GetData());
            this.astrandClient.Transmit(toSend);
        }

        public void ReceiveHeartrateData(byte heartrate)
        {
            AddHeartbeat(heartrate);
        }
    }
}
