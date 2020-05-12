using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using TXServer.Core.Commands;
using TXServer.ECSSystem.Base;
using TXServer.Library;
using static TXServer.Core.Commands.CommandManager;

namespace TXServer.Core.Protocol
{
    class DataDecoder
    {
        private readonly BinaryReader reader;
        private readonly OptionalMap map;

        private DataDecoder() { }

        public DataDecoder(BinaryReader reader)
        {
            this.reader = reader;
        }

        private DataDecoder(BinaryReader reader, OptionalMap map)
        {
            this.reader = reader;
            this.map = map;
        }

        private object DecodePrimitive(Type objType)
        {
            return reader.GetType()
                         .GetMethods()
                         .Single(method => method.Name == "Read" + objType.Name)
                         .Invoke(reader, null);
        }

        private object DecodeString()
        {
            byte part1 = reader.ReadByte();
            byte[] bytes;

            if ((part1 & 0x80) == 0x80)
            {
                bytes = reader.ReadBytes(((part1 & 0x3f) << 8) & reader.ReadByte());
            }
            else
            {
                bytes = reader.ReadBytes(part1);
            }

            return Encoding.UTF8.GetString(bytes);
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
            Type objType = FindCommandType(reader.ReadByte());

            return DecodeObject(objType);
        }

        private object SelectDecode(Type objType)
        {
            if (objType.IsPrimitive || objType == typeof(decimal))
            {
                return DecodePrimitive(objType);
            }

            if (objType == typeof(string))
            {
                return DecodeString();
            }

            if (objType.IsEnum)
            {
                return Enum.ToObject(objType, reader.ReadByte());
            }

            if (objType == typeof(Entity))
            {
                return DecodeEntity();
            }

            if (typeof(IDictionary).IsAssignableFrom(objType))
            {
                throw new NotImplementedException();
            }
            
            if (objType.IsArray || (objType.IsGenericType && typeof(ICollection<>).MakeGenericType(objType.GetGenericArguments()[0]).IsAssignableFrom(objType)))
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

        private object UnpackData(Type objType = null)
        {
            Int32 mapLength, length;

            // Magic.
            if (!reader.ReadBytes(2).SequenceEqual(Magic))
                throw new FileFormatException();

            // Length values.
            mapLength = reader.ReadInt32();
            length = reader.ReadInt32();

            // OptionalMap and packet contents.
            OptionalMap map = new OptionalMap(reader.ReadBytes((int)Math.Ceiling(mapLength / 8.0)), mapLength);

            using (MemoryStream stream = new MemoryStream(reader.ReadBytes(length)))
            {
                BinaryReader buffer = new BigEndianBinaryReader(stream);
                DataDecoder unpacker = new DataDecoder(buffer, map);

                if (objType == null)
                {
                    // Deserialize objects.
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

        public List<Command> DecodeCommands() => UnpackData() as List<Command>;
    }
}
