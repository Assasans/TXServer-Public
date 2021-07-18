using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using TXServer.Library;
using TXServer.Core.HeightMaps;
using TXServer.Core.Logging;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.Events.Battle.Flag;
using TXServer.ECSSystem.Events.Battle.VisualScore;
using TXServer.ECSSystem.Events.Chat;
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
            FlagEntity = FlagTemplate.CreateEntity(position, team, battle.BattleEntity);

            State = FlagState.Home;
        }

        private void ReShare()
        {
            List<Player> refs = new(FlagEntity.PlayerReferences);
            refs.UnshareEntities(FlagEntity);
            refs.ShareEntities(FlagEntity);
        }

        public void Capture(BattleTankPlayer battlePlayer)
        {
            State = FlagState.Captured;
            Carrier = battlePlayer;
            Entity tank = Carrier.MatchPlayer.Tank;

            FlagEntity.AddComponent(new TankGroupComponent(tank));

            Battle.PlayersInMap.SendEvent(new FlagPickupEvent(), FlagEntity);
            CurrentAssists.Add(battlePlayer);

            FlagEntity.RemoveComponent<FlagHomeStateComponent>();

            battlePlayer.MatchPlayer.TryDeactivateInvisibility();
        }

        public void Pickup(BattleTankPlayer battlePlayer)
        {
            if (battlePlayer == LastCarrier && LastCarrierMinTime > DateTime.UtcNow) return;

            State = FlagState.Captured;
            Carrier = battlePlayer;
            Entity tank = Carrier.MatchPlayer.Tank;

            FlagEntity.AddComponent(new TankGroupComponent(tank));

            Battle.PlayersInMap.SendEvent(new FlagPickupEvent(), FlagEntity);
            if (!CurrentAssists.Contains(battlePlayer))
                CurrentAssists.Add(battlePlayer);

            FlagEntity.RemoveComponent<FlagGroundedStateComponent>();

            battlePlayer.MatchPlayer.TryDeactivateInvisibility();
        }

        public void Drop(bool isUserAction, bool silent = false)
        {
            LastCarrier = Carrier;
            LastCarrierMinTime = DateTime.UtcNow.AddSeconds(3);

            ReturnStartTime = DateTime.UtcNow.AddSeconds(60);
            State = FlagState.Dropped;

            Vector3 flagPosition;
            if (!Server.Instance.Settings.DisableHeightMaps)
            {
                HeightMap map = Battle.HeightMap;
                (HeightMapLayer layer, float y) = map.Layers
                    .Select((layer) =>
                    {
                        // If layer is not loaded
                        if (layer.Image == null) return (null, -9999);

                        // Get height map layer
                        Image<Rgb24> image = layer.Image;

                        // Map tank coordinates to layer size
                        float x = MathUtils.Map(Carrier.MatchPlayer.TankPosition.X, map.MinX, map.MaxX, 0, image.Width);
                        float z = MathUtils.Map(Carrier.MatchPlayer.TankPosition.Z, map.MinZ, map.MaxZ, 0, image.Height);

                        // Get pixel color
                        Rgb24 pixel = image[(int) Math.Round(x), (int) Math.Round(z)];

                        // Map pixel color to height
                        float y = MathUtils.Map(pixel.R, byte.MinValue, byte.MaxValue, layer.MinY, layer.MaxY);

                        //Logger.Log(
                            //$"{Carrier.MatchPlayer.TankPosition.X} {Carrier.MatchPlayer.TankPosition.Z} {Carrier.MatchPlayer.TankPosition.Y} | {Path.GetFileName(layer.Path)} | {Math.Round(x)} {Math.Round(z)} | {pixel.R} {pixel.G} {pixel.B} | {y}");

                        return (layer, y);
                    })
                    .First((tuple) => tuple.y - Vector3.UnitY.Y < Carrier.MatchPlayer.TankPosition.Y);

                if (layer == null)
                {
                    ChatMessageReceivedEvent.SystemMessageTarget(
                        $"[Flag] Dropped at {Carrier.MatchPlayer.TankPosition.X}, {Carrier.MatchPlayer.TankPosition.Z}, {Carrier.MatchPlayer.TankPosition.Y}\n" +
                        "[Flag/Error] No layer found",
                        Battle.GeneralBattleChatEntity, Carrier.Player
                    );
                    return;
                }

                flagPosition = new Vector3(Carrier.MatchPlayer.TankPosition.X, y, Carrier.MatchPlayer.TankPosition.Z) - Vector3.UnitY;
            }
            else
            {
                flagPosition = Carrier.MatchPlayer.TankPosition - Vector3.UnitY;
            }


            Carrier = null;

            if (!silent)
                Battle.PlayersInMap.SendEvent(new FlagDropEvent(isUserAction), FlagEntity);
            FlagEntity.RemoveComponent<TankGroupComponent>();

            FlagEntity.ChangeComponent(new FlagPositionComponent(flagPosition));
            FlagEntity.AddComponent(new FlagGroundedStateComponent());
        }

        public void Return(BattleTankPlayer battlePlayer = null, bool silent = false)
        {
            State = FlagState.Home;
            if (battlePlayer != null)
            {
                FlagEntity.AddComponent(new TankGroupComponent(battlePlayer.MatchPlayer.Tank));
                // todo: calculate flag return score
                battlePlayer.SendEvent(new VisualScoreFlagReturnEvent(battlePlayer.MatchPlayer.GetScoreWithBonus(5)), battlePlayer.MatchPlayer.BattleUser);
            }

            if (!silent)
                Battle.PlayersInMap.SendEvent(new FlagReturnEvent(), FlagEntity);

            if (battlePlayer != null)
            {
                battlePlayer.MatchPlayer.UserResult.FlagReturns += 1;
                FlagEntity.RemoveComponent<TankGroupComponent>();
            }

            FlagEntity.ChangeComponent(new FlagPositionComponent(BasePosition));
            FlagEntity.RemoveComponent<FlagGroundedStateComponent>();
            FlagEntity.AddComponent(new FlagHomeStateComponent());
            ReShare();

            LastCarrier = null;
        }

        public (BattleTankPlayer, IEnumerable<UserResult>) Deliver()
        {
            State = FlagState.Home;

            FlagEntity.AddComponent(new FlagHomeStateComponent());
            Battle.PlayersInMap.SendEvent(new FlagDeliveryEvent(), FlagEntity);
            FlagEntity.RemoveComponent<TankGroupComponent>();

            FlagEntity.ChangeComponent(new FlagPositionComponent(BasePosition));
            ReShare();

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


        public BattleTankPlayer Carrier;
        private readonly List<BattleTankPlayer> CurrentAssists = new();

        private BattleTankPlayer LastCarrier;
        private DateTime LastCarrierMinTime;
    }
}
