﻿using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Effect;
using TXServer.ECSSystem.Components.Battle.Effect.ExternalImpact;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Effect
{
    [SerialVersionUID(1542270967570L)]
    public class ExternalImpactEffectTemplate : EffectBaseTemplate
    {
        public static Entity CreateEntity(MatchPlayer matchPlayer)
        {
            Entity effect = CreateEntity(new ExternalImpactEffectTemplate(), "battle/effect/externalimpact", matchPlayer);

            effect.AddComponent(matchPlayer.Battle.BattleEntity.GetComponent<BattleGroupComponent>());
            effect.AddComponent(new ExternalImpactEffectComponent());
            effect.AddComponent(new SplashEffectComponent(true));
            effect.AddComponent(new SplashWeaponComponent(40f, 0f, 15f));

            effect.AddComponent(new ImpactComponent(400));
            effect.AddComponent(new DamageWeakeningByDistanceComponent(70, 0, 100));
            effect.AddComponent(new DiscreteWeaponComponent());

            return effect;
        }
    }
}