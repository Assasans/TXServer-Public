using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.Events.Friend
{
    [SerialVersionUID(1507543176898L)]
    public class InvitedToSquadEvent : ECSEvent
    {
        public InvitedToSquadEvent(Player fromPlayer)
        {
            UserUid = fromPlayer.User.GetComponent<UserUidComponent>().Uid;
            FromUserId = fromPlayer.User.EntityId;
            EngineId = 0;
        }

        public string UserUid { get; set; }
        public long FromUserId { get; set; }
        public long EngineId { get; set; }
    }
}
