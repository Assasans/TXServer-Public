using System.Numerics;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.Types;
using TXServer.Core.ServerMapInformation;
using TXServer.ECSSystem.EntityTemplates.Battle.Bonus;
using System.Text.Json;
using System.IO;

namespace TXServer.Core.Battles
{
    public class BattleBonus
    {
        public BattleBonus(BonusType bonusType, Bonus bonus)
        {
            BattleBonusType = bonusType;
            Position = bonus.Position;
            Number = bonus.Number;
            HasParachute = bonus.HasParachute;
            BonusState = BonusState.Unused;

            BonusRegion = BonusRegionTemplate.CreateEntity(bonusType, Position);
        }
        public void CreateBonus(Entity battleEntity)
        {
            int spawnHeight = 30;
            if (!HasParachute)
                spawnHeight = 0;

            if (BattleBonusType != BonusType.GOLD)
                Bonus = SupplyBonusTemplate.CreateEntity(BattleBonusType, BonusRegion, new Vector3(Position.X, Position.Y + spawnHeight, Position.Z), battleEntity);
            else
                Bonus = GoldBonusWithCrystalsTemplate.CreateEntity(BattleBonusType, BonusRegion, new Vector3(Position.X, Position.Y + spawnHeight, Position.Z), battleEntity);
        }

        public Entity BonusRegion { get; set; }
        public Entity Bonus { get; set; }
        public BonusType BattleBonusType { get; set; }
        public Vector3 Position { get; set; }
        public bool HasParachute { get; set; }
        public int Number { get; set; }
        public int GoldboxCrystals { get; } = 1000;

        public BonusState BonusState
        {
            get => _BonusState;
            set
            {
                if (value != BonusState.New)
                {
                    if (_BonusState == BonusState.New)
                    {
                    }
                    else
                    {
                    }
                }
                _BonusState = value;

                switch (value)
                {
                    case BonusState.New:
                        if (BattleBonusType == BonusType.GOLD)
                            BonusStateChangeCountdown = 20;
                        break;
                    case BonusState.Redrop:
                        BonusStateChangeCountdown = 180;
                        break;
                    default:
                        BonusStateChangeCountdown = 0;
                        break;
                }
            }
        }
        private BonusState _BonusState;
        public double BonusStateChangeCountdown { get; set; }
    }
}
