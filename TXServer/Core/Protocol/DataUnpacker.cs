using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using TXServer.Core.Commands;
using TXServer.ECSSystem.Base;
using TXServer.Library;
using static TXServer.Core.Commands.CommandManager;
using static TXServer.Core.Commands.CommandTyping;

namespace TXServer.Core.Protocol
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

        private object DecodePrimitive(Type objType)
        {
            return reader.GetType()
                         .GetMethods()
                         .Single(method => method.Name.Contains("Read") && method.ReturnType == objType)
                         .Invoke(reader, null);
        }

        private object DecodeCollection(Type objType)
        {
            object obj = Activator.CreateInstance(objType);
            Type collectionInnerType = objType.GetGenericArguments()[0];

            byte count = reader.ReadByte();

            for (int i = 0; i < count; i++)
            {
                objType.GetMethod("Add", new Type[] { collectionInnerType })
                       .Invoke(obj, new object[] { SelectDecode(collectionInnerType) });
            }

            return obj;
        }

        private object DecodeEntity()
        {
            Int64 EntityId = reader.ReadInt64();

            return Entity.FindById(EntityId);
        }

        private object DecodeCommand()
        {
            Type objType = CommandTypeByCode[(CommandCode)reader.ReadByte()];

            return DecodeObject(objType);
        }

        private object SelectDecode(Type objType)
        {
            if (objType.IsPrimitive || objType == typeof(string) || objType == typeof(decimal))
            {
                return DecodePrimitive(objType);
            }

            if (objType == typeof(Entity))
            {
                return DecodeEntity();
            }

            if (typeof(IDictionary).IsAssignableFrom(objType))
            {
                throw new NotImplementedException();
            }
            else if (typeof(ICollection).IsAssignableFrom(objType))
            {
                return DecodeCollection(objType);
            }

            if (typeof(Command).IsAssignableFrom(objType))
            {
                return DecodeCommand();
            }

            if (objType.IsAbstract || objType.IsInterface)
            {
                objType = SerialVersionUIDTools.FindType(reader.ReadInt64());
            }

            return UnpackData(objType);
        }

        private object DecodeObject(Type objType)
        {
            object obj = FormatterServices.GetUninitializedObject(objType);

            foreach (PropertyInfo info in GetProtocolProperties(objType))
            {
                if (Attribute.IsDefined(info.PropertyType, typeof(OptionalMappedAttribute)))
                    if (map.Read()) continue;

                info.SetValue(obj, SelectDecode(info.PropertyType));
            }

            return obj;
        }

        public object UnpackData(Type objType = null)
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
                DataUnpacker unpacker = new DataUnpacker(buffer, map);

                if (objType == null)
                {
                    // Развертывание команд в объекты.
                    List<Command> commands = new List<Command>();

                    while (buffer.BaseStream.Position != length)
                    {
                        commands.Add(unpacker.SelectDecode(typeof(Command)) as Command);
                    }

                    return commands;
                }
                else
                {
                    return unpacker.DecodeObject(objType);
                }
            }
        }
    }
}
