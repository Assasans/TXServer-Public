using System;
using System.IO;

namespace TXServer.Library
{
    class BigEndianBinaryReader : BinaryReader
    {
        public BigEndianBinaryReader(Stream stream) : base(stream) { }

        public override short ReadInt16()
        {
            var data = base.ReadBytes(2);
            Array.Reverse(data);
            return BitConverter.ToInt16(data, 0);
        }

        public override int ReadInt32()
        {
            var data = base.ReadBytes(4);
            Array.Reverse(data);
            return BitConverter.ToInt32(data, 0);
        }

        public override long ReadInt64()
        {
            var data = base.ReadBytes(8);
            Array.Reverse(data);
            return BitConverter.ToInt64(data, 0);
        }

        public override ushort ReadUInt16()
        {
            var data = base.ReadBytes(2);
            Array.Reverse(data);
            return BitConverter.ToUInt16(data, 0);
        }

        public override uint ReadUInt32()
        {
            var data = base.ReadBytes(4);
            Array.Reverse(data);
            return BitConverter.ToUInt32(data, 0);
        }

        public override ulong ReadUInt64()
        {
            var data = base.ReadBytes(8);
            Array.Reverse(data);
            return BitConverter.ToUInt64(data, 0);
        }

        public float ReadFloat()
        {
            var data = base.ReadBytes(4);
            Array.Reverse(data);
            return BitConverter.ToSingle(data, 0);
        }
    }

    class BigEndianBinaryWriter : BinaryWriter
    {
        public BigEndianBinaryWriter(Stream stream) : base(stream) { }

        public override void Write(short i)
        {
            byte[] data = BitConverter.GetBytes(i);
            Array.Reverse(data);
            base.Write(data);
        }

        public override void Write(int i)
        {
            byte[] data = BitConverter.GetBytes(i);
            Array.Reverse(data);
            base.Write(data);
        }

        public override void Write(long i)
        {
            byte[] data = BitConverter.GetBytes(i);
            Array.Reverse(data);
            base.Write(data);
        }

        public override void Write(ushort i)
        {
            byte[] data = BitConverter.GetBytes(i);
            Array.Reverse(data);
            base.Write(data);
        }

        public override void Write(uint i)
        {
            byte[] data = BitConverter.GetBytes(i);
            Array.Reverse(data);
            base.Write(data);
        }

        public override void Write(ulong i)
        {
            byte[] data = BitConverter.GetBytes(i);
            Array.Reverse(data);
            base.Write(data);
        }

        public override void Write(float i)
        {
            byte[] data = BitConverter.GetBytes(i);
            Array.Reverse(data);
            base.Write(data);
        }
    }
}
