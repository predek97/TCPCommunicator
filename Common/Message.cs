using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace TCPCommunicator
{
    class Message
    {

        public Message() { }
        public Message(Stream rawData)
        {
            byte[] rawHeaderData = new byte[Header.RawHeader.Size];
            rawData.Read(rawHeaderData, 0, Header.RawHeader.Size);
            Header = new Header(rawHeaderData);
            byte[] rawMessageData = new byte[Header.Length];
            rawData.Read(rawMessageData, 0, Header.Length);
            Text = Encoding.BigEndianUnicode.GetString(rawMessageData);
        }

        public Message(string messageText)
        {
            Text = messageText;
        }
        public Header Header { get; private set; } = new Header(0);
        private string _text;
        public string Text
        {
            get => _text;
            private set
            {
                _text = value;
                Header.Length = Encoding.Unicode.GetByteCount(_text);
            }
        }

        public byte[] Serialize()
        {
            var ms = new MemoryStream();
            ms.Write(Header.Serialize());
            ms.Write(Encoding.BigEndianUnicode.GetBytes(_text));
            return ms.ToArray();
        }
    }
}
