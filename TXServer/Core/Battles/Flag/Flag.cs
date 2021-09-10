using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using TXServer.Core.HeightMaps;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.Events.Battle.Flag;
using TXServer.ECSSystem.Events.Battle.VisualScore;
using TXServer.ECSSystem.Types;
using TXServer.Library;
using static TXServer.Core.Battles.Battle;

namespace TXServer.Core.Battles
{
    public class Flag
    {
        public Flag(Vector3 position, Entity team, Battle battle)
        {
            Team = team;
            _basePosition = position;
            _battle = battle;

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
            Entity tank = Carrier.MatchPlayer.TankEntity;

            FlagEntity.AddComponent(new TankGroupComponent(tank));

            _battle.PlayersInMap.SendEvent(new FlagPickupEvent(), FlagEntity);
            CurrentAssists.Add(battlePlayer);

            FlagEntity.RemoveComponent<FlagHomeStateComponent>();

            battlePlayer.MatchPlayer.TryDeactivateInvisibility();
        }

        public void Drop(bool isUserAction, bool silent = false)
        {
            LastCarrier = Carrier;
            LastCarrierMinTime = DateTime.UtcNow.AddSeconds(3);

            ReturnStartTime = DateTime.UtcNow.AddSeconds(60);

            Vector3 flagPosition;
            if (!Server.Instance.Settings.DisableHeightMaps)
            {
                HeightMap map = _battle.HeightMap;
                (HeightMapLayer layer, float newHeight) tuple = map.Layers
                    .Select(layer =>
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
                        float newHeight = MathUtils.Map(pixel.R, byte.MinValue, byte.MaxValue, layer.MinY, layer.MaxY);

                        //Logger.Log(
                            //$"{Carrier.MatchPlayer.TankPosition.X} {Carrier.MatchPlayer.TankPosition.Z} {Carrier.MatchPlayer.TankPosition.Y} | {Path.GetFileName(layer.Path)} | {Math.Round(x)} {Math.Round(z)} | {pixel.R} {pixel.G} {pixel.B} | {y}");

                        return (layer, newHeight);
                    })
                    .FirstOrDefault(t => t.newHeight - Vector3.UnitY.Y < Carrier.MatchPlayer.TankPosition.Y);

                if (tuple == default)
                {
                    Carrier = null;
                    State = FlagState.Dropped;

                    FlagEntity.RemoveComponent<TankGroupComponent>();
                    FlagEntity.AddComponent(new FlagGroundedStateComponent());
                    if (!silent)
                        _battle.PlayersInMap.SendEvent(new FlagDropEvent(isUserAction), FlagEntity);

                    Return();
                    return;
                }

                (HeightMapLayer _, float y) = tuple;
                flagPosition = new Vector3(Carrier.MatchPlayer.TankPosition.X, y + 0.9f,
                                   Carrier.MatchPlayer.TankPosition.Z) - Vector3.UnitY;
            }
            else
                flagPosition = Carrier.MatchPlayer.TankPosition - Vector3.UnitY;

            // prevent glitch if flag state has changed in the meantime
            if (State != FlagState.Captured) return;

            Carrier = null;
            State = FlagState.Dropped;

            if (!silent)
                _battle.PlayersInMap.SendEvent(new FlagDropEvent(isUserAction), FlagEntity);
            FlagEntity.RemoveComponent<TankGroupComponent>();

            FlagEntity.ChangeComponent(new FlagPositionComponent(flagPosition));
            FlagEntity.AddComponent(new FlagGroundedStateComponent());
        }

        public void KillDrop(BattleTankPlayer battlePlayer)
        {
            bool inKillzone = _battle.Params.KillZoneEnabled && _battle.CurrentMapInfo.PuntativeGeoms.Any(geometry =>
                PositionFormulas.IsInsideBox(Carrier.MatchPlayer.TankPosition, geometry.Position, geometry.Size));
            bool outOfBounds = !battlePlayer.Player.Server.Settings.MapBoundsInactive &&
                               PositionFormulas.CheckOverflow(battlePlayer.MatchPlayer.TankPosition);

            if (inKillzone || outOfBounds)
            {
                Drop(false, true);
                Return();
            }
            else Drop(false);
        }

        public void Pickup(BattleTankPlayer battlePlayer)
        {
            if (battlePlayer == LastCarrier && LastCarrierMinTime > DateTime.UtcNow ||
                State == FlagState.Captured) return;

            State = FlagState.Captured;
            Carrier = battlePlayer;
            Entity tank = Carrier.MatchPlayer.TankEntity;

            FlagEntity.AddComponent(new TankGroupComponent(tank));

            _battle.PlayersInMap.SendEvent(new FlagPickupEvent(), FlagEntity);
            if (!CurrentAssists.Contains(battlePlayer))
                CurrentAssists.Add(battlePlayer);

            FlagEntity.RemoveComponent<FlagGroundedStateComponent>();

            battlePlayer.MatchPlayer.TryDeactivateInvisibility();
        }

        public void Return(BattleTankPlayer battlePlayer = null, bool silent = false)
        {
            State = FlagState.Home;
            if (battlePlayer != null)
            {
                FlagEntity.AddComponent(new TankGroupComponent(battlePlayer.MatchPlayer.TankEntity));
                // todo: calculate flag return score
                battlePlayer.SendEvent(new VisualScoreFlagReturnEvent(battlePlayer.MatchPlayer.GetScoreWithBonus(5)), battlePlayer.MatchPlayer.BattleUser);
            }

            if (!silent)
                _battle.PlayersInMap.SendEvent(new FlagReturnEvent(), FlagEntity);

            if (battlePlayer != null)
            {
                battlePlayer.MatchPlayer.UserResult.FlagReturns += 1;
                FlagEntity.RemoveComponent<TankGroupComponent>();
            }

            FlagEntity.ChangeComponent(new FlagPositionComponent(_basePosition));
            FlagEntity.RemoveComponent<FlagGroundedStateComponent>();
            FlagEntity.AddComponent(new FlagHomeStateComponent());
            ReShare();

            LastCarrier = null;
        }

        public (BattleTankPlayer, IEnumerable<UserResult>) Deliver()
        {
            State = FlagState.Home;

            FlagEntity.AddComponent(new FlagHomeStateComponent());
            _battle.PlayersInMap.SendEvent(new FlagDeliveryEvent(), FlagEntity);
            FlagEntity.RemoveComponent<TankGroupComponent>();

            FlagEntity.ChangeComponent(new FlagPositionComponent(_basePosition));
            ReShare();

            foreach (UserResult assistResult in ((CTFHandler)_battle.ModeHandler).BattleViewFor(Carrier).AllyTeamResults)
            {
                if (assistResult.UserId != Carrier.User.EntityId) continue;
                if (CurrentAssists.Select(p => p.User.EntityId).Contains(assistResult.UserId))
                    assistResult.FlagAssists += 1;
            }

            var carrier = Carrier;
            Carrier = null;
            LastCarrier = null;

            _battle.TriggerRandomGoldbox();

            return (carrier, GetAndClearAssistResults());

            IEnumerable<UserResult> GetAndClearAssistResults()
            {
                foreach (UserResult assistResult in ((CTFHandler)_battle.ModeHandler).BattleViewFor(carrier).AllyTeamResults)
                {
                    if (assistResult.UserId != carrier.User.EntityId) continue;
                    if (CurrentAssists.Select(p => p.User.EntityId).Contains(assistResult.UserId))
                        yield return assistResult;
                }
                CurrentAssists.Clear();
            }
        }

        private readonly Battle _battle;
        public Entity PedestalEntity { get; }
        public Entity FlagEntity { get; }
        public readonly Vector3 _basePosition;

        public Entity Team { get; }

        public FlagState State { get; private set; }
        public DateTime ReturnStartTime { get; private set; }


        public BattleTankPlayer Carrier;
        private List<BattleTankPlayer> CurrentAssists { get; } = new();
        private BattleTankPlayer LastCarrier { get; set; }
        private DateTime LastCarrierMinTime { get; set; }
    }
}
