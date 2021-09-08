using System;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Module.Healing;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;

namespace TXServer.Core.Battles.Effect
{
    public class RepairKitModule : BattleModule
    {
        public RepairKitModule(MatchPlayer matchPlayer, Entity garageModule) : base(
            matchPlayer,
            ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
        ) { }

        public override void Activate()
        {
            LastTickTime = DateTimeOffset.UtcNow.AddMilliseconds(-TickPeriod);

            float duration = IsSupply || IsCheat ? SupplyDuration(3000) : Duration;

            if (!IsSupply)
                Damage.DealHeal(InstantHp, MatchPlayer);

            if (EffectIsActive)
            {
                ChangeDuration(duration);
                return;
            }

            EffectEntity = HealingEffectTemplate.CreateEntity(MatchPlayer, (long) duration);
            MatchPlayer.Battle.PlayersInMap.ShareEntities(EffectEntity);

            Schedule(TimeSpan.FromMilliseconds(duration), Deactivate);
        }

        public override void Deactivate()
        {
            if (EffectEntity == null) return;
            if (IsCheat && !DeactivateCheat)
            {
                ChangeDuration(SupplyDuration(3000));
                return;
            }

            MatchPlayer.Battle.PlayersInMap.UnshareEntities(EffectEntity);

            EffectEntity = null;
            IsCheat = false;
        }

        public override void Init()
        {
            base.Init();
            ModuleHpPerMs = Config.GetComponent<ModuleHealingEffectHPPerMSPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
            InstantHp = Config.GetComponent<ModuleHealingEffectInstantHPPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
        }

        protected override void Tick()
        {
            base.Tick();

            if (EffectIsActive && DifferenceToLastHeal.TotalMilliseconds >= TickPeriod)
            {
                LastTickTime = DateTimeOffset.UtcNow;

                if (MatchPlayer.Tank.CurrentHealth >= MatchPlayer.Tank.MaxHealth) return;

                float healHp = MatchPlayer.Tank.CurrentHealth + TickPeriod * HpPerMs > MatchPlayer.Tank.MaxHealth
                    ? MatchPlayer.Tank.MaxHealth - MatchPlayer.Tank.CurrentHealth
                    : TickPeriod * HpPerMs;

                Damage.DealHeal(healHp, MatchPlayer);
            }
        }

        protected override bool AllowsEnabledState() => MatchPlayer.Tank.CurrentHealth < MatchPlayer.Tank.MaxHealth;

        private float HpPerMs => IsCheat || IsSupply ? 0.633f : ModuleHpPerMs;
        private float InstantHp { get; set; }
        private const float TickPeriod = 250;
        private float ModuleHpPerMs { get; set; }

        private DateTimeOffset LastTickTime { get; set; }
        private TimeSpan DifferenceToLastHeal => LastTickTime == default ? default : DateTimeOffset.UtcNow - LastTickTime;
    }
}
