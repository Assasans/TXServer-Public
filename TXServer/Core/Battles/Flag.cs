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

namespace TXServer.Core.Battles
{
    public class Flag
    {
        public Flag(Vector3 position, Entity team, List<BattleLobbyPlayer> enemyPlayers, Battle battle)
        {
            PedestalEntity = PedestalTemplate.CreateEntity(position, team, battle.BattleEntity);
            FlagEntity = FlagTemplate.CreateEntity(position, team, battle.BattleEntity);
            Team = team;
            TeamColor = team.GetComponent<TeamColorComponent>().TeamColor;
            Position = position;
            FlagState = FlagState.Home;
            EnemyPlayers = enemyPlayers;
            Battle = battle;
        }

        public void RecreateFlag()
        {
            FlagEntity = FlagTemplate.CreateEntity(Position, Team, Battle.BattleEntity);
            FlagReturnCountdown = 60;
            CurrentAssists.Clear();
        }

        public void CaptureFlag(Player player, Entity tank)
        {
            FlagState = FlagState.Captured;
            FlagEntity.AddComponent(new TankGroupComponent(tank));
            FlagEntity.RemoveComponent<FlagHomeStateComponent>();
            Battle.MatchPlayers.Select(x => x.Player).SendEvent(new FlagPickupEvent(), FlagEntity);
            CurrentAssists.Add(player.BattleLobbyPlayer);
        }

        public void PickupFlag(Player player, Entity tank)
        {
            FlagState = FlagState.Captured;
            FlagEntity.RemoveComponent<TankGroupComponent>();
            FlagEntity.AddComponent(new TankGroupComponent(tank));
            FlagEntity.RemoveComponent<FlagGroundedStateComponent>();
            Battle.MatchPlayers.Select(x => x.Player).SendEvent(new FlagPickupEvent(), FlagEntity);
            if (!CurrentAssists.Contains(player.BattleLobbyPlayer))
                CurrentAssists.Add(player.BattleLobbyPlayer);
        }

        public void DropFlag(Player player, bool isUserAction)
        {
            FlagReturnCountdown = 60;
            FlagState = FlagState.Dropped;
            player.BattleLobbyPlayer.BattlePlayer.FlagBlocks = 3;
            Vector3 flagPosition = new(player.BattleLobbyPlayer.BattlePlayer.TankPosition.X, player.BattleLobbyPlayer.BattlePlayer.TankPosition.Y - 1,
                player.BattleLobbyPlayer.BattlePlayer.TankPosition.Z);
            FlagEntity.AddComponent(new FlagGroundedStateComponent());
            FlagEntity.ChangeComponent(new FlagPositionComponent(flagPosition));
            Battle.MatchPlayers.Select(x => x.Player).SendEvent(new FlagDropEvent(IsUserAction: isUserAction), FlagEntity);
        }

        public void ReturnFlag(Player player, Entity tank)
        {
            FlagEntity.RemoveComponent<TankGroupComponent>();
            if (tank != null)
                FlagEntity.AddComponent(new TankGroupComponent(tank));
            FlagEntity.RemoveComponent<FlagGroundedStateComponent>();
            Battle.MatchPlayers.Select(x => x.Player).SendEvent(new FlagReturnEvent(), FlagEntity);
            Battle.MatchPlayers.Select(x => x.Player).UnshareEntity(FlagEntity);
            RecreateFlag();
            FlagState = FlagState.Home;
            Battle.MatchPlayers.Select(x => x.Player).ShareEntity(FlagEntity);

            if (player != null)
            {
                UserResult returnerResult = Battle.AllUserResults.Single(p => p.UserId == player.User.EntityId);
                returnerResult.FlagReturns += 1;
            }
        }

        public void DeliverFlag(Player player, Entity tank, Flag allieFlag)
        {
            FlagEntity.ChangeComponent(new FlagPositionComponent(allieFlag.Position));
            FlagEntity.AddComponent(new FlagGroundedStateComponent());
            Battle.MatchPlayers.Select(x => x.Player).SendEvent(new FlagDeliveryEvent(), FlagEntity);
            Battle.MatchPlayers.Select(x => x.Player).UnshareEntity(FlagEntity);
            RecreateFlag();
            FlagState = FlagState.Home;
            Battle.MatchPlayers.Select(x => x.Player).ShareEntity(FlagEntity);

            Battle.UpdateScore(player, allieFlag.Team, 1);
            Battle.UpdateUserStatistics(player, additiveScore: allieFlag.EnemyPlayers.Count * 10, 0, 0, 0);
            CurrentAssists.Remove(player.BattleLobbyPlayer);
            UserResult delivererResult = Battle.AllUserResults.Single(p => p.UserId == player.User.EntityId);
            delivererResult.Flags += 1;
            foreach (UserResult assistResult in Battle.AllUserResults)
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
        public List<BattleLobbyPlayer> EnemyPlayers { get; set; }
        public Battle Battle { get; }
        public double FlagReturnCountdown { get; set; } = 60;
        private List<BattleLobbyPlayer> CurrentAssists { get; } = new();
}
}
