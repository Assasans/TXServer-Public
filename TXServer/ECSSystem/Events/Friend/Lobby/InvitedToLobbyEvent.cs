using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.Events.Friend
{
    [SerialVersionUID(1499233137837L)]
    public class InvitedToLobbyEvent : ECSEvent
    {
        public InvitedToLobbyEvent(Player fromPlayer)
        {
            userUid = fromPlayer.User.GetComponent<UserUidComponent>().Uid;
            lobbyId = fromPlayer.BattlePlayer.Battle.BattleLobbyEntity.EntityId;
            engineId = 0;
        }

        public string userUid { get; set; }
        public long lobbyId { get; set; }
        public long engineId { get; set; }
    }
}
