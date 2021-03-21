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
            Team = team;
            TeamColor = team.GetComponent<TeamColorComponent>().TeamColor;
            Position = position;
            EnemyPlayers = enemyPlayers;
            Battle = battle;

            PedestalEntity = PedestalTemplate.CreateEntity(position, team, battle.BattleEntity);
            FlagEntity = FlagTemplate.CreateEntity(position, Team, Battle.BattleEntity);

            State = FlagState.Home;
        }

        private void Reshare()
        {
            List<Player> refs = new(FlagEntity.PlayerReferences);
            refs.UnshareEntity(FlagEntity);
            refs.ShareEntity(FlagEntity);
        }

        public void Capture(BattlePlayer battlePlayer)
        {
            State = FlagState.Captured;
            Carrier = battlePlayer;
            Entity tank = Carrier.MatchPlayer.Tank;

            FlagEntity.AddComponent(new TankGroupComponent(tank));

            Battle.MatchPlayers.Select(x => x.Player).SendEvent(new FlagPickupEvent(), FlagEntity);
            CurrentAssists.Add(battlePlayer);

            FlagEntity.RemoveComponent<FlagHomeStateComponent>();
        }

        public void Pickup(BattlePlayer battlePlayer)
        {
            State = FlagState.Captured;
            Carrier = battlePlayer;
            Entity tank = Carrier.MatchPlayer.Tank;

            FlagEntity.AddComponent(new TankGroupComponent(tank));

            Battle.MatchPlayers.Select(x => x.Player).SendEvent(new FlagPickupEvent(), FlagEntity);
            if (!CurrentAssists.Contains(battlePlayer))
                CurrentAssists.Add(battlePlayer);
            
            FlagEntity.RemoveComponent<FlagGroundedStateComponent>();
        }

        public void Drop(bool isUserAction)
        {
            ReturnStartTime = DateTime.Now.AddSeconds(60);
            State = FlagState.Dropped;

            Carrier.MatchPlayer.FlagBlocks = 3;
            Vector3 flagPosition = Carrier.MatchPlayer.TankPosition;
            Carrier = null;

            Battle.MatchPlayers.Select(x => x.Player).SendEvent(new FlagDropEvent(isUserAction), FlagEntity);
            FlagEntity.RemoveComponent<TankGroupComponent>();

            FlagEntity.ChangeComponent(new FlagPositionComponent(flagPosition));
            Reshare();
            FlagEntity.AddComponent(new FlagGroundedStateComponent());
        }

        public void Return(BattlePlayer battlePlayer = null)
        {
            State = FlagState.Home;
            if (battlePlayer != null)
                FlagEntity.AddComponent(new TankGroupComponent(battlePlayer.MatchPlayer.Tank));

            Battle.MatchPlayers.Select(x => x.Player).SendEvent(new FlagReturnEvent(), FlagEntity);

            if (battlePlayer != null)
            {
                battlePlayer.MatchPlayer.UserResult.FlagReturns += 1;
                FlagEntity.RemoveComponent<TankGroupComponent>();
            }

            FlagEntity.ChangeComponent(new FlagPositionComponent(Position));
            FlagEntity.RemoveComponent<FlagGroundedStateComponent>();
            FlagEntity.AddComponent(new FlagHomeStateComponent());
            Reshare();
        }

        public void Deliver()
        {
            State = FlagState.Home;

            Battle.MatchPlayers.Select(x => x.Player).SendEvent(new FlagDeliveryEvent(), FlagEntity);
            FlagEntity.RemoveComponent<TankGroupComponent>();

            FlagEntity.ChangeComponent(new FlagPositionComponent(Position));
            FlagEntity.AddComponent(new FlagGroundedStateComponent());
            FlagEntity.RemoveComponent<FlagGroundedStateComponent>();
            
            FlagEntity.AddComponent(new FlagHomeStateComponent());

            Battle.UpdateScore(Carrier.Team, 1);
            Battle.UpdateUserStatistics(Carrier.Player, additiveScore: EnemyPlayers.Count * 10, 0, 0, 0);
            CurrentAssists.Remove(Carrier);

            UserResult carrierResult = Carrier.MatchPlayer.UserResult;
            carrierResult.Flags += 1;

            foreach (UserResult assistResult in ((CTFHandler)Battle.ModeHandler).BattleViewFor(Carrier).AllyTeamResults)
            {
                if (CurrentAssists.Select(p => p.User.EntityId).Contains(assistResult.UserId))
                    assistResult.FlagAssists += 1;
            }
            CurrentAssists.Clear();

            Carrier = null;
        }

        public Entity PedestalEntity { get; }
        public Entity FlagEntity { get; }

        public Entity Team { get; }
        public TeamColor TeamColor { get; }

        private readonly Vector3 Position;
        public FlagState State { get; private set; }

        private readonly Battle Battle;
        private readonly IReadOnlyCollection<BattlePlayer> EnemyPlayers;

        public DateTime ReturnStartTime { get; set; }
        private readonly List<BattlePlayer> CurrentAssists = new();
        private BattlePlayer Carrier;
    }
}
