using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Effect.Mine;
using TXServer.ECSSystem.Components.Battle.Module.Icetrap;
using TXServer.ECSSystem.Components.Battle.Module.Mine;
using TXServer.ECSSystem.Components.Battle.Module.MultipleUsage;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;
using TXServer.ECSSystem.Events.Battle.Effect.Mine;

namespace TXServer.Core.Battles.Effect
{
    public class IcetrapModule : BattleModule
    {
        public IcetrapModule(MatchPlayer matchPlayer, Entity garageModule) : base(
            matchPlayer,
            ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
        ) { }

        public override void Activate()
        {
            Entity mine = IcetrapEffectTemplate.CreateEntity(MatchPlayer, activationTime: ActivationTime,
                beginHideDistance: BeginHideDistance, damageMaxRadius: DamageMaxRadius,
                damageMinRadius: DamageMinRadius, damageMinPercent: DamageMinPercent, hideRange: HideRange,
                impact: Impact);
            EffectEntities.Add(mine);
            Positions.Add(mine, mine.GetComponent<MinePositionComponent>().Position);

            MatchPlayer.Battle.PlayersInMap.ShareEntities(mine);
        }

        public override void Deactivate()
        {
            foreach (Entity mine in EffectEntities)
                MatchPlayer.Battle.PlayersInMap.UnshareEntities(mine);
            EffectEntities.Clear();
        }

        public override void Init()
        {
            base.Init();
            IsAffectedByEmp = false;
            WeaponType = typeof(IcetrapEffectTemplate);

            ActivationTime = (long) Config.GetComponent<ModuleEffectActivationTimePropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
            BeginHideDistance = Config.GetComponent<ModuleMineEffectBeginHideDistancePropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
            DamageMaxRadius = Config
                .GetComponent<ModuleMineEffectSplashDamageMaxRadiusPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
            DamageMinRadius = Config
                .GetComponent<ModuleMineEffectSplashDamageMinRadiusPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
            DamageMinPercent = Config
                .GetComponent<ModuleMineEffectSplashDamageMinPercentPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
            ExplosionDelayMs = Config.GetComponent<ModuleMineEffectExplosionDelayMSPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
            HideRange = Config.GetComponent<ModuleMineEffectHideRangePropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
            Impact = Config.GetComponent<ModuleMineEffectImpactPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
            TemperatureChange = Config.GetComponent<ModuleIcetrapEffectTemperatureLimitPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
            TemperatureNormalizationBlock = Config.GetComponent<
                ModuleIcetrapEffectTemperatureDurationPropertyComponent>(ConfigPath).UpgradeLevel2Values[Level];
            TriggeringArea = Config.GetComponent<ModuleMineEffectTriggeringAreaPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
        }

        public override float BaseDamage(Entity mine, MatchPlayer target)
        {
            if (EffectEntities.Contains(mine))
                Explode(mine);

            target.TemperatureHits.Clear();
            target.Temperature = MatchPlayer.TemperatureFromAllHits();
            target.SpeedByTemperature();
            Damage.DealNewTemperature(mine, MarketItem, target, MatchPlayer);

            return base.BaseDamage(mine, target);
        }


        private void Explode(Entity mine)
        {
            MatchPlayer.Battle.PlayersInMap.SendEvent(new MineExplosionEvent(), mine);
            EffectEntities.Remove(mine);
            MatchPlayer.Battle.PlayersInMap.Where(p => p.Player.EntityList.Contains(mine)).UnshareEntities(mine);
        }

        private void TryExplode(Entity mine)
        {
            Positions.Remove(mine);
            MatchPlayer.Battle.PlayersInMap.SendEvent(new MineTryExplosionEvent(), mine);
        }

        protected override void Tick()
        {
            base.Tick();

            foreach ((Entity mine, Vector3 position) in Positions)
            {
                List<MatchPlayer> players = MatchPlayer.Battle.MatchTankPlayers.Select(p => p.MatchPlayer).ToList();
                for (int i = 0; i < MatchPlayer.Battle.MatchTankPlayers.Count; i++)
                {
                    if (MatchPlayer.IsEnemyOf(players[i]) &&
                        Vector3.Distance(players[i].TankPosition, position) < TriggeringArea)
                        TryExplode(mine);
                }
            }
        }

        private Dictionary<Entity, Vector3> Positions { get; } = new();

        private long ActivationTime { get; set; }
        private float BeginHideDistance { get; set; }
        private float DamageMaxRadius { get; set; }
        private float DamageMinRadius { get; set; }
        private float DamageMinPercent { get; set; }
        private float ExplosionDelayMs { get; set; }
        private float HideRange { get; set; }
        private float Impact { get; set; }
        private float TriggeringArea { get; set; }
        private float TemperatureDuration { get; set; }
    }
}
