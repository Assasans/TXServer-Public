using System;
using System.Text;
using System.Collections.Generic;

namespace Simple.Net {
    public class NetBuffer : IDisposable {
        public List<byte> buffer { get; protected set; } = new List<byte>();
        public long hashCode {get; protected set; }
        public byte[] ToByteArray() {
            List<byte> result = new List<byte>();
            result.AddRange(BitConverter.GetBytes(hashCode));
            result.AddRange(buffer);
            return result.ToArray();
        }
        
        public override string ToString() {
            var sb = new StringBuilder("byte[] { ");
            foreach (var b in ToByteArray())
                sb.Append(b + ", ");
            sb.Append("}");
            return sb.ToString();
        }

        public void Dispose()
            => buffer = null;
    }
}