using TXServer.ECSSystem.Types;
using System;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Health;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.Components.Battle.Chassis;
using System.Linq;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.EntityTemplates.Effects;

namespace TXServer.Core.Battles
{
    public class SupplyEffect
    {
        public SupplyEffect(BonusType bonusType, MatchPlayer matchPlayer, bool cheat = false, long duration = 30)
        {
            if (matchPlayer.TankState != TankState.Active) return;
            if (duration == 30 && bonusType == BonusType.REPAIR)
                duration = 3;

            BonusType = bonusType;
            MatchPlayer = matchPlayer;
            Cheat = cheat;
            StopTime = DateTime.Now.AddSeconds(duration);

            if (CheckForDuplicate()) return;
            ApplyEffect(duration * 1000);

            MatchPlayer.Battle.PlayersInMap.ShareEntities(SupplyEffectEntity);
            MatchPlayer.SupplyEffects.Add(this);
        }

        public void Remove()
        {
            MatchPlayer.SupplyEffects.Remove(this);
            MatchPlayer.Battle.PlayersInMap.UnshareEntities(SupplyEffectEntity);

            if (BonusType == BonusType.SPEED)
            {
                // todo: load from configs
                MatchPlayer.Tank.ChangeComponent<SpeedComponent>(component => component.Speed = 9.967f);
            }
        }

        private bool CheckForDuplicate()
        {
            SupplyEffect existingSupplyEffect = MatchPlayer.SupplyEffects.SingleOrDefault(supplyEffect => supplyEffect.BonusType == BonusType);
            if (existingSupplyEffect != null)
            {
                existingSupplyEffect.ExtendTime();
                return true;
            }
            return false;
        }

        private void ApplyEffect(long durationMillis)
        {
            switch (BonusType)
            {
                case BonusType.ARMOR:
                    SupplyEffectEntity = ArmorEffectTemplate.CreateEntity(durationMillis, MatchPlayer.Tank);
                    break;
                case BonusType.DAMAGE:
                    SupplyEffectEntity = DamageEffectTemplate.CreateEntity(durationMillis, MatchPlayer.Tank);
                    break;
                case BonusType.REPAIR:
                    MatchPlayer.Tank.ChangeComponent(new TemperatureComponent(0));
                    MatchPlayer.Tank.ChangeComponent<HealthComponent>(component => component.CurrentHealth = component.MaxHealth);
                    MatchPlayer.Battle.PlayersInMap.SendEvent(new HealthChangedEvent(), MatchPlayer.Tank);

                    SupplyEffectEntity = HealingEffectTemplate.CreateEntity(MatchPlayer.Tank);
                    break;
                case BonusType.SPEED:
                    MatchPlayer.Tank.ChangeComponent<SpeedComponent>(component =>
                    {
                        if (Cheat) component.Speed = float.MaxValue;
                        else component.Speed = (float)(component.Speed * 1.5);
                    });
                    SupplyEffectEntity = TurboSpeedEffectTemplate.CreateEntity(durationMillis, MatchPlayer.Tank);
                    break;
            }
        }

        public void ExtendTime()
        {
            if (BonusType == BonusType.REPAIR)
            {
                Remove();
                _ = new SupplyEffect(BonusType, MatchPlayer, Cheat);
            }
            else
            {
                StopTime = DateTime.Now.AddSeconds(30);
                SupplyEffectEntity.RemoveComponent<DurationComponent>();
                SupplyEffectEntity.AddComponent(new DurationComponent { StartedTime = DateTime.Now });
            }
        }

        public BonusType BonusType { get; set; }
        public Entity SupplyEffectEntity { get; set; }
        public DateTime StopTime { get; set; }
        public bool Cheat { get; set; }
        private MatchPlayer MatchPlayer { get; set; }
    }
}
