using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TXServer.Core.Commands;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.Types;
using TXServer.Core.ServerMapInformation;
using TXServer.ECSSystem.EntityTemplates.Battle.Bonus;

namespace TXServer.Core.Battles
{
    public class BattleBonus
    {
        public BattleBonus(BonusType bonusType, Vector3 position)
        {
            BonusRegion = BonusRegionTemplate.CreateEntity(bonusType, position);
            BattleBonusType = bonusType;
            Position = position;
            BonusState = BonusState.Unused;
        }
        public void CreateBonus(Entity battleEntity)
        {
            if (BattleBonusType != BonusType.GOLD)
                Bonus = SupplyBonusTemplate.CreateEntity(BattleBonusType, BonusRegion, new Vector3(Position.X, Position.Y + 30, Position.Z), battleEntity);
            else
                Bonus = GoldBonusWithCrystalsTemplate.CreateEntity(BattleBonusType, BonusRegion, new Vector3(Position.X, Position.Y + 30, Position.Z), battleEntity);
        }
        
        public IEnumerable<Entity> GetEntities()
        {
            return from property in typeof(BattlePlayer).GetProperties()
                   where property.PropertyType == typeof(Entity)
                   select (Entity)property.GetValue(this);
        }

        public static readonly Dictionary<TankState, Type> StateComponents = new Dictionary<TankState, Type>
        {
            //{ TankState.New, typeof(TankNewStateComponent) },
            { TankState.Spawn, typeof(TankSpawnStateComponent) },
            { TankState.SemiActive, typeof(TankSemiActiveStateComponent) },
            { TankState.Active, typeof(TankActiveStateComponent) },
            { TankState.Dead, typeof(TankDeadStateComponent) },
        };

        public Entity BonusRegion { get; set; }
        public Entity Bonus { get; set; }
        public BonusType BattleBonusType { get; set; }
        public Vector3 Position { get; set; }
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
                            BonusStateChangeCountdown = 10;
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
