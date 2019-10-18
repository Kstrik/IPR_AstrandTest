using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ÄstrandTestFietsClient.BikeCommunication.Temp
{
    public abstract class Page
    {
        public enum PageType
        {
            PAGE16 = 16,
            PAGE25 = 25
        }

        public byte PageID;

        public Page(byte pageID)
        {
            this.PageID = pageID;
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

        public override string ToString()
        {
            return BitConverter.ToString(GetBytes()).Replace("-", " ");
        }
    }
}
