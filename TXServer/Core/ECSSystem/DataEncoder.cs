using System;
using System.Collections;
using System.IO;
using System.Reflection;
using TXServer.Core.ECSSystem;
using TXServer.Core.Commands;
using static TXServer.Core.Commands.CommandTyping;
using static TXServer.Core.Commands.CommandManager;
using TXServer.Bits;

namespace TXServer.Core
{
    class DataPacker
    {
        private readonly BinaryWriter writer;
        private readonly OptionalMap map;

        private DataPacker() { }

        public DataPacker(BinaryWriter writer)
        {
            this.writer = writer;
        }

        private DataPacker(BinaryWriter writer, OptionalMap map)
        {
            this.writer = writer;
            this.map = map;
        }

        private void Encode(object obj, bool encodeEntity = false)
        {
            Type objType = obj.GetType();

            if (objType.IsPrimitive || objType == typeof(string) || objType == typeof(decimal))
            {
                writer.GetType()
                      .GetMethod("Write", new Type[] { objType })
                      .Invoke(writer, new object[] { obj });
                return;
            }

            else if (objType.IsArray)
            {
                object[] arr = obj as object[];

                if (!typeof(Command).IsAssignableFrom(objType.GetElementType()))
                    writer.Write((byte)arr.Length);

                foreach (object el in arr)
                {
                    Encode(el);
                }
                return;
            }
            else if (typeof(ICollection).IsAssignableFrom(objType))
            {
                ICollection collection = obj as ICollection;

                writer.Write((byte)collection.Count);

                foreach (object el in collection)
                {
                    Encode(el);
                }
                return;
            }

            else if (typeof(Command).IsAssignableFrom(objType))
            {
                writer.Write((byte)CommandByType[objType]);
                if (objType == typeof(EntityShareCommand))
                    encodeEntity = true;
            }
            else if (objType.GetCustomAttribute(typeof(SerialVersionUIDAttribute)) != null)
            {
                writer.Write(SerialVersionUIDTools.GetId(objType));
            }

            else if (objType == typeof(Entity))
            {
                writer.Write(Player.Instance.Value.EntityIds[obj as Entity]);
                if (!encodeEntity) return;
            }

            foreach (PropertyInfo info in GetProtocolProperties(objType))
            {
                object value = info.GetValue(obj);

                if (info.GetCustomAttribute(typeof(OptionalMappedAttribute)) != null)
                {
                    map.Add(value == null);
                    if (value == null) continue;
                }

                Encode(value, encodeEntity);
            }
        }

        public void PackData(Command[] commands)
        {
            BinaryWriter writtenCommands = new BigEndianBinaryWriter(new MemoryStream());

            OptionalMap map = new OptionalMap();
            DataPacker encoder = new DataPacker(writtenCommands, map);

            foreach (Command command in commands)
            {
                command.BeforeWrap();
                encoder.Encode(command);
            }

            map.Reset();

            writer.Write(Magic);
            writer.Write(map.Length);
            writer.Write((UInt32)writtenCommands.BaseStream.Length);

            writer.Write(map.Data());

            writtenCommands.BaseStream.Position = 0;
            writtenCommands.BaseStream.CopyTo(writer.BaseStream);
        }
    }
}
