using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TXServer.Bits;
using TXServer.Core.Commands;
using TXServer.Core.ECSSystem;
using static TXServer.Core.Commands.CommandManager;
using static TXServer.Core.Commands.CommandTyping;

namespace TXServer.Core
{
    class DataUnpacker
    {
        private readonly BinaryReader reader;
        private readonly OptionalMap map;

        private DataUnpacker() { }

        public DataUnpacker(BinaryReader reader)
        {
            this.reader = reader;
        }

        private DataUnpacker(BinaryReader reader, OptionalMap map)
        {
            this.reader = reader;
            this.map = map;
        }

        private object Decode(Type objType, bool decodeEntity = false) // Pasta-style code.
        {
            object obj = null;

            if (objType.IsPrimitive || objType == typeof(string) || objType == typeof(decimal))
            {
                return reader.GetType()
                                .GetMethods()
                                .Single(method => method.Name.Contains("Read") && method.ReturnType == objType)
                                .Invoke(reader, null);
            }

            else if (objType.IsArray) // Untested
            {
                object[] arr = new object[reader.ReadByte()];

                for (var i = 0; i < arr.Length; i++)
                {
                    arr[i] = Decode(objType.GetElementType());
                }
                return arr;
            }
            else if (typeof(ICollection).IsAssignableFrom(objType))
            {
                obj = Activator.CreateInstance(objType);
                Type collectionInnerType = objType.GetGenericArguments()[0];

                byte count = reader.ReadByte();

                for (int i = 0; i < count; i++)
                {
                    objType.GetMethod("Add", new Type[] { collectionInnerType })
                           .Invoke(obj, new object[] { Decode(collectionInnerType) });
                }
                return obj;
            }

            if (objType == typeof(Entity))
            {
                UInt64 EntityId = reader.ReadUInt64();
                if (decodeEntity)
                {
                    obj = UnpackData(objType, decodeEntity);

                    Player.Instance.Value.EntityList[EntityId] = obj as Entity;
                    Player.Instance.Value.EntityIds[obj as Entity] = EntityId;

                    return obj;
                }
                else
                {
                    return Entity.FindById(EntityId);
                }
            }
            else if (typeof(Command).IsAssignableFrom(objType))
            {
                objType = CommandType[(CommandId)reader.ReadByte()];

                if (objType == typeof(EntityShareCommand))
                    decodeEntity = true;

                return DecodeObject(objType, decodeEntity);
            }
            else if (objType.IsInterface || objType.IsAbstract)
            {
                objType = SerialVersionUIDTools.FindType(reader.ReadUInt64());
            }

            return UnpackData(objType, decodeEntity);
        }

        private object DecodeObject(Type objType, bool decodeEntity)
        {
            object obj = Activator.CreateInstance(objType);

            foreach (PropertyInfo info in GetProtocolProperties(objType))
            {
                if (info.GetCustomAttribute(typeof(OptionalMappedAttribute)) != null)
                {
                    if (map.Read()) continue;
                }

                info.SetValue(obj, Decode(info.PropertyType, decodeEntity));
            }

            return obj;
        }

        public object UnpackData(Type objType = null, bool decodeEntity = false)
        {
            Int32 mapLength, length;

            // Получение заголовка.
            if (!reader.ReadBytes(2).SequenceEqual(Magic))
                throw new FileFormatException();

            // Получение значений из заголовка.
            mapLength = reader.ReadInt32();
            length = reader.ReadInt32();

            // Получение OptionalMap и содержимого пакета.
            OptionalMap map = new OptionalMap(reader.ReadBytes((int)Math.Ceiling(mapLength / 8.0)), mapLength);

            using (MemoryStream stream = new MemoryStream(reader.ReadBytes(length)))
            {
                BinaryReader buffer = new BigEndianBinaryReader(stream);

                DataUnpacker decoder = new DataUnpacker(buffer, map);

                if (objType == null)
                {
                    // Развертывание команд в объекты.
                    List<Command> commands = new List<Command>();

                    while (buffer.BaseStream.Position != length)
                    {
                        commands.Add(decoder.Decode(typeof(Command)) as Command);
                    }

                    return commands;
                }
                else
                {
                    return decoder.DecodeObject(objType, decodeEntity);
                }
            }
        }
    }
}
