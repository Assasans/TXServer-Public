using System.Linq;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Team;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(1463741053998L)]
    public class FlagCollisionRequestEvent : ECSEvent
    {
        public static void Execute(Player player, Entity tank, Entity flagEntity)
        {
            Core.Battles.Battle battle = player.BattlePlayer.Battle;
            var handler = (Core.Battles.Battle.CTFHandler)battle.ModeHandler;

            Flag collisedFlag = handler.Flags.Values.Single(x => x.FlagEntity == flagEntity);

            BattleView view = handler.BattleViewFor(player.BattlePlayer);
            Flag enemyFlag = view.EnemyTeamFlag;
            Flag allieFlag = view.AllyTeamFlag;
            
            if (battle.BattleState == BattleState.WarmUp)
                return;

            switch (collisedFlag.State)
            {
                case FlagState.Home:
                    if (flagEntity.GetComponent<TeamGroupComponent>().Key == tank.GetComponent<TeamGroupComponent>().Key)
                    {
                        if (enemyFlag.FlagEntity.GetComponent<TankGroupComponent>() == null)
                            return;
                        if (enemyFlag.State == FlagState.Captured)
                        {
                            if (enemyFlag.FlagEntity.GetComponent<TankGroupComponent>().Key != tank.GetComponent<TankGroupComponent>().Key)
                                return;
                        }
                        else return;
                        enemyFlag.Deliver();
                    }
                    else
                        enemyFlag.Capture(player.BattlePlayer);
                    break;
                case FlagState.Dropped:
                    if (flagEntity.GetComponent<TeamGroupComponent>().Key == tank.GetComponent<TeamGroupComponent>().Key)
                        allieFlag.Return(player.BattlePlayer);
                    else
                    {
                        // pickup flag
                        if (player.BattlePlayer.MatchPlayer.FlagBlocks > 0)
                        {
                            player.BattlePlayer.MatchPlayer.FlagBlocks -= 1;
                            return;
                        }
                        enemyFlag.Pickup(player.BattlePlayer);
                    }
                    break;
            }
        }
    }
}