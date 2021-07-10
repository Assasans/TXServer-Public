using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle.Flag
{
    [SerialVersionUID(-1910863908782544246L)]
    public class FlagDropRequestEvent : ECSEvent
    {
        public static void Execute(Player player, Entity flagEntity, Entity tank)
        {
            Core.Battles.Battle battle = player.BattlePlayer.Battle;
            var handler = (Core.Battles.Battle.CTFHandler)battle.ModeHandler;

            Core.Battles.Flag enemyFlag = handler.BattleViewFor(player.BattlePlayer).EnemyTeamFlag;

            if (enemyFlag.State == FlagState.Captured)
                enemyFlag.Drop(true);
        }
    }
}
