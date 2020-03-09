using System;
using System.Collections.Generic;
using System.Linq;

namespace TXServer.Core.Commands
{
    public static partial class CommandTyping
    {
        public enum CommandCode 
        {
            SendEvent = 1,
            EntityShare,
            EntityUnshare,
            ComponentAdd,
            ComponentRemove,
            ComponentChange,
            InitTime,
            Close = 9
        }


        // Список типов команд.
        public static readonly Dictionary<CommandCode, Type> CommandTypeByCode = new Dictionary<CommandCode, Type>
        {
            { CommandCode.SendEvent, typeof(SendEventCommand) },
            { CommandCode.EntityShare, typeof(EntityShareCommand) },
            // { CommandId.EntityUnshare, typeof(EntityUnshareCommand) },
            { CommandCode.ComponentAdd, typeof(ComponentAddCommand) },
            // { CommandId.ComponentRemove, typeof(ComponentRemoveCommand) },
            { CommandCode.ComponentChange, typeof(ComponentChangeCommand) },
            { CommandCode.InitTime, typeof(InitTimeCommand) },
            // { CommandId.Close, typeof(CloseCommand) },
        };

        public static readonly Dictionary<Type, CommandCode> CommandCodeByType = CommandTypeByCode.ToDictionary(x => x.Value, x => x.Key);
    }
}
