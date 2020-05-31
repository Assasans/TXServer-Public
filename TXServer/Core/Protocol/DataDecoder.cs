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

        private int DecodeLength(BinaryReader buf)
        {
            byte num1 = buf.ReadByte();
            if ((num1 & 128) == 0) return num1;
            
            byte num2 = buf.ReadByte();
            if ((num1 & 64) == 0) return ((num1 & 63) << 8) + (num2 & byte.MaxValue);
            
            byte num3 = buf.ReadByte();
            return ((num1 & 63) << 16) + ((num2 & byte.MaxValue) << 8) + (num3 & byte.MaxValue);
        }

        private object DecodeCollection(Type objType, Player player)
        {
            int count = DecodeLength(reader);

            if (objType.IsArray)
            {
                Array array = Array.CreateInstance(objType.GetElementType(), count);

                for (int i = 0; i < count; i++)
                {
                    array.SetValue(SelectDecode(objType.GetElementType(), player), i);
                }

                return array;
            }

            object obj = Activator.CreateInstance(objType);
            Type collectionInnerType = objType.GetGenericArguments()[0];

            for (int i = 0; i < count; i++)
            {
                objType.GetMethod("Add", new Type[] { collectionInnerType })
                    .Invoke(obj, new object[] { SelectDecode(collectionInnerType, player) });
            }

            return obj;
        }

        private object DecodeEntity(Player player)
        {
            Int64 EntityId = reader.ReadInt64();

            return player.FindById(EntityId);
        }

        private object DecodeCommand(Player player)
        {
            Type objType = FindCommandType(reader.ReadByte());

            return DecodeObject(objType, player);
        }

        private object SelectDecode(Type objType, Player player)
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
                return DecodeEntity(player);
            }

            if (typeof(IDictionary).IsAssignableFrom(objType))
            {
                throw new NotImplementedException();
            }
            
            if (objType.IsArray || (objType.IsGenericType && typeof(ICollection<>).MakeGenericType(objType.GetGenericArguments()[0]).IsAssignableFrom(objType)))
            {
                return DecodeCollection(objType, player);
            }

            if (typeof(Command).IsAssignableFrom(objType))
            {
                return DecodeCommand(player);
            }

            if (objType.IsAbstract || objType.IsInterface)
            {
                objType = SerialVersionUIDTools.FindType(reader.ReadInt64());
            }

            return UnpackData(player, objType);
        }

        private object DecodeObject(Type objType, Player player)
        {
            object obj = FormatterServices.GetUninitializedObject(objType);

            foreach (PropertyInfo info in GetProtocolProperties(objType))
            {
                if (Attribute.IsDefined(info.PropertyType, typeof(OptionalMappedAttribute)))
                    if (map.Read()) continue;

                info.SetValue(obj, SelectDecode(info.PropertyType, player));
            }

            return obj;
        }

        private object UnpackData(Player player, Type objType = null)
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
                        commands.Add(unpacker.SelectDecode(typeof(Command), player) as Command);
                    }

                    return commands;
                }
                else
                {
                    return unpacker.DecodeObject(objType, player);
                }
            }
        }

        public List<Command> DecodeCommands(Player player) => UnpackData(player) as List<Command>;
    }
}
