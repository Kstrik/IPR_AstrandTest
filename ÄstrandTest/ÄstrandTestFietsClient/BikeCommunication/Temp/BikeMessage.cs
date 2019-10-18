using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ÄstrandTestFietsClient.BikeCommunication.Temp
{
    public class BikeMessage
    {
        public byte sync;
        public byte length;
        public byte type;
        public byte channel;
        public Page data;
        public byte checksum;

        public BikeMessage(Page page)
        {
            this.sync = 0xA4;
            this.length = (byte)page.GetLength();
            this.type = 0x4E;
            this.channel = 0x05;
            this.data = page;
            CalculateChecksum();
        }

        private void CalculateChecksum()
        {
            this.checksum = 0x00;
            byte[] bytes = GetBytes();

            for(int i = 0; i < bytes.Length - 1; i++)
            {
                this.checksum ^= bytes[i];
            }
        }

        public byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>
            {
                this.sync,
                this.length,
                this.type,
                this.channel,
            };
            bytes.AddRange(this.data.GetBytes());
            bytes.Add(this.checksum);

            return bytes.ToArray();
        }
    }
}
