using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Friend
{
    [SerialVersionUID(1507725575587L)]
    public class FriendInviteToSquadEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {

        }

        public long UserId { get; set; }
        public InteractionSource InteractionSource { get; set; }
        public long SourceId { get; set; }
    }
}
