using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
            DamagePerSecond = Config.GetComponent<TXServer.ECSSystem.ServerComponents.Damage.
                DamagePerSecondPropertyComponent>(MarketItemPath, false)?.FinalValue ?? 0;

            MaxDamage = Config.GetComponent<TXServer.ECSSystem.ServerComponents.Damage.
                MaxDamagePropertyComponent>(MarketItemPath, false)?.FinalValue ?? 0;
            MinDamage = Config.GetComponent<TXServer.ECSSystem.ServerComponents.Damage.
                MinDamagePropertyComponent>(MarketItemPath, false)?.FinalValue ?? 0;

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

            WeaponRotationComponent weaponRotationComponent = BattlePlayer.TurretRotationSpeed is null
                ? Config.GetComponent<WeaponRotationComponent>(
                    MatchPlayer.Tank.TemplateAccessor.ConfigPath.Replace("battle", "garage"))
                : new WeaponRotationComponent((float) BattlePlayer.TurretRotationSpeed);
            OriginalWeaponRotationComponent = (WeaponRotationComponent) weaponRotationComponent.Clone();
            CustomComponents.Add(weaponRotationComponent);
        }

        public abstract float BaseDamage(float hitDistance, MatchPlayer target, bool isSplashHit = false);

        protected void ChangeRotationSpeed(float accelerationMultiplier = 1,
            float speedMultiplier = 1)
        {
            Weapon.ChangeComponent<WeaponRotationComponent>(
                component =>
                {
                    component.Acceleration *= accelerationMultiplier;
                    component.Speed *= speedMultiplier;
                });
        }

        public virtual float DamageWithCritical(bool backHit, float damage) => damage;

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

        public virtual bool IsCritical(MatchPlayer victim, Vector3 localPosition) => false;

        public virtual bool IsOnCooldown(MatchPlayer target) => false;

        protected bool IsStreamOnCooldown(MatchPlayer target)
        {
            if (!MatchPlayer.StreamHitLengths.ContainsKey(target))
            {
                MatchPlayer.StreamHitLengths[target] = (0, DateTimeOffset.UtcNow);
                return true;
            }

            (double, DateTimeOffset) streamLength = MatchPlayer.StreamHitLengths[target];
            streamLength.Item1 += (DateTimeOffset.UtcNow - streamLength.Item2).TotalMilliseconds;
            streamLength.Item2 = DateTimeOffset.UtcNow;
            MatchPlayer.StreamHitLengths[target] = streamLength;

            if (MatchPlayer.StreamHitLengths[target].Item1 / 1000 < CooldownIntervalSec)
                return true;

            MatchPlayer.StreamHitLengths.Remove(target);
            return false;
        }

        public virtual void OnSpawn()
        {
            Weapon.AddComponent(new ShootableComponent());
        }
        public virtual void OnDespawn()
        {
            Weapon.TryRemoveComponent<ShootableComponent>();
        }

        protected void RestoreRotation() => Weapon.ChangeComponent((Component) OriginalWeaponRotationComponent.Clone());

        public virtual float TemperatureDeltaPerHit(float targetTemperature) => 0;

        public virtual void Tick() {}


        protected readonly MatchPlayer MatchPlayer;
        private BattleTankPlayer BattlePlayer => MatchPlayer.Player.BattlePlayer;

        protected Entity Weapon => MatchPlayer.Weapon;
        protected Entity MarketItem => MatchPlayer.Player.CurrentPreset.Weapon;
        protected string MarketItemPath => MarketItem.TemplateAccessor.ConfigPath;
        private string BattleItemPath => "battle/weapon/" + MarketItemPath.Split('/').Last();
        public List<Component> CustomComponents { get; } = new();

        public bool AllowsSelfDamage { get; protected init; }
        public bool NotFriendlyFireUsable { get; protected init; }

        protected float CooldownIntervalSec { get; }
        protected float DamagePerSecond { get; }

        protected float MaxDamage { get; }
        protected float MinDamage { get; }

        public float MaxHeatDamage { get; protected init; }
        public float MinHeatDamage { get; protected init; }

        private float MaxDamageDistance { get; }
        private float MinDamageDistance { get; }
        protected float MinDamagePercent { get; }

        protected float RadiusOfMaxSplashDamage { get; }
        protected float RadiusOfMinSplashDamage { get; }

        public float TemperatureLimit { get; init; } = 1;

        public WeaponRotationComponent OriginalWeaponRotationComponent { get; set; }
    }
}
