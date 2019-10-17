using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networking.ÄstrandTest.BikeCommunication
{
    class Page16 : Page
    {
        public byte equipmentTypeBitField;
        public byte elapsedTime;
        public byte distanceTraveled;
        public byte speedLSB;
        public byte speedMSB;
        public byte heartrate;
        public byte capabillitiesBitField;
        public byte feStateBitField;
        public byte bitFields; 

        public Page16(byte equipmentTypeBitField, byte elapsedTime, byte distanceTraveled, byte speedMSB, byte heartrate, byte capabillitiesBitField, byte feStateBitField) 
            : base(0x10)
        {
            this.equipmentTypeBitField = equipmentTypeBitField;
            this.elapsedTime = elapsedTime;
            this.distanceTraveled = distanceTraveled;
            this.speedLSB = ReverseByte(speedMSB);
            this.speedMSB = speedMSB;
            this.heartrate = heartrate;
            this.capabillitiesBitField = capabillitiesBitField;
            this.feStateBitField = feStateBitField;
            this.bitFields = (byte)((capabillitiesBitField << 4) | feStateBitField);
        }

        public Page16(byte equipmentTypeBitField, byte elapsedTime, byte distanceTraveled, byte speedMSB, byte heartrate, byte bitFields)
            : base(0x10)
        {
            this.equipmentTypeBitField = equipmentTypeBitField;
            this.elapsedTime = elapsedTime;
            this.distanceTraveled = distanceTraveled;
            this.speedLSB = ReverseByte(speedMSB);
            this.speedMSB = speedMSB;
            this.heartrate = heartrate;
            this.capabillitiesBitField = (byte)(bitFields >> 4);
            this.feStateBitField = (byte)(bitFields & 15);
            this.bitFields = bitFields;
        }

        public override byte[] GetBytes()
        {
            return new byte[8] {
                this.pageID,
                this.equipmentTypeBitField,
                this.elapsedTime,
                this.distanceTraveled,
                this.speedLSB,
                this.speedMSB,
                this.heartrate,
                this.bitFields
            };
        }

        public override int GetLength()
        {
            return 9;
        }

        public override Page SimulateNewPage(double variance, Random random)
        {
            Page16 page = new Page16(this.equipmentTypeBitField, 
                                    RandomWithVariance(random, this.elapsedTime, variance),
                                    (byte)(this.distanceTraveled + variance), 
                                    RandomWithVariance(random, this.speedMSB, variance), 
                                    RandomWithVariance(random, this.heartrate, variance), 
                                    this.capabillitiesBitField, this.feStateBitField);
            return page;
        }
    }
}
