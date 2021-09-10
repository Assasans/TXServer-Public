using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.Events;
using TXServer.ECSSystem.Types;

namespace TXServer.Core.Battles
{
    public partial class Battle
    {
        public class CustomBattleHandler : IBattleTypeHandler
        {
            public Battle Battle { get; init; }

            public Player Owner
            {
                get => _Owner;
                init => _Owner = value;
            }
            private Player _Owner;

            public bool IsOpen { get; private set; }
            public bool HackBattle { get; set; }
            public bool HackBattleDemocracy { get; set; } = true;

            public void OpenBattle()
            {
                IsOpen = true;
                Battle.BattleLobbyEntity.AddComponent(new OpenToConnectLobbyComponent());
            }

            public void SetupBattle()
            {
                (Battle.MapEntity, _) = Battle.ConvertMapParams(Battle.Params, Battle.IsMatchMaking);
                Battle.BattleLobbyEntity = CustomBattleLobbyTemplate.CreateEntity(Battle.Params, Battle.MapEntity, Battle.GravityTypes[Battle.Params.Gravity], Owner);

                Battle.BattleState = BattleState.CustomNotStarted;
            }

            public void Tick()
            {
                switch (Battle.BattleState)
                {
                    case BattleState.Starting:
                        if (!Battle.JoinedTankPlayers.Any())
                        {
                            Battle.BattleState = BattleState.CustomNotStarted;
                            break;
                        }

                        if (Battle.CountdownTimer < 0)
                        {
                            Battle.StartBattle();
                            Battle.BattleLobbyEntity.AddComponent(Battle.BattleEntity.GetComponent<BattleGroupComponent>());
                            Battle.BattleState = BattleState.Running;
                        }
                        break;
                    case BattleState.Running:
                        if (!Battle.MatchTankPlayers.Any() && !Battle.KeepRunning)
                        {
                            Battle.BattleLobbyEntity.RemoveComponent<BattleGroupComponent>();
                            Battle.ModeHandler.ResetScore();
                            Battle.BattleState = BattleState.CustomNotStarted;
                        }

                        if (Battle.CountdownTimer < 0)
                            Battle.FinishBattle();
                        break;
                }
            }

            public void OnPlayerAdded(BattleTankPlayer battlePlayer)
            {
            }

            public void OnPlayerRemoved(BattleTankPlayer battlePlayer)
            {
                if (battlePlayer.Player == Owner)
                {
                    if (Battle.JoinedTankPlayers.All(p => p == battlePlayer)) return;

                    var allBattlePlayers = Battle.JoinedTankPlayers.Where(p => p != battlePlayer).ToList();
                    _Owner = allBattlePlayers[new Random().Next(allBattlePlayers.Count)].Player;
                    Battle.BattleLobbyEntity.RemoveComponent<UserGroupComponent>();
                    Battle.BattleLobbyEntity.AddComponent(new UserGroupComponent(Owner.User));
                }
            }
        }
    }
}
