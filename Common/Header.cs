using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using System.Net;

namespace TCPCommunicator
{
    public class Header
    {
        public Header(int length) { Length = length; }

        public Header(byte[] rawData)
        {
            GCHandle gcHandle = GCHandle.Alloc(rawData, GCHandleType.Pinned);
            try
            {
                var rawHeader = (RawHeader)Marshal.PtrToStructure(gcHandle.AddrOfPinnedObject(), typeof(RawHeader));
                Length = rawHeader.Length;
                if (BitConverter.IsLittleEndian)
                {
                    //since network is big endian we have to correct for it
                    Length = IPAddress.NetworkToHostOrder(Length);
                }
            }
            finally
            {
                gcHandle.Free();
            }
        }
        public int Length { get; set; }

        public byte[] Serialize()
        {
            var rawHeader = new RawHeader() { Length = Length };
            if (BitConverter.IsLittleEndian)
            {
                //since network is big endian we have to correct for it
                rawHeader.Length = IPAddress.HostToNetworkOrder(rawHeader.Length);
            }
            byte[] rawData = new byte[RawHeader.Size];
            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms, Encoding.UTF8))
                {
                    bw.Write(rawHeader.Length);
                }
                rawData = ms.ToArray();
            }
            return rawData;
        }



        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = Size)]
        public unsafe struct RawHeader
        {
            public const int Size = 4;
            /* _________________________________________________________________
             * |    1-2 bit   | 3 bit |4 bit |  5-8 bit |   9-16bit  | 17-32 bit|
             * |______________|_______|______|__________|____________|__________|
             * |Version Number|Padding|Events|CSRC Count|PAYLOAD TYPE|   SSRC   |
             * |______________|_______|______|__________|____________|__________|
             */
            public Int32 Length;
        }
    }
}
