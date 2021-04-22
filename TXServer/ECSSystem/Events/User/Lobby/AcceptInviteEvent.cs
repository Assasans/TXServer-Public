using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using static TXServer.Core.Battles.Battle;

namespace TXServer.ECSSystem.Events.User.Lobby
{
    [SerialVersionUID(1497349612322L)]
    public class AcceptInviteEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            Core.Battles.Battle battle = Server.Instance.FindBattleById(lobbyId, 0);

            battle.AddPlayer(player, false);
        }

        public long lobbyId { get; set; }
        public long engineId { get; set; }
    }
}
