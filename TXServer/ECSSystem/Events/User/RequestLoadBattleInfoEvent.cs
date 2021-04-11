using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Friend
{
    [SerialVersionUID(635890723433891050L)]
    public class RequestLoadBattleInfoEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            Core.Battles.Battle battle = Server.FindBattleById(lobbyId: 0, battleId: BattleId);

            player.SendEvent(new BattleInfoForLabelLoadedEvent(battle.MapEntity, BattleId, battle.Params.BattleMode.ToString()), entity);
        }

        public long BattleId { get; private set; }
    }
}
