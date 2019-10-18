using Avans.TI.BLE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ÄstrandTestFietsClient.BikeCommunication
{
    public class RealBike : Bike
    {
        public string ModelNumber;
        private BLE bleBike;

        public RealBike(string ModelNumber, IBikeDataReceiver bikeDataReceiver)
            : base(bikeDataReceiver)
        {
            this.ModelNumber = ModelNumber;
            //ThreadStart bikeStart = new ThreadStart(ConnectToBike);
            //Thread bikeThread = new Thread(bikeStart);
            //bikeThread.Start();
            Task.Run(() => ConnectToBike());
        }

        private async Task ConnectToBike()
        {
            Console.WriteLine("Starting connection to bike");
            int errorCode = 0;
            this.bleBike = new BLE();
            Thread.Sleep(1000); // We need some time to list available devices

            // Connecting
            errorCode = await this.bleBike.OpenDevice("Tacx Flux " + ModelNumber);

            // Set service
            errorCode = await this.bleBike.SetService("6e40fec1-b5a3-f393-e0a9-e50e24dcca9e");

            // Subscribe
            bleBike.SubscriptionValueChanged += BleBike_SubscriptionValueChanged;
            errorCode = await this.bleBike.SubscribeToCharacteristic("6e40fec2-b5a3-f393-e0a9-e50e24dcca9e");
        }

        private void BleBike_SubscriptionValueChanged(object sender, BLESubscriptionValueChangedEventArgs e)
        {
            ReceivedData(e.Data);
        }

        public void SetResistence(byte resistance)
        {
            resistance *= 2;
            byte[] output = new byte[13];
            output[0] = 0x4A; // Sync bit;
            output[1] = 0x09; // Message Length
            output[2] = 0x4E; // Message type
            output[3] = 0x05; // Message type
            output[4] = 0x30; // Data Type
            output[5] = 0xFF; // Data Type
            output[6] = 0xFF; // Data Type
            output[7] = 0xFF; // Data Type
            output[8] = 0xFF; // Data Type
            output[9] = 0xFF; // Data Type
            output[10] = 0xFF; // Data Type
            output[11] = resistance;//0-200%

            output[12] = output[0];
            for (int i = 1; i < output.Length - 1; i++)
            {
                output[12] ^= output[i];
            }

            bleBike.WriteCharacteristic("6e40fec3-b5a3-f393-e0a9-e50e24dcca9e", output);
        }

        public override void ReceivedData(byte[] data)
        {
            base.ReceivedData(data);
        }

        public override bool ToggleListening()
        {
            return false;
        }

        public override bool StartListening()
        {
            return false;
        }

        public override bool StopListening()
        {
            return false;
        }
    }
}
