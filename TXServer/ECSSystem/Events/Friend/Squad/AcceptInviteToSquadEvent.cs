using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Friend
{
    [SerialVersionUID(1507538648077L)]
    public class AcceptInviteToSquadEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {

        }

        public long FromUserId { get; set; }
        public long EngineId { get; set; }
    }
}
