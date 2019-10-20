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
        public int Distance { get; set; }
        public int Cadence { get; set; }

        public int Speed { get; set; }
        public int Heartbeat { get; set; }

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
                bytes.Add((byte)Heartbeat);
            }
            if (HasPage16)
            {
                bytes.Add((byte)Message.ValueId.SPEED);
                bytes.Add((byte)Speed);
                bytes.Add((byte)Message.ValueId.DISTANCE);
                string distance = this.Distance.ToString();
                bytes.Add((byte)distance.Length);
                bytes.AddRange(Encoding.UTF8.GetBytes(distance));
            }
            if (HasPage25)
            {
                bytes.Add((byte)Message.ValueId.CYCLE_RHYTHM);
                bytes.Add((byte)Cadence);
            }
            return bytes.ToArray();
        }
    }
}
