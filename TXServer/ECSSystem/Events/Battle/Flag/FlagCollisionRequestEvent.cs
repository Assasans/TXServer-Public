using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.Core;
using System.Linq;
using TXServer.ECSSystem.Components.Battle.Team;
using TXServer.ECSSystem.Components.Battle;
using TXServer.Core.Battles;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Tank;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(1463741053998L)]
    public class FlagCollisionRequestEvent : ECSEvent
    {
        public static void Execute(Player player, Entity tank, Entity flagEntity)
        {
            Core.Battles.Battle battle = ServerConnection.BattlePool.Single(b => b.BattleEntity.GetComponent<BattleGroupComponent>().Key == tank.GetComponent<BattleGroupComponent>().Key);
            Flag collisedFlag = battle.Flags.Values.Single(f => f.FlagEntity.EntityId == flagEntity.EntityId);
            Flag enemyFlag = battle.Flags.Values.Single(f => f.TeamColor != player.User.GetComponent<TeamColorComponent>().TeamColor);
            Flag allieFlag = battle.Flags.Values.Single(f => f.TeamColor == player.User.GetComponent<TeamColorComponent>().TeamColor);

            if (battle.BattleState == BattleState.WarmUp)
                return;

            switch (collisedFlag.FlagState)
            {
                case FlagState.Home:
                    if (flagEntity.GetComponent<TeamGroupComponent>().Key == tank.GetComponent<TeamGroupComponent>().Key)
                    {
                        if (enemyFlag.FlagEntity.GetComponent<TankGroupComponent>() == null)
                            return;
                        if (enemyFlag.FlagState == FlagState.Captured)
                        {
                            if (enemyFlag.FlagEntity.GetComponent<TankGroupComponent>().Key != tank.GetComponent<TankGroupComponent>().Key)
                                return;
                        }
                        else return;
                        enemyFlag.DeliverFlag(player, tank, allieFlag);
                    }
                    else
                        enemyFlag.CaptureFlag(player, tank);
                    break;
                case FlagState.Dropped:
                    if (flagEntity.GetComponent<TeamGroupComponent>().Key == tank.GetComponent<TeamGroupComponent>().Key)
                        allieFlag.ReturnFlag(player, tank);
                    else
                    {
                        if (player.BattleLobbyPlayer.BattlePlayer.FlagBlocks > 0)
                        {
                            player.BattleLobbyPlayer.BattlePlayer.FlagBlocks -= 1;
                            return;
                        }
                        enemyFlag.PickupFlag(player, tank);
                    }
                    break;
            }
        }
    }
}