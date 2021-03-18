using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.Types;
using static TXServer.Core.Battles.Battle;

namespace TXServer.Core.Battles
{
    public class Flag
    {
        public Flag(Vector3 position, Entity team, List<BattlePlayer> enemyPlayers, Battle battle)
        {
            PedestalEntity = PedestalTemplate.CreateEntity(position, team, battle.BattleEntity);
            Team = team;
            TeamColor = team.GetComponent<TeamColorComponent>().TeamColor;
            Position = position;
            EnemyPlayers = enemyPlayers;
            Battle = battle;

            Prepare();
        }

        private void Prepare()
        {
            FlagEntity = FlagTemplate.CreateEntity(Position, Team, Battle.BattleEntity);

            CurrentAssists.Clear();
            FlagState = FlagState.Home;
        }

        private void Recreate()
        {
            Battle.MatchPlayers.Select(x => x.Player).UnshareEntity(FlagEntity);
            Prepare();
            Battle.MatchPlayers.Select(x => x.Player).ShareEntity(FlagEntity);
        }

        public void Capture(Player player, Entity tank)
        {
            FlagState = FlagState.Captured;
            FlagEntity.AddComponent(new TankGroupComponent(tank));
            FlagEntity.RemoveComponent<FlagHomeStateComponent>();
            Battle.MatchPlayers.Select(x => x.Player).SendEvent(new FlagPickupEvent(), FlagEntity);
            CurrentAssists.Add(player.BattlePlayer);
        }

        public void Pickup(Player player, Entity tank)
        {
            FlagState = FlagState.Captured;
            FlagEntity.RemoveComponent<TankGroupComponent>();
            FlagEntity.AddComponent(new TankGroupComponent(tank));
            FlagEntity.RemoveComponent<FlagGroundedStateComponent>();
            Battle.MatchPlayers.Select(x => x.Player).SendEvent(new FlagPickupEvent(), FlagEntity);
            if (!CurrentAssists.Contains(player.BattlePlayer))
                CurrentAssists.Add(player.BattlePlayer);
        }

        public void Drop(Player player, bool isUserAction)
        {
            ReturnStartTime = DateTime.Now.AddSeconds(60);
            FlagState = FlagState.Dropped;
            player.BattlePlayer.MatchPlayer.FlagBlocks = 3;
            Vector3 flagPosition = new(player.BattlePlayer.MatchPlayer.TankPosition.X, player.BattlePlayer.MatchPlayer.TankPosition.Y - 1,
                player.BattlePlayer.MatchPlayer.TankPosition.Z);
            FlagEntity.AddComponent(new FlagGroundedStateComponent());
            FlagEntity.ChangeComponent(new FlagPositionComponent(flagPosition));
            Battle.MatchPlayers.Select(x => x.Player).SendEvent(new FlagDropEvent(IsUserAction: isUserAction), FlagEntity);
        }

        public void Return(Player player, Entity tank)
        {
            FlagEntity.RemoveComponent<TankGroupComponent>();
            if (tank != null)
                FlagEntity.AddComponent(new TankGroupComponent(tank));
            FlagEntity.RemoveComponent<FlagGroundedStateComponent>();
            Battle.MatchPlayers.Select(x => x.Player).SendEvent(new FlagReturnEvent(), FlagEntity);
            Recreate();

            if (player != null)
            {
                player.BattlePlayer.MatchPlayer.UserResult.FlagReturns += 1;
            }
        }

        public void Deliver(Player player, Entity tank, Flag allieFlag)
        {
            FlagEntity.ChangeComponent(new FlagPositionComponent(allieFlag.Position));
            FlagEntity.AddComponent(new FlagGroundedStateComponent());
            Battle.MatchPlayers.Select(x => x.Player).SendEvent(new FlagDeliveryEvent(), FlagEntity);
            Recreate();

            Battle.UpdateScore(player, allieFlag.Team, 1);
            Battle.UpdateUserStatistics(player, additiveScore: allieFlag.EnemyPlayers.Count * 10, 0, 0, 0);
            CurrentAssists.Remove(player.BattlePlayer);
            UserResult delivererResult = player.BattlePlayer.MatchPlayer.UserResult;
            delivererResult.Flags += 1;

            foreach (UserResult assistResult in ((CTFHandler)Battle.ModeHandler).BattleViewFor(player.BattlePlayer).AllyTeamResults)
            {
                if (CurrentAssists.Select(p => p.User.EntityId).Contains(assistResult.UserId))
                    assistResult.FlagAssists += 1;
            }
        }

        public Entity PedestalEntity { get; }
        public Entity FlagEntity { get; set; }
        public Entity Team { get; set; }
        public TeamColor TeamColor { get; }
        public Vector3 Position { get; }
        public FlagState FlagState { get; set; }
        public List<BattlePlayer> EnemyPlayers { get; set; }
        public Battle Battle { get; }
        public DateTime ReturnStartTime { get; set; }
        private List<BattlePlayer> CurrentAssists { get; } = new();
    }
}
