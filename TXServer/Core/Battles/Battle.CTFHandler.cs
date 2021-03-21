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

            public override BattleView BattleViewFor(BattlePlayer battlePlayer)
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
                    { RedTeamEntity, new Flag(Battle.CurrentMapInfo.Flags.Red.Position, RedTeamEntity, BlueTeamPlayers, Battle) },
                    { BlueTeamEntity, new Flag(Battle.CurrentMapInfo.Flags.Blue.Position, BlueTeamEntity, RedTeamPlayers, Battle) }
                };
            }

            public override void CompleteWarmUp()
            {
                base.CompleteWarmUp();

                Battle.MatchPlayers.Select(x => x.Player).ShareEntities(Flags.Values.Select(x => x.FlagEntity));
                Battle.IsWarmUpCompleted = true;
            }

            public override void Tick()
            {
                base.Tick();

                foreach (Flag flag in Flags.Values)
                {
                    if (flag.State == FlagState.Dropped && DateTime.Now > flag.ReturnStartTime)
                        flag.Return();
                }
            }

            public override void OnMatchJoin(BattlePlayer battlePlayer)
            {
                base.OnMatchJoin(battlePlayer);

                battlePlayer.Player.ShareEntities(Flags.Select(p => p.Value.PedestalEntity));
                if (!Battle.IsMatchMaking || Battle.IsWarmUpCompleted)
                {
                    battlePlayer.Player.ShareEntities(Flags.Select(f => f.Value.FlagEntity));
                }
            }

            public override void OnMatchLeave(BattlePlayer battlePlayer)
            {
                base.OnMatchLeave(battlePlayer);

                battlePlayer.Player.UnshareEntities(Flags.Select(p => p.Value.PedestalEntity));

                if (!Battle.IsMatchMaking || Battle.IsWarmUpCompleted)
                {
                    foreach (KeyValuePair<Entity, Flag> flag in Flags)
                    {
                        if (flag.Value.State != FlagState.Captured)
                            continue;
                        flag.Value.FlagEntity.PlayerReferences.Remove(battlePlayer.Player);
                        flag.Value.Drop(false);
                    }
                    battlePlayer.Player.UnshareEntities(Flags.Select(f => f.Value.FlagEntity));
                }
            }
        }
    }
}
