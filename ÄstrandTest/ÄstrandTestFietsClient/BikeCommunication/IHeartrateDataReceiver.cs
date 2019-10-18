using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ÄstrandTestFietsClient.BikeCommunication
{
    public interface IHeartrateDataReceiver
    {
        void ReceiveHeartrateData(byte heartrate);
    }
}
