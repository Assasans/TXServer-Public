using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Team;
using TXServer.ECSSystem.Types;
using static TXServer.Core.Battles.Battle;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(1463741053998L)]
    public class FlagCollisionRequestEvent : ECSEvent
    {
        public static void Execute(Player player, Entity tank, Entity flagEntity)
        {
            Core.Battles.Battle battle = player.BattlePlayer.Battle;
            var handler = (Core.Battles.Battle.CTFHandler)battle.ModeHandler;

            Flag flag = handler.Flags.Values.First(x => x.FlagEntity == flagEntity);

            BattleView view = handler.BattleViewFor(player.BattlePlayer);
            
            if (battle.BattleState == BattleState.WarmUp)
                return;

            switch (flag.State)
            {
                case FlagState.Home:
                    if (flagEntity.GetComponent<TeamGroupComponent>().Key == tank.GetComponent<TeamGroupComponent>().Key)
                    {
                        Flag carriedFlag = handler.Flags.Values.First(x => x != flag);

                        if (carriedFlag.State != FlagState.Captured) return;
                        if (carriedFlag.FlagEntity.GetComponent<TankGroupComponent>()?.Key != tank.EntityId) return;

                        (BattlePlayer carrier, IEnumerable<UserResult> assistResults) = carriedFlag.Deliver();
                        battle.UpdateScore(carrier.Team, 1);
                        battle.UpdateUserStatistics(carrier.Player, additiveScore: view.EnemyTeamPlayers.Count * 10, 0, 0, 0);

                        UserResult carrierResult = carrier.MatchPlayer.UserResult;
                        carrierResult.Flags += 1;

                        foreach (UserResult assistResult in assistResults)
                            assistResult.FlagAssists += 1;
                    }
                    else
                    {
                        flag.Capture(player.BattlePlayer);
                    }
                    break;
                case FlagState.Dropped:
                    if (flagEntity.GetComponent<TeamGroupComponent>().Key == tank.GetComponent<TeamGroupComponent>().Key)
                        flag.Return(player.BattlePlayer);
                    else
                        flag.Pickup(player.BattlePlayer);
                    break;
            }
        }
    }
}