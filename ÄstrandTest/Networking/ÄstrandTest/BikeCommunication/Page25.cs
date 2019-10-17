using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networking.ÄstrandTest.BikeCommunication
{
    class Page25 : Page
    {
        public byte updateEventCount;
        public byte instantaneousCadence;
        public byte accumulatedPowerLSB;
        public byte accumulatedPowerMSB;
        public byte instantaneousPowerLSB;
        public byte instantaneousPowerMSB;
        public byte trainerField;
        public byte bitFields;
        
        public Page25(byte updateEventCount, byte instantaneousCadence, byte accumulatedPowerMSB, byte instantaneousPowerMSB, byte trainerField, byte flagBitField, byte stateBitField) 
            : base(0x19)
        {
            this.updateEventCount = updateEventCount;
            this.instantaneousCadence = instantaneousCadence;
            this.accumulatedPowerLSB = ReverseByte(accumulatedPowerMSB);
            this.accumulatedPowerMSB = accumulatedPowerMSB;
            this.instantaneousPowerLSB = ReverseByte(instantaneousPowerMSB);
            this.instantaneousPowerMSB = instantaneousPowerMSB;
            this.trainerField = trainerField;
            this.bitFields = (byte)((flagBitField << 4) | stateBitField);
        }

        public Page25(byte updateEventCount, byte instantaneousCadence, byte accumulatedPowerMSB, byte instantaneousPowerMSB, byte trainerField, byte bitFields)
            : base(0x19)
        {
            this.updateEventCount = updateEventCount;
            this.instantaneousCadence = instantaneousCadence;
            this.accumulatedPowerLSB = ReverseByte(accumulatedPowerMSB);
            this.accumulatedPowerMSB = accumulatedPowerMSB;
            this.instantaneousPowerLSB = ReverseByte(instantaneousPowerMSB);
            this.instantaneousPowerMSB = instantaneousPowerMSB;
            this.trainerField = trainerField;
            this.bitFields = bitFields;
        }

        public override byte[] GetBytes()
        {
            return new byte[8]
            {
                this.PageID,
                this.updateEventCount,
                this.instantaneousCadence,
                this.accumulatedPowerLSB,
                this.accumulatedPowerMSB,
                (byte)((this.instantaneousPowerLSB << 4) | (this.instantaneousPowerMSB >> 2)),
                (byte)((this.instantaneousPowerMSB << 4) | this.trainerField),
                this.bitFields
            };
        }

        public override int GetLength()
        {
            return 9;
        }
    }
}
