using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.Events.User.Squad
{
    [SerialVersionUID(1507799564788L)]
    public class RequestedToSquadEvent : ECSEvent
    {
        public RequestedToSquadEvent(Player fromPlayer, long squadId)
        {
            UserUid = fromPlayer.User.GetComponent<UserUidComponent>().Uid;
            FromUserId = fromPlayer.User.EntityId;
            SquadId = squadId;
            EngineId = 0;
        }

        public string UserUid { get; set; }
        public long FromUserId { get; set; }
        public long SquadId { get; set; }
        public long EngineId { get; set; }
    }
}