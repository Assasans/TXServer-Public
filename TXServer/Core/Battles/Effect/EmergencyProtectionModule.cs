using System;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Module.EmergencyProtection;
using TXServer.ECSSystem.Components.Battle.Weapon;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;
using TXServer.ECSSystem.Events.Battle.Effect;
using TXServer.Library;
using HealthComponent = TXServer.ECSSystem.Components.Battle.Health.HealthComponent;

namespace TXServer.Core.Battles.Effect
{
    public class EmergencyProtectionModule : BattleModule
    {
        public EmergencyProtectionModule(MatchPlayer matchPlayer, Entity garageModule) : base(
            matchPlayer,
            ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
        ) { }

        public override void Activate()
        {
            if (IsOnCooldown || IsEmpLocked) return;
            if (EffectIsActive) Deactivate();

            CurrentAmmunition--;

            ImmunityEndTime = DateTimeOffset.UtcNow.AddMilliseconds(HolyshieldDuration);

            EffectEntity = EmergencyProtectionEffectTemplate.CreateEntity(MatchPlayer);
            MatchPlayer.Battle.PlayersInMap.ShareEntities(EffectEntity);

            MatchPlayer.Battle.PlayersInMap.SendEvent(new TriggerEffectExecuteEvent(), EffectEntity);
            MatchPlayer.Weapon.TryRemoveComponent<ShootableComponent>();

            MatchPlayer.TemperatureHits.Clear();
            Damage.DealNewTemperature(EffectEntity, MarketItem, MatchPlayer, MatchPlayer);

            Schedule(TimeSpan.FromMilliseconds(HolyshieldDuration), Deactivate);
        }

        public override void Deactivate()
        {
            if (EffectEntity == null) return;

            if (DateTimeOffset.UtcNow >= ImmunityEndTime)
            {
                float healHp = FixedHp + AdditiveHpFactor * MatchPlayer.Tank.GetComponent<HealthComponent>().MaxHealth;
                Damage.DealHeal(healHp, MatchPlayer);
            }

            MatchPlayer.Weapon.AddComponent(new ShootableComponent());
            MatchPlayer.Battle.PlayersInMap.UnshareEntities(EffectEntity);

            EffectEntity = null;
        }

        public override void Init()
        {
            base.Init();

            AdditiveHpFactor = Config
                .GetComponent<ModuleEmergencyProtectionEffectAdditiveHPFactorPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
            FixedHp = Config.GetComponent<ModuleEmergencyProtectionEffectFixedHPPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
            HolyshieldDuration = Config
                .GetComponent<ModuleEmergencyProtectionEffectHolyshieldDurationPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];

            TemperatureChange = (float) MathUtils.Map(Level, 0, 9, -0.8, -1);
        }

        private DateTimeOffset ImmunityEndTime { get; set; }

        private float AdditiveHpFactor { get; set; }
        private float FixedHp { get; set; }
        private float HolyshieldDuration { get; set; }
    }
}
