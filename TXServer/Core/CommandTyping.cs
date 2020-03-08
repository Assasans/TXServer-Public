using System;
using System.Collections.Generic;
using System.Linq;

namespace TXServer.Core.Commands
{
    public static partial class CommandTyping
    {
        public enum CommandId
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
        public static readonly Dictionary<CommandId, Type> CommandType = new Dictionary<CommandId, Type>
        {
            { CommandId.SendEvent, typeof(SendEventCommand) },
            { CommandId.EntityShare, typeof(EntityShareCommand) },
            // { CommandId.EntityUnshare, typeof(EntityUnshareCommand) },
            { CommandId.ComponentAdd, typeof(ComponentAddCommand) },
            // { CommandId.ComponentRemove, typeof(ComponentRemoveCommand) },
            { CommandId.ComponentChange, typeof(ComponentChangeCommand) },
            { CommandId.InitTime, typeof(InitTimeCommand) },
            // { CommandId.Close, typeof(CloseCommand) },
        };

        public static readonly Dictionary<Type, CommandId> CommandByType = CommandType.ToDictionary(x => x.Value, x => x.Key);
    }
}
