using Networking.ÄstrandTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ÄstrandTestFietsClient.BikeCommunication
{
    public struct ClientMessage
    {
        public byte Distance { get; set; }
        public byte Cadence { get; set; }

        public byte Speed { get; set; }
        public byte Heartbeat { get; set; }

        public byte CheckBits { get; set; }

        public Boolean HasHeartbeat;
        public Boolean HasPage16;
        public Boolean HasPage25;

        public byte[] GetData()
        {
            List<byte> bytes = new List<byte>();
            if (HasHeartbeat)
            {
                bytes.Add((byte)Message.ValueId.HEARTRATE);
                bytes.Add(Heartbeat);
            }
            if (HasPage16)
            {
                bytes.Add((byte)Message.ValueId.SPEED);
                bytes.Add(Speed);
                bytes.Add((byte)Message.ValueId.DISTANCE);
                bytes.Add(Distance);
            }
            if (HasPage25)
            {
                bytes.Add((byte)Message.ValueId.CYCLE_RHYTHM);
                bytes.Add(Cadence);
            }
            return bytes.ToArray();
        }
    }
}
