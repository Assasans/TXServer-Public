using System;
using System.Collections.Generic;
using System.Numerics;
using TXServer.Core.Battles.Effect;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;
using TXServer.ECSSystem.EntityTemplates.Battle.Bonus;
using TXServer.ECSSystem.Events.Battle.Bonus;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.Core.Battles
{
    public class BattleBonus
    {
        public BattleBonus(BonusType bonusType, ServerMapInformation.Bonus bonus, Battle battle)
        {
            BonusType = bonusType;
            Position = bonus.Position;
            HasParachute = bonus.HasParachute;
            State = BonusState.Unused;
            if (!HasParachute) SpawnHeight = 0;
            if (battle.TypeHandler is not Battle.MatchMakingBattleHandler) GoldboxCrystals = 0;
            Battle = battle;
        }

        public void CreateRegion()
        {
            State = BonusState.RegionShared;
            BonusRegion = BonusRegionTemplate.CreateEntity(BonusType, Position);
            Battle.PlayersInMap.ShareEntities(BonusRegion);
        }

        public void CreateBonus(Entity battleEntity)
        {
            BonusEntity = BonusType != BonusType.GOLD
                ? SupplyBonusTemplate.CreateEntity(BonusType, BonusRegion,
                    new Vector3(Position.X, Position.Y + SpawnHeight, Position.Z), battleEntity)
                : GoldBonusWithCrystalsTemplate.CreateEntity(BonusRegion,
                    new Vector3(Position.X, Position.Y + SpawnHeight, Position.Z), battleEntity);
            Battle.PlayersInMap.ShareEntities(BonusEntity);
            State = BonusState.Spawned;
        }

        public void Take(Player player)
        {
            BattleTankPlayer battlePlayer = player.BattlePlayer;

            Battle.PlayersInMap.SendEvent(new BonusTakenEvent(), BonusEntity);
            if (BonusType == BonusType.GOLD)
                Battle.PlayersInMap.SendEvent(new GoldTakenNotificationEvent(), battlePlayer.MatchPlayer.BattleUser);
            Battle.PlayersInMap.UnshareEntities(BonusEntity);

            State = BonusType == BonusType.GOLD ? BonusState.Unused : BonusState.ReDrop;
            battlePlayer.MatchPlayer.UserResult.BonusesTaken += 1;

            switch (BonusType)
            {
                case BonusType.GOLD:
                    player.Data.SetCrystals(player.Data.Crystals + CurrentCrystals);
                    Battle.PlayersInMap.UnshareEntities(BonusRegion);
                    break;
                default:
                    (Type, Entity) desc = _bonusToModule[BonusType];
                    if (!player.BattlePlayer.MatchPlayer.HasModule(desc.Item1, out BattleModule module))
                    {
                        module =
                            (BattleModule) Activator.CreateInstance(desc.Item1, player.BattlePlayer.MatchPlayer,
                                desc.Item2);
                        player.BattlePlayer.MatchPlayer.Modules.Add(module);
                    }
                    module.IsSupply = true;
                    module.Activate();
                    break;
            }
        }

        public Entity BonusRegion { get; set; }
        public Entity BonusEntity { get; set; }
        public BonusType BonusType { get; }
        private Vector3 Position { get; }
        private int SpawnHeight { get; } = 30;
        private bool HasParachute { get; }
        private Battle Battle { get; }

        private static int GoldboxCrystals { get; set; } = 1000;
        public int CurrentCrystals { get; set; } = GoldboxCrystals;

        public BonusState State
        {
            get => _state;
            set
            {
                _state = value;

                switch (value)
                {
                    case BonusState.RegionShared:
                        if (BonusType == BonusType.GOLD)
                            StateChangeCountdown = 20;
                        break;
                    case BonusState.ReDrop:
                        StateChangeCountdown = 180;
                        break;
                    case BonusState.Unused:
                        CurrentCrystals = GoldboxCrystals;
                        break;
                }
            }
        }
        private BonusState _state;
        public double StateChangeCountdown { get; set; } = -1;

        private readonly Dictionary<BonusType, (Type, Entity)> _bonusToModule = new()
        {
            { BonusType.ARMOR, (typeof(AbsorbingArmorEffect), Modules.GlobalItems.Absorbingarmor) },
            { BonusType.DAMAGE, (typeof(IncreasedDamageModule), Modules.GlobalItems.Increaseddamage) },
            { BonusType.REPAIR, (typeof(RepairKitModule), Modules.GlobalItems.Repairkit) },
            { BonusType.SPEED, (typeof(TurboSpeedModule), Modules.GlobalItems.Turbospeed) }
        };
    }
}
