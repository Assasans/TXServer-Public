using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using TXServer.Core.Commands;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.Types;
using TXServer.Library;
using static TXServer.Core.Commands.CommandManager;

namespace TXServer.Core.Protocol
{
    class DataDecoder
    {
        private readonly BinaryReader reader;
        private readonly OptionalMap map;

        private readonly Dictionary<Type, Func<object>> decodeMethods;

        private DataDecoder()
        {
            decodeMethods = new Dictionary<Type, Func<object>>
            {
                { typeof(string), DecodeString },
                { typeof(Vector3), DecodeVector3 },
                { typeof(MoveCommand), DecodeMoveCommand },
                { typeof(Movement), DecodeMovement },
                { typeof(Type), DecodeType }
            };
        }

        public DataDecoder(BinaryReader reader) : this()
        {
            this.reader = reader;
        }

        private DataDecoder(BinaryReader reader, OptionalMap map) : this()
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

            return player.FindEntityById(EntityId);
        }

        private object DecodeVector3()
        {
            return new Vector3(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
        }

        private object DecodeCommand(Player player)
        {
            Type objType = FindCommandType(reader.ReadByte());

            return DecodeObject(objType, player);
        }

        private object DecodeMoveCommand()
        {
            const int WEAPON_ROTATION_SIZE = 2;
            byte[] bufferEmpty = Array.Empty<byte>();
            byte[] bufferForWeaponRotation = new byte[WEAPON_ROTATION_SIZE];
            BitArray bitsEmpty = new BitArray(bufferEmpty);
            BitArray bitsForWeaponRotation = new BitArray(bufferForWeaponRotation);
            const int WEAPON_ROTATION_COMPONENT_BITSIZE = WEAPON_ROTATION_SIZE * 8;
            const float WEAPON_ROTATION_FACTOR = 360f / (1 << WEAPON_ROTATION_COMPONENT_BITSIZE);

            bool flag = map.Read();
            bool flag2 = map.Read();
            bool flag3 = map.Read();
            MoveCommand moveCommand = default;
            if (flag3)
            {
                DiscreteTankControl discreteTankControl = default;
                discreteTankControl.Control = reader.ReadByte();
                moveCommand.TankControlHorizontal = discreteTankControl.TurnAxis;
                moveCommand.TankControlVertical = discreteTankControl.MoveAxis;
                moveCommand.WeaponRotationControl = discreteTankControl.WeaponControl;
            }
            else
            {
                moveCommand.TankControlVertical = reader.ReadSingle();
                moveCommand.TankControlHorizontal = reader.ReadSingle();
                moveCommand.WeaponRotationControl = reader.ReadSingle();
            }
            if (flag)
            {
                moveCommand.Movement = new Movement?((Movement)MovementCodec.Decode(reader));
            }
            byte[] buffer = GetBuffer(flag2);
            BitArray bits = GetBits(flag2);
            int num = 0;
            reader.Read(buffer, 0, buffer.Length);
            MovementCodec.CopyBits(buffer, bits);
            if (flag2)
            {
                moveCommand.WeaponRotation = new float?(MovementCodec.ReadFloat(bits, ref num, WEAPON_ROTATION_COMPONENT_BITSIZE, WEAPON_ROTATION_FACTOR));
            }
            if (num != bits.Length)
            {
                throw new Exception("Move command unpack mismatch");
            }
            moveCommand.ClientTime = reader.ReadInt32();
            return moveCommand;

            byte[] GetBuffer(bool hasWeaponRotation) => (!hasWeaponRotation) ? bufferEmpty : bufferForWeaponRotation;
            BitArray GetBits(bool hasWeaponRotation) => (!hasWeaponRotation) ? bitsEmpty : bitsForWeaponRotation;
        }

        private object DecodeMovement()
        {
            return MovementCodec.Decode(reader);
        }

        private object DecodeType()
        {
            return SerialVersionUIDTools.FindType(reader.ReadInt64());
        }

        private object SelectDecode(Type objType, Player player)
        {
            if (objType.IsPrimitive || objType == typeof(decimal))
            {
                return DecodePrimitive(objType);
            }

            if (decodeMethods.TryGetValue(objType, out Func<object> method))
            {
                return method();
            }

            if (objType.IsEnum)
            {
                return Enum.ToObject(objType, reader.ReadByte());
            }

            if (objType == typeof(Entity))
            {
                return DecodeEntity(player);
            }

            // if (typeof(DateTimeOffset).IsAssignableFrom(objType))
            // {
            //     long time = reader.ReadInt64();
            //     return DateTimeOffset.FromUnixTimeMilliseconds(time + player.Connection.DiffToClient);
            // }

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
                objType = (Type)DecodeType();
                return UnpackData(player, objType);
            }

            return DecodeObject(objType, player);
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

                if (objType != null)
                    return unpacker.DecodeObject(objType, player);

                // Deserialize objects.
                List<Command> commands = new List<Command>();

                while (buffer.BaseStream.Position != length)
                {
                    commands.Add(unpacker.SelectDecode(typeof(Command), player) as Command);
                }

                return commands;
            }
        }

        public List<Command> DecodeCommands(Player player) => UnpackData(player) as List<Command>;
    }
}
