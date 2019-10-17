using ÄstrandTestSpecialistClient.UI;
using Networking.ÄstrandTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ÄstrandTestSpecialistClient.Net
{
    public class TestData
    {
        public TestDataControl TestDataControl;
        public string Username;

        public TestData(string username)
        {
            this.Username = username;
            this.TestDataControl = new TestDataControl(this.Username);
            this.TestDataControl.Foreground = Brushes.White;
            this.TestDataControl.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30"));
        }

        public void HandleBikeData(List<byte> bytes)
        {
            for (int i = 0; i < bytes.Count; i += 2)
            {
                int value = bytes[i + 1];

                switch ((Message.ValueId)bytes[i])
                {
                    case Message.ValueId.HEARTRATE:
                        {
                            this.TestDataControl.Heartrate = value;
                            this.TestDataControl.UpdateChart(value);
                            break;
                        }
                    case Message.ValueId.DISTANCE:
                        {
                            this.TestDataControl.Distance = value;
                            break;
                        }
                    case Message.ValueId.SPEED:
                        {
                            this.TestDataControl.Speed = value;
                            break;
                        }
                    case Message.ValueId.CYCLE_RHYTHM:
                        {
                            this.TestDataControl.CycleRhythm = value;
                            break;
                        }
                }
            }
        }
    }
}
