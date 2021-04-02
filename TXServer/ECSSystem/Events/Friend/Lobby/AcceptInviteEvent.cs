using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using static TXServer.Core.Battles.Battle;

namespace TXServer.ECSSystem.Events.Friend
{
    [SerialVersionUID(1497349612322L)]
    public class AcceptInviteEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            Core.Battles.Battle battle = Server.FindBattleById(lobbyId, 0);

            battle.AddPlayer(player);
        }

        public long lobbyId { get; set; }
        public long engineId { get; set; }
    }
}
