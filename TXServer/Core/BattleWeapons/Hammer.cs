using System;
using TXServer.Core.Battles;
using TXServer.Core.Configuration;
using Damage = TXServer.ECSSystem.ServerComponents.Damage;

namespace TXServer.Core.BattleWeapons
{
    public class Hammer : BattleWeapon
    {
        public Hammer(MatchPlayer matchPlayer) : base(matchPlayer) => DamagePerPellet =
            Config.GetComponent<Damage.DamagePerPelletPropertyComponent>(MarketItemPath).FinalValue;

        public override float BaseDamage(float hitDistance, MatchPlayer target, bool isSplashHit = false)
        {
            float distanceModifier = DamageDistanceMultiplier(hitDistance);

            return (int) Math.Round(DamagePerPellet * distanceModifier);
        }

        private float DamagePerPellet { get; set; }
    }
}
