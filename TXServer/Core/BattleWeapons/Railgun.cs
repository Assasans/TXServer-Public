using System;
using TXServer.Core.Battles;
using TXServer.Library;

namespace TXServer.Core.BattleWeapons
{
    public class Railgun : BattleWeapon
    {
        public Railgun(MatchPlayer matchPlayer) : base(matchPlayer)
        {
        }

        public override float BaseDamage(float hitDistance, MatchPlayer target, bool isSplashHit = false) =>
            (int) Math.Round(new Random().NextGaussianRange(MinDamage, MaxDamage));
    }
}
