using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ÄstrandTestFietsClient.BikeCommunication
{
    public abstract class Bike
    {
        public IBikeDataReceiver bikeDataReceiver;

        public Bike(IBikeDataReceiver bikeDataReceiver)
        {
            this.bikeDataReceiver = bikeDataReceiver;
        }

        public abstract bool ToggleListening();
        public abstract bool StartListening();
        public abstract bool StopListening();

        public virtual void ReceivedData(byte[] data)
        {
            bikeDataReceiver?.ReceiveBikeData(data, this);
        }
    }
}
