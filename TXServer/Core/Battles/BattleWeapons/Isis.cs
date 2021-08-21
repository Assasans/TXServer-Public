using System.Linq;
using TXServer.Core.Battles.Effect;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.Events.Battle.VisualScore;
using TXServer.ECSSystem.Types;

namespace TXServer.Core.Battles.BattleWeapons
{
    public class Isis : BattleWeapon
    {
        public Isis(MatchPlayer matchPlayer) : base(matchPlayer)
        {
            DecreaseFriendTemperature = -Config.GetComponent<TXServer.ECSSystem.ServerComponents.Damage.
                DecreaseFriendTemperaturePropertyComponent>(MarketItemPath).FinalValue / 2.25f;
            IncreaseFriendTemperature = Config.GetComponent<TXServer.ECSSystem.ServerComponents.Damage.
                IncreaseFriendTemperaturePropertyComponent>(MarketItemPath).FinalValue / 2.25f;

            HealPerSecond = Config.GetComponent<TXServer.ECSSystem.ServerComponents.Damage.HealingPropertyComponent>(
                MarketItemPath).FinalValue;
            SelfHealPercentage = Config.GetComponent<TXServer.ECSSystem.ServerComponents.Damage.
                SelfHealingPropertyComponent>(MarketItemPath).FinalValue;

            NotFriendlyFireUsable = true;
        }

        public override float BaseDamage(float hitDistance, MatchPlayer target, bool isSplashHit = false)
        {
            bool isEnemy = MatchPlayer.IsEnemyOf(target);
            float dps = (isEnemy ? DamagePerSecond : HealPerSecond) * CooldownIntervalSec;

            if (isEnemy) SelfHeal(dps);

            return (int) dps;
        }

        private float GetHealHpWithEffects(float hp)
        {
            if (MatchPlayer.TryGetModule(out AdrenalineModule adrenalineModule) &&
                adrenalineModule.EffectIsActive)
                hp *= adrenalineModule.DamageFactor;

            if (MatchPlayer.TryGetModule(out IncreasedDamageModule damageModule) &&
                damageModule.EffectIsActive)
                hp *= damageModule.Factor;

            return hp;
        }

        public void HealMate(MatchPlayer target, HitTarget hitTarget)
        {
            float hp = GetHealHpWithEffects(BaseDamage(0, target));

            Damage.DealNewTemperature(Weapon, MarketItem, target, MatchPlayer, onlyNormalize:true);
            if (!Damage.DealHeal(hp, target)) return;

            int additiveScore = MatchPlayer.GetScoreWithBonus(2);

            target.Player.BattlePlayer.MatchPlayer.HealthChanged();
            MatchPlayer.Battle.Spectators.Concat(new[] {(IPlayerPart) MatchPlayer})
                .SendEvent(new DamageInfoEvent(hp, hitTarget.LocalHitPoint, false, true), target.Tank);
            MatchPlayer
                .SendEvent(new VisualScoreHealEvent(additiveScore), MatchPlayer.BattleUser);

            MatchPlayer.UpdateStatistics(additiveScore:additiveScore, 0, 0, 0, null);
        }

        public override bool IsOnCooldown(MatchPlayer target) => IsStreamOnCooldown(target);

        private void SelfHeal(float damage) =>
            Damage.DealHeal(GetHealHpWithEffects(damage / 100 * SelfHealPercentage), MatchPlayer);

        public override float TemperatureDeltaPerHit(float targetTemperature) =>
            targetTemperature switch
            {
                > 0 => DecreaseFriendTemperature,
                < 0 => IncreaseFriendTemperature,
                _ => 0
            };


        private float DecreaseFriendTemperature { get; }
        private float IncreaseFriendTemperature { get; }

        private float HealPerSecond { get; }
        private float SelfHealPercentage { get; }
    }
}
