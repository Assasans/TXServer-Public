using System;
using System.Collections.Generic;
using System.Numerics;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Weapon;
using TXServer.ECSSystem.Components.Battle.Weapon.Hammer;
using TXServer.ECSSystem.ServerComponents.Weapon;
using TXServer.ECSSystem.Types;

namespace TXServer.Core.Battles.BattleWeapons
{
    public class Hammer : BattleWeapon
    {
        public Hammer(MatchPlayer matchPlayer) : base(matchPlayer)
        {
            CurrentCartridgeCount =
                (int) Config.GetComponent<MagazineWeapon.MagazineSizePropertyComponent>(MarketItemPath).FinalValue;
            DamagePerPellet =
                Config.GetComponent<TXServer.ECSSystem.ServerComponents.Damage.DamagePerPelletPropertyComponent>(MarketItemPath).FinalValue;
            MaxCartridgeCount = (int) Config.GetComponent<MagazineWeapon.MagazineSizePropertyComponent>(MarketItemPath)
                .FinalValue;
            ReloadMagazineDurationSec = Config
                .GetComponent<MagazineWeapon.ReloadMagazineTimePropertyComponent>(MarketItemPath).FinalValue;

            CustomComponents = new Component[]
            {
                new WeaponCooldownComponent(1.9f)
            };
        }

        public override float BaseDamage(float hitDistance, MatchPlayer target, bool isSplashHit = false)
        {
            float distanceModifier = DamageDistanceMultiplier(hitDistance);
            return (int) Math.Round(DamagePerPellet * distanceModifier);
        }

        private void CombineHitInfo(List<HitTarget> hitTargets, out Dictionary<MatchPlayer, (bool, float, Vector3)> hits)
        {
            hits = new Dictionary<MatchPlayer, (bool, float, Vector3)>();

            foreach (HitTarget hitTarget in hitTargets)
            {
                MatchPlayer target = Core.Battles.Damage.GetTargetByHit(MatchPlayer, hitTarget);
                (bool backHit, float damage, Vector3 hitPoint) hitInfo =
                    hits.ContainsKey(target) ? hits[target] : (false, 0, hitTarget.LocalHitPoint);

                if (hitInfo.backHit is false && Core.Battles.Damage.IsBackHit(hitTarget.LocalHitPoint, target.Tank))
                {
                    hitInfo.backHit = true;
                    hitInfo.hitPoint = hitTarget.LocalHitPoint;
                }

                hitInfo.damage += BaseDamage(hitTarget.HitDistance, target);

                hits[target] = hitInfo;
            }
        }

        public override void OnDespawn()
        {
            base.OnDespawn();
            ResetMagazine();
        }

        public void ProcessHits(List<HitTarget> targets)
        {
            CombineHitInfo(targets, out Dictionary<MatchPlayer, (bool, float, Vector3)> hits);
            foreach (KeyValuePair<MatchPlayer, (bool, float, Vector3)> hit in hits)
                Core.Battles.Damage.DealDamage(MatchPlayer.Player.CurrentPreset.Weapon, hit.Key,
                    MatchPlayer, hit.Value.Item2, hit.Value.Item1, hit.Value.Item3);
        }

        private void ProcessReloading()
        {
            if (ReloadStartTime is null) return;

            if (!((DateTimeOffset.UtcNow - ReloadStartTime).Value.TotalMilliseconds >=
                  ReloadMagazineDurationSec * 1000)) return;

            ResetReload();
            CurrentCartridgeCount = MaxCartridgeCount;
            Weapon.AddComponent(new ShootableComponent());
        }

        public void ProcessShot() => CurrentCartridgeCount--;

        public void ResetMagazine()
        {
            CurrentCartridgeCount = MaxCartridgeCount;
            ResetReload();
        }

        private void ResetReload()
        {
            ReloadStartTime = null;
            Weapon.TryRemoveComponent<MagazineReloadStateComponent>();
        }

        private void StartReloading()
        {
            Weapon.TryRemoveComponent<ShootableComponent>();
            Weapon.AddComponent(new MagazineReloadStateComponent());
            ReloadStartTime = DateTimeOffset.UtcNow;
        }

        public override void Tick()
        {
            base.Tick();
            ProcessReloading();
        }

        private int CurrentCartridgeCount
        {
            get => _currentCartridgeCount;
            set
            {
                _currentCartridgeCount = value;

                Weapon.AddOrChangeComponent(new MagazineStorageComponent(value));
                if (value < 1)
                    StartReloading();
            }
        }

        private DateTimeOffset? ReloadStartTime { get; set; }

        private float DamagePerPellet { get; }
        private int MaxCartridgeCount { get; }
        private float ReloadMagazineDurationSec { get; }

        private int _currentCartridgeCount;
    }
}
