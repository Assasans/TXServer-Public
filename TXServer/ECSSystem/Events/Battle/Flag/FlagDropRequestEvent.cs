using System.Linq;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(-1910863908782544246L)]
    public class FlagDropRequestEvent : ECSEvent
    {
        public static void Execute(Player player, Entity flagEntity, Entity tank)
        {
            Core.Battles.Battle battle = player.BattlePlayer.Battle;
            var handler = (Core.Battles.Battle.CTFHandler)battle.ModeHandler;

            Flag enemyFlag = handler.BattleViewFor(player.BattlePlayer).EnemyTeamFlag;

            if (enemyFlag.FlagState == FlagState.Captured)
                enemyFlag.FlagState = FlagState.Dropped;
            else
                return;

            enemyFlag.Drop(player, isUserAction:true);
        }
    }
}