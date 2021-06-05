using TXServer.ECSSystem.Types;
using System;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Health;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.Components.Battle.Chassis;
using System.Linq;
using TXServer.Core.Battles.Effect;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;

namespace TXServer.Core.Battles
{
    public class SupplyEffect
    {
        public SupplyEffect(BonusType bonusType, MatchPlayer matchPlayer, bool cheat = false, bool bonus = true,
            long duration = 30000)
        {
            if (matchPlayer.TankState != TankState.Active) return;

            if (duration == 30000 && bonusType == BonusType.REPAIR)
                duration = 3000;
            if (bonus && matchPlayer.FindModule(typeof(EngineerModule), out BattleModule module))
                duration = ((EngineerModule) module).SupplyDuration(duration);


            BonusType = bonusType;
            MatchPlayer = matchPlayer;
            Cheat = cheat;
            StopTime = DateTime.UtcNow.AddMilliseconds(duration);

            if (CheckForDuplicate()) return;
            ApplyEffect(duration);

            MatchPlayer.Battle.PlayersInMap.ShareEntities(SupplyEffectEntity);
            MatchPlayer.SupplyEffects.Add(this);
        }

        public void Remove()
        {
            MatchPlayer.SupplyEffects.Remove(this);
            MatchPlayer.Battle.PlayersInMap.UnshareEntities(SupplyEffectEntity);

            if (BonusType == BonusType.SPEED)
            {
                MatchPlayer.Tank.ChangeComponent(_prevSpeedComponent);
                _prevSpeedComponent = null;
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
                    SupplyEffectEntity = ArmorEffectTemplate.CreateEntity(durationMillis, MatchPlayer);
                    break;
                case BonusType.DAMAGE:
                    SupplyEffectEntity = DamageEffectTemplate.CreateEntity(durationMillis, MatchPlayer);
                    break;
                case BonusType.REPAIR:
                    MatchPlayer.Tank.ChangeComponent(new TemperatureComponent(0));
                    MatchPlayer.Tank.ChangeComponent<HealthComponent>(component => component.CurrentHealth = component.MaxHealth);
                    MatchPlayer.Battle.PlayersInMap.SendEvent(new HealthChangedEvent(), MatchPlayer.Tank);

                    SupplyEffectEntity = HealingEffectTemplate.CreateEntity(MatchPlayer);
                    break;
                case BonusType.SPEED:
                    MatchPlayer.Tank.ChangeComponent<SpeedComponent>(component =>
                    {
                        _prevSpeedComponent = (SpeedComponent)component.Clone();
                        if (Cheat) component.Speed = float.MaxValue;
                        else component.Speed *= 1.5f;
                    });
                    SupplyEffectEntity = TurboSpeedEffectTemplate.CreateEntity(durationMillis, MatchPlayer);
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
                StopTime = DateTime.UtcNow.AddSeconds(30);
                SupplyEffectEntity.RemoveComponent<DurationComponent>();
                SupplyEffectEntity.AddComponent(new DurationComponent(DateTime.UtcNow));
            }
        }

        public BonusType BonusType { get; set; }
        public Entity SupplyEffectEntity { get; set; }
        private SpeedComponent _prevSpeedComponent;
        public DateTime StopTime { get; set; }
        public bool Cheat { get; set; }
        private MatchPlayer MatchPlayer { get; set; }
    }
}
