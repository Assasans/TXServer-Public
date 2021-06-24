using System;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Health;
using TXServer.ECSSystem.Components.Battle.Module.LifeSteal;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.Events.Battle.Effect;

namespace TXServer.Core.Battles.Effect
{
    public class LifeStealModule : BattleModule
    {
        public LifeStealModule(MatchPlayer matchPlayer, Entity garageModule) : base(
            matchPlayer,
            ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
        ) { }

        public override void Activate()
        {
            if (IsOnCooldown || MatchPlayer.TankState == TankState.Dead || IsEmpLocked) return;

            HealthComponent healthComponent = MatchPlayer.Tank.GetComponent<HealthComponent>();

            if (!(healthComponent.CurrentHealth < healthComponent.MaxHealth)) return;

            CurrentAmmunition--;

            EffectEntity = LifestealEffectTemplate.CreateEntity(MatchPlayer);
            MatchPlayer.Battle.PlayersInMap.ShareEntities(EffectEntity);

            MatchPlayer.Tank.ChangeComponent<HealthComponent>(component =>
            {
                component.CurrentHealth += component.MaxHealth * AdditiveHpFactor;
                component.CurrentHealth += FixedHp;
            });

            MatchPlayer.Battle.PlayersInMap.SendEvent(new TriggerEffectExecuteEvent(), EffectEntity);
            MatchPlayer.HealthChanged();

            Schedule(() => { MatchPlayer.Battle.PlayersInMap.UnshareEntities(EffectEntity); });
        }

        public override void Init()
        {
            base.Init();

            AdditiveHpFactor = Config.GetComponent<ModuleLifestealEffectAdditiveHPFactorPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
            FixedHp = Config.GetComponent<ModuleLifestealEffectFixedHPPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
        }

        private float AdditiveHpFactor { get; set; }
        private float FixedHp { get; set; }
    }
}
