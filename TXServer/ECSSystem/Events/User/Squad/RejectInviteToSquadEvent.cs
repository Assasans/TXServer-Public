using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.User.Squad
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
