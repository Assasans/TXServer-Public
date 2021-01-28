using System;
using System.Linq;
using System.Numerics;
using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Tank;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(-1910863908782544246L)]
    public class FlagDropRequestEvent : ECSEvent
    {
        public static void Execute(Player player, Entity flag, Entity tank)
        {
            Core.Battles.Battle battle = ServerConnection.BattlePool.Single(b => b.BattleEntity.GetComponent<BattleGroupComponent>().Key == tank.GetComponent<BattleGroupComponent>().Key);

            if (flag.GetComponent<FlagGroundedStateComponent>() == null)
            {

                battle.FlagBlockedTankKey = tank.GetComponent<TankGroupComponent>().Key;
                battle.DroppedFlags.Add(flag, DateTime.Now.AddMinutes(1));

                CommandManager.SendCommands(player,
                    new ComponentAddCommand(flag, new FlagGroundedStateComponent()),
                    // TODO: drop flag at latest tank position
                    new ComponentChangeCommand(flag, new FlagPositionComponent(new Vector3(x: 0, y: 3, z: 0))));
                CommandManager.BroadcastCommands(battle.RedTeamPlayers.Concat(battle.BlueTeamPlayers).Select(x => x.Player),
                    new SendEventCommand(new FlagDropEvent(IsUserAction: true), flag));
            }  
        }
    }
}