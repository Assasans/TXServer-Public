using System;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Weapon;
using TXServer.ECSSystem.ServerComponents.Weapon;
using TXServer.Library;

namespace TXServer.Core.Battles.BattleWeapons
{
    public class Ricochet : BattleWeapon
    {
        public Ricochet(MatchPlayer matchPlayer) : base(matchPlayer)
        {
            float energyChargePerShot =
                Config.GetComponent<EnergyChargePerShotPropertyComponent>(MarketItemPath).FinalValue;
            float energyRechargeSpeed =
                Config.GetComponent<EnergyRechargeSpeedPropertyComponent>(MarketItemPath).FinalValue;

            CustomComponents = new Component[]
            {
                new WeaponCooldownComponent(0.7f),
                new DiscreteWeaponEnergyComponent(energyRechargeSpeed, energyChargePerShot)
            };
        }

        public override float BaseDamage(float hitDistance, MatchPlayer target, bool isSplashHit = false)
        {
            float damage = (int) Math.Round(new Random().NextGaussianRange(MinDamage, MaxDamage));
            float distanceModifier = DamageDistanceMultiplier(hitDistance);

            return (int) Math.Round(damage * distanceModifier);
        }
    }
}
