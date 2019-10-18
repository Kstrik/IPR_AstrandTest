using Avans.TI.BLE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ÄstrandTestFietsClient.BikeCommunication
{
    public class HeartrateMonitor
    {
        public IHeartrateDataReceiver heartrateDataReceiver;

        public HeartrateMonitor(IHeartrateDataReceiver heartrateDataReceiver)
        {
            this.heartrateDataReceiver = heartrateDataReceiver;
            //Thread HRMThread = new Thread(StartHeartrateMonitor);
            //HRMThread.Start();
            Task.Run(() => StartHeartrateMonitor());
        }

        private async Task StartHeartrateMonitor()
        {
            Console.WriteLine("Connecting to HeartrateMonitor");
            int errorCode = 0;
            BLE BlEheart = new BLE();
            errorCode = await BlEheart.OpenDevice("Decathlon Dual HR");
            Console.WriteLine("Waiting for Heartrate Services");
            await BlEheart.SetService("HeartRate");
            Console.WriteLine("Connected to Heartrate service");
            BlEheart.SubscriptionValueChanged += BleBike_SubscriptionValueChanged;
            await BlEheart.SubscribeToCharacteristic("HeartRateMeasurement");
            Console.WriteLine("Subscribed to Heartrate Measurement");
            Console.WriteLine("Errorcode: " + errorCode); // 1 = no heartbeat detected.
        }

        private void BleBike_SubscriptionValueChanged(object sender, BLESubscriptionValueChangedEventArgs e)
        {
            byte[] data = e.Data;
            byte heartbeat = data[1];
            Console.WriteLine("Heartbeat: {0}", heartbeat);
            this.heartrateDataReceiver.ReceiveHeartrateData(heartbeat);
        }
    }
}
