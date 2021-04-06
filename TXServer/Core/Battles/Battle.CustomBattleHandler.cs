using System;
using System.Linq;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.Events;

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
                        if (!Battle.AllBattlePlayers.Any())
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
                        if (!Battle.MatchPlayers.Any())
                        {
                            Battle.BattleLobbyEntity.RemoveComponent<BattleGroupComponent>();
                            Battle.BattleState = BattleState.CustomNotStarted;
                        }

                        if (Battle.CountdownTimer < 0)
                            Battle.FinishBattle();
                        break;
                }
            }

            public void OnPlayerAdded(BattlePlayer battlePlayer)
            {
            }

            public void OnPlayerRemoved(BattlePlayer battlePlayer)
            {
                if (battlePlayer.Player == Owner)
                {
                    if (Battle.AllBattlePlayers.Any())
                    {
                        var AllBattlePlayers = Battle.AllBattlePlayers.ToList();
                        _Owner = AllBattlePlayers[new Random().Next(AllBattlePlayers.Count)].Player;
                        Battle.BattleLobbyEntity.RemoveComponent<UserGroupComponent>();
                        Battle.BattleLobbyEntity.AddComponent(new UserGroupComponent(Owner.User));
                    }
                }
            }
        }
    }
}
