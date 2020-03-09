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

        private void EncodePrimitive(object obj)
        {
            writer.GetType()
                  .GetMethod("Write", new Type[] { obj.GetType() })
                  .Invoke(writer, new object[] { obj });
        }

        private void EncodeCollection(object obj)
        {
            ICollection collection = obj as ICollection;

            writer.Write((byte)collection.Count);

            foreach (object el in collection)
            {
                SelectEncode(el);
            }
        }

        private void EncodeCommand(object obj)
        {
            writer.Write((byte)CommandCodeByType[obj.GetType()]);
        }

        private void EncodeEntity(object obj)
        {
            writer.Write(Player.Instance.Value.EntityIds[obj as Entity]);
        }
        
        private void SelectEncode(object obj)
        {
            Type objType = obj.GetType();

            if (objType.IsPrimitive || objType == typeof(string) || objType == typeof(decimal))
            {
                EncodePrimitive(obj);
                return;
            }

           else if (objType == typeof(Entity))
            {
                EncodeEntity(obj);
            }

            else if (typeof(ICollection).IsAssignableFrom(objType))
            {
                EncodeCollection(obj);
                return;
            }

            else if (typeof(Command).IsAssignableFrom(objType))
            {
                EncodeCommand(obj);
            }

            else if (Attribute.IsDefined(objType, typeof(SerialVersionUIDAttribute)))
            {
                writer.Write(SerialVersionUIDTools.GetId(objType));
            }

            EncodeObject(obj);
        }

        private void EncodeObject(object obj)
        {
            foreach (PropertyInfo info in GetProtocolProperties(obj.GetType()))
            {
                object value = info.GetValue(obj);

                if (info.GetCustomAttribute(typeof(OptionalMappedAttribute)) != null)
                {
                    map.Add(value == null);
                    if (value == null) continue;
                }

                SelectEncode(value);
            }
        }

        public void PackData(Command[] commands)
        {
            BinaryWriter writtenCommands = new BigEndianBinaryWriter(new MemoryStream());

            OptionalMap map = new OptionalMap();
            DataPacker encoder = new DataPacker(writtenCommands, map);

            foreach (Command command in commands)
            {
                encoder.SelectEncode(command);
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
