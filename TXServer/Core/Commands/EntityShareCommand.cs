using System.IO;
using System;
using TXServer.Core.ECSSystem;
using TXServer.Bits;

namespace TXServer.Core.Commands
{
    public class EntityShareCommand : Command
    {
        public EntityShareCommand() { }

        public EntityShareCommand(Entity Entity)
        {
            this.Entity = Entity;
        }

        public override void BeforeWrap()
        {
            UInt64 newEntityId = Player.Instance.Value.GenerateId();

            // Добавить Entity в общий список.
            Player.Instance.Value.EntityList[newEntityId] = Entity;
            Player.Instance.Value.EntityIds[Entity] = newEntityId;
        }

        [Protocol] public Entity Entity { get; set; }
    }
}
