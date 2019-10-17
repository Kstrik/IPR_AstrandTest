using System;
using System.Collections.Generic;
using System.Text;

namespace Networking.ÄstrandTest
{
    public class Message
    {
        public enum ID
        {
            CLIENT_REGISTER,
            SPECIALIST_REGISTER,
            CLIENT_LOGIN,
            SPECIALIST_LOGIN,
            LOGOUT,

            START_TEST,
            END_TEST,
            BIKEDATA,

            GET_TESTS,
            TEST_NAME,
            GET_TEST_DATA,
            TEST_DATA,
            TEST_DATA_BEGIN,
            TEST_DATA_END,
        }

        public enum State
        {
            NONE,
            ERROR,
            OK
        }

        public enum ValueId
        {
            HEARTRATE,
            DISTANCE,
            SPEED,
            CYCLE_RHYTHM
        }

        private byte id;
        private byte state;
        private byte[] contentLength;
        private byte[] content;

        public Message(ID id, State state, byte[] content)
        {
            this.id = (byte)id;
            this.state = (byte)state;

            if (content == null)
                content = new byte[0];

            this.contentLength = new byte[4];
            contentLength[0] = (byte)content.Length;
            contentLength[1] = (byte)(content.Length >> 8);
            contentLength[2] = (byte)(content.Length >> 16);
            contentLength[3] = (byte)(content.Length >> 24);

            this.content = content;
        }

        private Message(ID id, State state, byte[] contentLength, byte[] content)
        {
            this.id = (byte)id;
            this.state = (byte)state;
            this.contentLength = contentLength;
            this.content = content;
        }

        public byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(this.id);
            bytes.Add(this.state);
            bytes.AddRange(this.contentLength);
            bytes.AddRange(this.content);

            return bytes.ToArray();
        }

        public static Message Parse(byte[] bytes)
        {
            byte[] contentLength = new byte[] { bytes[2], bytes[3], bytes[4], bytes[5] };
            byte[] content = new List<byte>(bytes).GetRange(6, bytes.Length - 6).ToArray();
            return new Message((ID)bytes[0], (State)bytes[1], contentLength, content);
        }

        public ID GetId()
        {
            return (ID)this.id;
        }

        public State GetState()
        {
            return (State)this.state;
        }

        public byte[] GetContent()
        {
            return this.content;
        }
    }
}
