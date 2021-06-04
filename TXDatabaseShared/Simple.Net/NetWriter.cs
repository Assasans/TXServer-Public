using System;
using System.Text;

namespace Simple.Net {
    public class NetWriter : NetBuffer {
        public NetWriter(long hashCode)
            => this.hashCode = hashCode;
        
        public void Push(bool value)
            => buffer.AddRange(BitConverter.GetBytes(value));
        
        public void Push(long value)
            => buffer.AddRange(BitConverter.GetBytes(value));
        
        public void Push(double value)
            => buffer.AddRange(BitConverter.GetBytes(value));
        
        public void Push(string value) {
            if (value == null)
                value = string.Empty;
            
            byte[] content = Encoding.UTF8.GetBytes(value);
            Push((long)content.Length);
            buffer.AddRange(content);
        }
    }
}