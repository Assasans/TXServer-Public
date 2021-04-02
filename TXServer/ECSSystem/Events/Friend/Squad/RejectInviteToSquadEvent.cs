using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Friend
{
    [SerialVersionUID(1510640414175L)]
    public class RejectInviteToSquadEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {

        }

        public long FromUserId { get; set; }
        public long EngineId { get; set; }
    }
}
