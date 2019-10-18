using Networking;
using Networking.ÄstrandTest;
using Networking.ÄstrandTest.Net.ServerSide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ÄstrandTestServer.Net
{
    public class ÄstrandTest
    {
        public string Username;
        public int BirthYear;
        public int Weight;
        public bool IsMan;

        public List<(int heartRate, DateTime time)> HeartrateValues;
        public List<(int distance, DateTime time)> DistanceValues;
        public List<(int speed, DateTime time)> SpeedValues;
        public List<(int cycleRhythm, DateTime time)> CycleRhythmValues;

        public ÄstrandTest()
        {
            this.HeartrateValues = new List<(int heartRate, DateTime time)>();
            this.DistanceValues = new List<(int distance, DateTime time)>();
            this.SpeedValues = new List<(int speed, DateTime time)>();
            this.CycleRhythmValues = new List<(int cycleRhythm, DateTime time)>();

            this.Username = "";
            this.BirthYear = 0;
            this.Weight = 0;
            this.IsMan = true;
        }

        public ÄstrandTest(string username, int birthYear, int weight, bool isMan)
        {
            this.HeartrateValues = new List<(int heartRate, DateTime time)>();
            this.DistanceValues = new List<(int distance, DateTime time)>();
            this.SpeedValues = new List<(int speed, DateTime time)>();
            this.CycleRhythmValues = new List<(int cycleRhythm, DateTime time)>();

            this.Username = username;
            this.BirthYear = birthYear;
            this.Weight = weight;
            this.IsMan = isMan;
        }

        public void Transmit(UserAccount userAccount)
        {
            if (userAccount.Connection != null)
            {
                List<byte> startBytes = new List<byte>();
                startBytes.Add((byte)this.Username.Length);
                startBytes.AddRange(Encoding.UTF8.GetBytes(this.Username));
                startBytes.Add((byte)this.BirthYear.ToString().Length);
                startBytes.AddRange(Encoding.UTF8.GetBytes(this.BirthYear.ToString()));
                startBytes.Add((byte)this.Weight);
                startBytes.Add((this.IsMan) ? (byte)1 : (byte)0);
                userAccount.Connection.Transmit(DataEncryptor.Encrypt(new Message(Message.ID.TEST_DATA_BEGIN, Message.State.OK, startBytes.ToArray()).GetBytes(), DataEncryptor.NetworkKey));

                int maxLength = GetMaxLength();
                for (int i = 0; i < maxLength; i++)
                {
                    List<byte> bytes = new List<byte>();
                    bytes.Add((byte)this.Username.Length);
                    bytes.AddRange(Encoding.UTF8.GetBytes(this.Username));

                    if (HeartrateValues.Count - 1 > i)
                    {
                        bytes.Add((byte)Message.ValueId.HEARTRATE);
                        bytes.Add((byte)HeartrateValues[i].heartRate);
                        bytes.AddRange(Encoding.UTF8.GetBytes(HeartrateValues[i].time.ToString()));
                    }

                    if (DistanceValues.Count - 1 > i)
                    {
                        bytes.Add((byte)Message.ValueId.DISTANCE);
                        bytes.Add((byte)DistanceValues[i].distance);
                        bytes.AddRange(Encoding.UTF8.GetBytes(DistanceValues[i].time.ToString()));
                    }

                    if (SpeedValues.Count - 1 > i)
                    {
                        bytes.Add((byte)Message.ValueId.SPEED);
                        bytes.Add((byte)SpeedValues[i].speed);
                        bytes.AddRange(Encoding.UTF8.GetBytes(SpeedValues[i].time.ToString()));
                    }

                    if (CycleRhythmValues.Count - 1 > i)
                    {
                        bytes.Add((byte)Message.ValueId.CYCLE_RHYTHM);
                        bytes.Add((byte)CycleRhythmValues[i].cycleRhythm);
                        bytes.AddRange(Encoding.UTF8.GetBytes(CycleRhythmValues[i].time.ToString()));
                    }

                    userAccount.Connection.Transmit(DataEncryptor.Encrypt(new Message(Message.ID.TEST_DATA, Message.State.OK, bytes.ToArray()).GetBytes(), DataEncryptor.NetworkKey));
                }

                List<byte> endBytes = new List<byte>();
                endBytes.Add((byte)this.Username.Length);
                endBytes.AddRange(Encoding.UTF8.GetBytes(this.Username));
                userAccount.Connection.Transmit(DataEncryptor.Encrypt(new Message(Message.ID.TEST_DATA_END, Message.State.OK, endBytes.ToArray()).GetBytes(), DataEncryptor.NetworkKey));
            }
        }

        private int GetMaxLength()
        {
            return Math.Max(Math.Max(HeartrateValues.Count, DistanceValues.Count), Math.Max(SpeedValues.Count, CycleRhythmValues.Count));
        }
    }
}
