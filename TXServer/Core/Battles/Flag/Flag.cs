﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.Events.Battle.VisualScore;
using TXServer.ECSSystem.Types;
using static TXServer.Core.Battles.Battle;

namespace TXServer.Core.Battles
{
    public class Flag
    {
        public Flag(Vector3 position, Entity team, Battle battle)
        {
            Team = team;
            TeamColor = team.GetComponent<TeamColorComponent>().TeamColor;
            BasePosition = position;
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
            if (battlePlayer == LastCarrier && LastCarrierMinTime > DateTime.Now) return;

            State = FlagState.Captured;
            Carrier = battlePlayer;
            Entity tank = Carrier.MatchPlayer.Tank;

            FlagEntity.AddComponent(new TankGroupComponent(tank));

            Battle.MatchPlayers.Select(x => x.Player).SendEvent(new FlagPickupEvent(), FlagEntity);
            if (!CurrentAssists.Contains(battlePlayer))
                CurrentAssists.Add(battlePlayer);
            
            FlagEntity.RemoveComponent<FlagGroundedStateComponent>();
        }

        public void Drop(bool isUserAction, bool silent = false)
        {
            LastCarrier = Carrier;
            LastCarrierMinTime = DateTime.Now.AddSeconds(3);

            ReturnStartTime = DateTime.Now.AddSeconds(60);
            State = FlagState.Dropped;

            Vector3 flagPosition = Carrier.MatchPlayer.TankPosition - Vector3.UnitY;
            Carrier = null;

            if (!silent)
                Battle.MatchPlayers.Select(x => x.Player).SendEvent(new FlagDropEvent(isUserAction), FlagEntity);
            FlagEntity.RemoveComponent<TankGroupComponent>();

            FlagEntity.ChangeComponent(new FlagPositionComponent(flagPosition));
            FlagEntity.AddComponent(new FlagGroundedStateComponent());
        }

        public void Return(BattlePlayer battlePlayer = null, bool silent = false)
        {
            State = FlagState.Home;
            if (battlePlayer != null)
            {
                FlagEntity.AddComponent(new TankGroupComponent(battlePlayer.MatchPlayer.Tank));
                // todo: calculate flag return score
                battlePlayer.Player.SendEvent(new VisualScoreFlagReturnEvent(battlePlayer.MatchPlayer.GetScoreWithPremium(5)), battlePlayer.MatchPlayer.BattleUser);
            } 

            if (!silent)
                Battle.MatchPlayers.Select(x => x.Player).SendEvent(new FlagReturnEvent(), FlagEntity);

            if (battlePlayer != null)
            {
                battlePlayer.MatchPlayer.UserResult.FlagReturns += 1;
                FlagEntity.RemoveComponent<TankGroupComponent>();
            }

            FlagEntity.ChangeComponent(new FlagPositionComponent(BasePosition));
            FlagEntity.RemoveComponent<FlagGroundedStateComponent>();
            FlagEntity.AddComponent(new FlagHomeStateComponent());
            Reshare();

            LastCarrier = null;
        }

        public (BattlePlayer, IEnumerable<UserResult>) Deliver()
        {
            State = FlagState.Home;
            
            FlagEntity.RemoveComponent<TankGroupComponent>();

            FlagEntity.ChangeComponent(new FlagPositionComponent(BasePosition));
            FlagEntity.AddComponent(new FlagGroundedStateComponent());
            FlagEntity.RemoveComponent<FlagGroundedStateComponent>();
            
            FlagEntity.AddComponent(new FlagHomeStateComponent());
            Battle.MatchPlayers.Select(x => x.Player).SendEvent(new FlagDeliveryEvent(), FlagEntity);
            Reshare();

            foreach (UserResult assistResult in ((CTFHandler)Battle.ModeHandler).BattleViewFor(Carrier).AllyTeamResults)
            {
                if (assistResult.UserId != Carrier.User.EntityId) continue;
                if (CurrentAssists.Select(p => p.User.EntityId).Contains(assistResult.UserId))
                    assistResult.FlagAssists += 1;
            }

            var carrier = Carrier;
            Carrier = null;

            LastCarrier = null;

            return (carrier, getAndClearAssistResults());

            IEnumerable<UserResult> getAndClearAssistResults()
            {
                foreach (UserResult assistResult in ((CTFHandler)Battle.ModeHandler).BattleViewFor(carrier).AllyTeamResults)
                {
                    if (assistResult.UserId != carrier.User.EntityId) continue;
                    if (CurrentAssists.Select(p => p.User.EntityId).Contains(assistResult.UserId))
                        yield return assistResult;
                }
                CurrentAssists.Clear();
            }
        }

        private readonly Battle Battle;
        public Entity PedestalEntity { get; }
        public Entity FlagEntity { get; }
        private readonly Vector3 BasePosition;

        public Entity Team { get; }
        public TeamColor TeamColor { get; }
        
        public FlagState State { get; private set; }
        public DateTime ReturnStartTime { get; set; }


        public BattlePlayer Carrier;
        private readonly List<BattlePlayer> CurrentAssists = new();

        private BattlePlayer LastCarrier;
        private DateTime LastCarrierMinTime;
    }
}