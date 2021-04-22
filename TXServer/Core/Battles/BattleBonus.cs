using System;
using System.Numerics;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.Types;
using TXServer.Core.ServerMapInformation;
using TXServer.ECSSystem.EntityTemplates.Battle.Bonus;
using System.Text.Json;
using System.IO;
using System.Linq;

namespace TXServer.Core.Battles
{
    public class BattleBonus
    {
        public BattleBonus(BonusType bonusType, Bonus bonus, Battle battle)
        {
            BattleBonusType = bonusType;
            Position = bonus.Position;
            HasParachute = bonus.HasParachute;
            BonusState = BonusState.Unused;
            if (!HasParachute) SpawnHeight = 0;
            if (battle.TypeHandler is not Battle.MatchMakingBattleHandler) GoldboxCrystals = 0;
            Battle = battle;
        }

        public void CreateRegion()
        {
            BonusState = BonusState.RegionShared;
            BonusRegion = BonusRegionTemplate.CreateEntity(BattleBonusType, Position);
            Battle.MatchPlayers.Select(x => x.Player).ShareEntity(BonusRegion);
        }

        public void CreateBonus(Entity battleEntity)
        {
            if (BattleBonusType != BonusType.GOLD)
                Bonus = SupplyBonusTemplate.CreateEntity(BattleBonusType, BonusRegion, new Vector3(Position.X, Position.Y + SpawnHeight, Position.Z), battleEntity);
            else
                Bonus = GoldBonusWithCrystalsTemplate.CreateEntity(BattleBonusType, BonusRegion, new Vector3(Position.X, Position.Y + SpawnHeight, Position.Z), battleEntity);
            Battle.MatchPlayers.Select(x => x.Player).ShareEntity(Bonus);
            BonusState = BonusState.Spawned;
        }

        public Entity BonusRegion { get; set; }
        public Entity Bonus { get; set; }
        public BonusType BattleBonusType { get; }
        private Vector3 Position { get; }
        private int SpawnHeight { get; } = 30;
        private bool HasParachute { get; }
        private Battle Battle { get; }

        private static int GoldboxCrystals { get; set; } = 1000;
        public int CurrentCrystals { get; set; } = GoldboxCrystals;

        public BonusState BonusState
        {
            get => _BonusState;
            set
            {
                _BonusState = value;

                switch (value)
                {
                    case BonusState.RegionShared:
                        if (BattleBonusType == BonusType.GOLD)
                            BonusStateChangeCountdown = 20;
                        break;
                    case BonusState.Redrop:
                        BonusStateChangeCountdown = 180;
                        break;
                    case BonusState.Unused:
                        CurrentCrystals = GoldboxCrystals;
                        break;
                }
            }
        }
        private BonusState _BonusState;
        public double BonusStateChangeCountdown { get; set; } = -1;
    }
}
