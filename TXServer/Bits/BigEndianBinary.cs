using System;
using System.IO;

namespace TXServer.Bits
{
    class BigEndianBinaryReader : BinaryReader
    {
        public BigEndianBinaryReader(Stream stream) : base(stream) { }

        public override Int32 ReadInt32()
        {
            var data = base.ReadBytes(4);
            Array.Reverse(data);
            return BitConverter.ToInt32(data, 0);
        }

        public override Int64 ReadInt64()
        {
            var data = base.ReadBytes(8);
            Array.Reverse(data);
            return BitConverter.ToInt64(data, 0);
        }

        public override UInt32 ReadUInt32()
        {
            var data = base.ReadBytes(4);
            Array.Reverse(data);
            return BitConverter.ToUInt32(data, 0);
        }

        public override UInt64 ReadUInt64()
        {
            var data = base.ReadBytes(8);
            Array.Reverse(data);
            return BitConverter.ToUInt64(data, 0);
        }
    }

    class BigEndianBinaryWriter : BinaryWriter
    {
        public BigEndianBinaryWriter(Stream stream) : base(stream) { }

        public override void Write(Int32 i)
        {
            byte[] data = BitConverter.GetBytes(i);
            Array.Reverse(data);
            base.Write(data);
        }

        public override void Write(Int64 i)
        {
            byte[] data = BitConverter.GetBytes(i);
            Array.Reverse(data);
            base.Write(data);
        }

        public override void Write(UInt32 i)
        {
            byte[] data = BitConverter.GetBytes(i);
            Array.Reverse(data);
            base.Write(data);
        }

        public override void Write(UInt64 i)
        {
            byte[] data = BitConverter.GetBytes(i);
            Array.Reverse(data);
            base.Write(data);
        }
    }
}
