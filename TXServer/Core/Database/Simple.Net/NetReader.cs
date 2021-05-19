using System;
using System.Text;

namespace Simple.Net {
    public class NetReader : NetBuffer {
        public int readPos = 0;
        
        public NetReader(byte[] buffer) {
            this.buffer.AddRange(buffer);
            this.hashCode = ReadInt64();
        }
        
        public bool ReadBool() {
            try {
                return BitConverter.ToBoolean(buffer.ToArray(), readPos++);
            } catch {
                throw new OverflowException("Cannot read Bool!");
            }
        }

        public long ReadInt64() {
            try {
                long result = BitConverter.ToInt64(buffer.ToArray(), readPos);
                readPos += 8;
                return result;
            } catch {
                throw new OverflowException("Cannot read Int64!");
            }
        }
        
        public double ReadDouble() {
            try {
                double result = BitConverter.ToDouble(buffer.ToArray(), readPos);
                readPos += 8;
                return result;
            } catch {
                throw new OverflowException("Cannot read Double!");
            }
        }

        public string ReadString() {
            int length;
            try { length = (int)ReadInt64(); }
            catch { throw new OverflowException("Cannot read string! Failed to read Int64 length of string"); }
            try {
                string result = Encoding.UTF8.GetString(buffer.ToArray(), readPos, length);
                readPos += length;
                return result;
            } catch {
                throw new OverflowException($"Cannot read string of length '{length}'! End of buffer!");
            }
        }
    }
}