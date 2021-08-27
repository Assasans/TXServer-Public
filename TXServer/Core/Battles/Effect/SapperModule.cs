using System;
using System.Collections.Generic;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Module.Sapper;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.Core.Battles.Effect
{
    public class SapperModule : BattleModule
    {
        public SapperModule(MatchPlayer matchPlayer, Entity garageModule) : base(
            matchPlayer,
            ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
        ) { }

        public override void Activate()
        {
            if (EffectIsActive || IsOnCooldown) return;

            EffectEntity = SapperEffectTemplate.CreateEntity(MatchPlayer);
            MatchPlayer.Battle.PlayersInMap.ShareEntities(EffectEntity);

            CurrentAmmunition--;

            Schedule(TimeSpan.FromMilliseconds(Duration), Deactivate);
        }

        public override void Deactivate()
        {
            if (!EffectIsActive) return;

            MatchPlayer.Battle.PlayersInMap.UnshareEntities(EffectEntity);
            EffectEntity = null;
        }

        public override void Init()
        {
            base.Init();

            DamageResistanceEffect  = Config.GetComponent<DamageResistanceEffectPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
        }

        public override float DamageWithEffect(float damage, MatchPlayer target, bool isHeatDamage, bool isModuleDamage,
            Entity weaponMarketItem)
        {
            if (!(Mines.Contains(weaponMarketItem.TemplateAccessor.Template.GetType()) && EffectIsActive &&
                  !IsOnCooldown))
                return damage;

            Activate();
            return damage * DamageResistanceEffect;
        }

        private float DamageResistanceEffect { get; set; }

        private List<Type> Mines = new()
        {
            Modules.GlobalItems.Mine.GetType(),
            Modules.GlobalItems.Spidermine.GetType(),
        };
    }
}
