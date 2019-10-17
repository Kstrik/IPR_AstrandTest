using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networking.ÄstrandTest.BikeCommunication
{
    public abstract class Page
    {
        public byte pageID;

        public Page(byte pageID)
        {
            this.pageID = pageID;
        }

        public abstract byte[] GetBytes();

        public abstract int GetLength();

        public static byte ReverseByte(byte inByte)
        {
            byte result = 0x00;

            for (byte mask = 0x80; Convert.ToInt32(mask) > 0; mask >>= 1)
            {
                result = (byte)(result >> 1);

                var tempbyte = (byte)(inByte & mask);
                if (tempbyte != 0x00)
                {
                    result = (byte)(result | 0x80);
                }
            }

            return (result);
        }

        public abstract Page SimulateNewPage(double variance, Random random);

        public override string ToString()
        {
            return BitConverter.ToString(GetBytes()).Replace("-", " ");
        }

        public static byte RandomWithVariance(Random random, byte value, double variance)
        {
            double valueWithPrc = ((value / 100) * variance);
            return (byte)random.Next((int)(value - valueWithPrc), (int)(value + valueWithPrc));
        }
    }
}
