using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Team;
using TXServer.ECSSystem.Types;

namespace TXServer.Core.Battles
{
    public partial class Battle
    {
        public class CTFHandler : TeamBattleHandler
        {
            public Dictionary<Entity, Flag> Flags { get; private set; }

            public override TeamColor LosingTeam => (BlueTeamEntity.GetComponent<TeamScoreComponent>().Score - RedTeamEntity.GetComponent<TeamScoreComponent>().Score) switch
            {
                > 5 => TeamColor.RED,
                < -5 => TeamColor.BLUE,
                _ => TeamColor.NONE
            };

            public override BattleView BattleViewFor(BattleTankPlayer battlePlayer)
            {
                BattleView view = base.BattleViewFor(battlePlayer);
                view.AllyTeamFlag = Flags[battlePlayer.Team];
                view.EnemyTeamFlag = Flags.First(x => x.Key != battlePlayer.Team).Value;
                return view;
            }

            public override void SetupBattle()
            {
                base.SetupBattle();
                Flags = new()
                {
                    { RedTeamEntity, new Flag(Battle.CurrentMapInfo.Flags.Red, RedTeamEntity, Battle) },
                    { BlueTeamEntity, new Flag(Battle.CurrentMapInfo.Flags.Blue, BlueTeamEntity, Battle) }
                };
            }

            public override void CompleteWarmUp()
            {
                base.CompleteWarmUp();

                Battle.PlayersInMap.ShareEntities(Flags.Values.Select(x => x.FlagEntity));
                Battle.IsWarmUpCompleted = true;
            }

            public override void OnFinish()
            {
                base.OnFinish();

                foreach (KeyValuePair<Entity, Flag> flag in Flags)
                {
                    if (flag.Value.State == FlagState.Captured)
                        flag.Value.Drop(false, silent: true);
                }
            }

            public override void Tick()
            {
                base.Tick();

                if (Flags?.Values != null)
                    foreach (Flag flag in Flags.Values)
                        if (flag.State == FlagState.Dropped && DateTime.UtcNow > flag.ReturnStartTime)
                            flag.Return();
            }

            public override void OnMatchJoin(BaseBattlePlayer battlePlayer)
            {
                base.OnMatchJoin(battlePlayer);

                battlePlayer.ShareEntities(Flags.Select(p => p.Value.PedestalEntity));
                if (!Battle.IsMatchMaking || Battle.IsWarmUpCompleted)
                {
                    battlePlayer.ShareEntities(Flags.Select(f => f.Value.FlagEntity));
                }
            }

            public override void OnMatchLeave(BaseBattlePlayer battlePlayer)
            {
                base.OnMatchLeave(battlePlayer);

                battlePlayer.UnshareEntities(Flags.Select(p => p.Value.PedestalEntity));

                if (!Battle.IsMatchMaking || Battle.IsWarmUpCompleted)
                {
                    foreach (KeyValuePair<Entity, Flag> flag in Flags)
                    {
                        if (flag.Value.State != FlagState.Captured || flag.Value.Carrier != battlePlayer)
                            continue;
                        flag.Value.FlagEntity.PlayerReferences.Remove(battlePlayer.Player);
                        flag.Value.Drop(false);
                    }
                    battlePlayer.UnshareEntities(Flags.Select(f => f.Value.FlagEntity));
                }
            }
        }
    }
}
