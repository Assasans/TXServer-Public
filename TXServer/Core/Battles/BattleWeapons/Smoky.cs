using System;
using System.Numerics;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Events.Battle.Weapon.Smoky;
using TXServer.ECSSystem.ServerComponents.Hit;
using TXServer.Library;

namespace TXServer.Core.Battles.BattleWeapons
{
    public class Smoky : BattleWeapon
    {
        public Smoky(MatchPlayer matchPlayer) : base(matchPlayer)
        {
            AfterCriticalProbability = Config
                .GetComponent<CriticalHit.AfterCriticalHitProbabilityPropertyComponent>(MarketItemPath).FinalValue;
            CriticalProbabilityDelta = Config
                .GetComponent<CriticalHit.CriticalProbabilityDeltaPropertyComponent>(MarketItemPath).FinalValue;
            MaxCriticalProbability = Config
                .GetComponent<CriticalHit.MaxCriticalProbabilityPropertyComponent>(MarketItemPath).FinalValue;
            CurrentCriticalProbability = StartCriticalProbability = Config
                .GetComponent<CriticalHit.StartCriticalProbabilityPropertyComponent>(MarketItemPath).FinalValue;
        }

        public override float BaseDamage(float hitDistance, MatchPlayer target, bool isSplashHit = false)
        {
            float damage = (int) Math.Round(new Random().NextGaussianRange(MinDamage, MaxDamage));
            float distanceModifier = DamageDistanceMultiplier(hitDistance);

            return (int) Math.Round(damage * distanceModifier);
        }

        public override float DamageWithCritical(bool backHit, float damage) =>
            (float) (backHit ? damage * 2.07f : damage * 1.87);

        public override bool IsCritical(MatchPlayer victim, Vector3 localPosition)
        {
            if (new Random().Next(0, 100) <= CurrentCriticalProbability * 100)
            {
                CurrentCriticalProbability = AfterCriticalProbability;

                MatchPlayer.Battle.PlayersInMap.SendEvent(new CriticalDamageEvent(victim.TankEntity, localPosition),
                    MatchPlayer.WeaponEntity);
                return true;
            }

            CurrentCriticalProbability = Math.Clamp(CurrentCriticalProbability + CriticalProbabilityDelta,
                AfterCriticalProbability, MaxCriticalProbability);

            return false;
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            CurrentCriticalProbability = StartCriticalProbability;
        }

        private float CurrentCriticalProbability { get; set; }

        private float AfterCriticalProbability { get; }
        private float CriticalProbabilityDelta { get; }
        private float MaxCriticalProbability { get; }
        private float StartCriticalProbability { get; }
    }
}
