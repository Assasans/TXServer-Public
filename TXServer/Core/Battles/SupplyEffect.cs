﻿using TXServer.ECSSystem.Types;
using System;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Health;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.Components.Battle.Chassis;
using System.Linq;
using TXServer.ECSSystem.Components.Battle;

namespace TXServer.Core.Battles
{
    public class SupplyEffect
    {
        public SupplyEffect(BonusType bonusType, MatchPlayer matchPlayer)
        {
            BonusType = bonusType;
            MatchPlayer = matchPlayer;
            StopTime = GetStopTime();

            SupplyEffect existingSupplyEffect = matchPlayer.SupplyEffects.SingleOrDefault(supplyEffect => supplyEffect.BonusType == bonusType);
            if (existingSupplyEffect != null)
            {
                existingSupplyEffect.StopTime = GetStopTime();
                existingSupplyEffect.SupplyEffectEntity.RemoveComponent<DurationComponent>();
                existingSupplyEffect.SupplyEffectEntity.AddComponent(new DurationComponent(new TXDate(DateTimeOffset.Now)));
                return;
            }

            switch (bonusType) {
                case BonusType.ARMOR:
                    SupplyEffectEntity = ArmorEffectTemplate.CreateEntity(MatchPlayer.Tank);
                    break;
                case BonusType.DAMAGE:
                    SupplyEffectEntity = DamageEffectTemplate.CreateEntity(MatchPlayer.Tank);
                    break;
                case BonusType.REPAIR:
                    MatchPlayer.Tank.ChangeComponent(new TemperatureComponent(0));
                    MatchPlayer.Tank.ChangeComponent<HealthComponent>(component => component.CurrentHealth = component.MaxHealth);
                    MatchPlayer.Battle.MatchPlayers.Select(x => x.Player).SendEvent(new HealthChangedEvent(), MatchPlayer.Tank);

                    SupplyEffectEntity = HealingEffectTemplate.CreateEntity(MatchPlayer.Tank);
                    break;
                case BonusType.SPEED:
                    MatchPlayer.Tank.ChangeComponent<SpeedComponent>(component => component.Speed = (float)(component.Speed * 1.5));
                    SupplyEffectEntity = TurboSpeedEffectTemplate.CreateEntity(MatchPlayer.Tank);
                    break;
            }

            MatchPlayer.Battle.MatchPlayers.Select(x => x.Player).ShareEntity(SupplyEffectEntity);
            MatchPlayer.SupplyEffects.Add(this);
        }

        public void Remove()
        {
            MatchPlayer.SupplyEffects.Remove(this);
            MatchPlayer.Battle.MatchPlayers.Select(x => x.Player).UnshareEntity(SupplyEffectEntity);
            if (BonusType == BonusType.SPEED)
                MatchPlayer.Tank.ChangeComponent<SpeedComponent>(component => component.Speed = (float)(component.Speed / 1.5));
        }

        private DateTime GetStopTime()
        {
            // todo: read this from configs   
            if (BonusType == BonusType.REPAIR)
                return DateTime.Now.AddSeconds(3);
            else
                return DateTime.Now.AddSeconds(30);
        }

        public BonusType BonusType { get; set; }
        public Entity SupplyEffectEntity { get; set; }
        public DateTime StopTime { get; set; }
        private MatchPlayer MatchPlayer { get; set; }
    }
}