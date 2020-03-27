using System;
using System.Collections.Generic;
using System.Linq;

namespace TXServer.Core.Protocol
{
    public class OptionalMap
    {
        public OptionalMap()
        {
        }

        public OptionalMap(byte[] data, Int32 Length)
        {
            Load(data, Length);
        }

        private List<byte> data = new List<byte>();

        public Int32 Length { get; private set; }
        public Int32 Position { get; private set; }

        public void Add(bool isPresent)
        {
            if (Position >= Length)
            {
                data.Add(0);
                Length += 8;
            }
            data[Position / 8] |= (byte)(Convert.ToInt32(isPresent) << (7 - Position++ % 8));
        }

        public bool Read()
        {
            if (Position >= Length)
            {
                throw new InvalidOperationException("Попытка чтения в конце OptionalMap");
            }
            return Convert.ToBoolean((data[Position / 8] >> (7 - Position++ % 8)) & 1);
        }

        public void Reset() => Position = 0;

        public byte[] Data()
        {
            return data.ToArray();
        }

        public void Load(byte[] data, Int32 Length)
        {
            this.data = data.ToList();
            this.Length = Length;
        }
    }
}
