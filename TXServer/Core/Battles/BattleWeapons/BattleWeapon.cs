using System.Linq;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Weapon;
using TXServer.ECSSystem.ServerComponents;
using TXServer.Library;

namespace TXServer.Core.Battles.BattleWeapons
{
    public abstract class BattleWeapon
    {
        protected BattleWeapon(MatchPlayer matchPlayer)
        {
            MatchPlayer = matchPlayer;

            CooldownIntervalSec = Config.GetComponent<WeaponCooldownComponent>(BattleItemPath, false)?.CooldownIntervalSec ?? 0;
            DamagePerSecond = Config.GetComponent<TXServer.ECSSystem.ServerComponents.Damage.DamagePerSecondPropertyComponent>(MarketItemPath, false)?.FinalValue ?? 0;

            MaxDamage = Config.GetComponent<TXServer.ECSSystem.ServerComponents.Damage.MaxDamagePropertyComponent>(MarketItemPath, false)?.FinalValue ?? 0;
            MinDamage = Config.GetComponent<TXServer.ECSSystem.ServerComponents.Damage.MinDamagePropertyComponent>(MarketItemPath, false)?.FinalValue ?? 0;

            if (MaxDamage == 0)
            {
                MaxDamage = Config
                    .GetComponent<TXServer.ECSSystem.ServerComponents.Damage.AimingMaxDamagePropertyComponent>(MarketItemPath, false)?.FinalValue ?? 0;
                MinDamage = Config
                    .GetComponent<TXServer.ECSSystem.ServerComponents.Damage.AimingMinDamagePropertyComponent>(MarketItemPath, false)?.FinalValue ?? 0;
            }

            MaxDamageDistance = Config.GetComponent<
                DamageWeakeningByDistance.MaxDamageDistancePropertyComponent>(MarketItemPath, false)?.FinalValue ?? 0;
            MinDamageDistance = Config.GetComponent<
                DamageWeakeningByDistance.MinDamageDistancePropertyComponent>(MarketItemPath, false)?.FinalValue ?? 0;
            MinDamagePercent = Config.GetComponent<
                DamageWeakeningByDistance.MinDamagePercentPropertyComponent>(MarketItemPath, false)?.FinalValue ?? 0;

            SplashWeaponComponent damageComponent = Weapon.GetComponent<SplashWeaponComponent>();
            if (damageComponent != null)
            {
                RadiusOfMaxSplashDamage = damageComponent.RadiusOfMaxSplashDamage;
                RadiusOfMinSplashDamage = damageComponent.RadiusOfMinSplashDamage;
            }
        }

        public abstract float BaseDamage(float hitDistance, MatchPlayer target, bool isSplashHit = false);

        protected float DamageDistanceMultiplier(float distance)
        {
            float distanceModifier;
            if (distance < MaxDamageDistance)
                distanceModifier = 1;
            else
                distanceModifier = MathUtils.Map(distance, MinDamageDistance,
                    MaxDamageDistance, MinDamagePercent / 100, 1);

            return distanceModifier;
        }

        public virtual void OnSpawn()
        {
            Weapon.AddComponent(new ShootableComponent());
        }
        public virtual void OnDespawn()
        {
            Weapon.TryRemoveComponent<ShootableComponent>();
        }

        public virtual void Tick() {}


        protected readonly MatchPlayer MatchPlayer;
        protected Entity Weapon => MatchPlayer.Weapon;
        protected Entity MarketItem => MatchPlayer.Player.CurrentPreset.Weapon;
        protected string MarketItemPath => MarketItem.TemplateAccessor.ConfigPath;
        private string BattleItemPath => "battle/weapon/" + MarketItemPath.Split('/').Last();
        public Component[] CustomComponents { get; set; }

        protected float CooldownIntervalSec { get; }
        protected float DamagePerSecond { get; }

        protected float MaxDamage { get; }
        protected float MinDamage { get; }

        private float MaxDamageDistance { get; }
        private float MinDamageDistance { get; }
        protected float MinDamagePercent { get; }

        protected float RadiusOfMaxSplashDamage { get; }
        protected float RadiusOfMinSplashDamage { get; }
    }
}
