using System;
using System.Text.RegularExpressions;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Health;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;

namespace TXServer.Core.Battles.Effect
{
    public class RepairKitModule : BattleModule
    {
        public RepairKitModule(MatchPlayer matchPlayer, Entity garageModule) : base(
            matchPlayer,
            ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
        )
        {
        }

        public override void Activate()
        {
            LastTickTime = DateTimeOffset.UtcNow.AddMilliseconds(-TickPeriod);

            float duration = IsSupply || IsCheat ? SupplyDuration(3000) : Duration;
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

        protected override void Tick()
        {
            base.Tick();

            if (EffectIsActive && DifferenceToLastHeal.TotalMilliseconds >= 250)
            {
                LastTickTime = DateTimeOffset.UtcNow;

                HealthComponent healthComponent = MatchPlayer.Tank.GetComponent<HealthComponent>();
                if (healthComponent.CurrentHealth >= healthComponent.MaxHealth) return;

                float healHp = healthComponent.CurrentHealth + TickPeriod * HpPerMs > healthComponent.MaxHealth
                    ? healthComponent.MaxHealth - healthComponent.CurrentHealth
                    : TickPeriod * HpPerMs;

                Damage.DealHeal(healHp, MatchPlayer);
            }
        }

        // TODO: find configs for repair kit module (card) & init
        private float HpPerMs { get; } = 0.633f;
        private float TickPeriod { get; } = 250;

        private DateTimeOffset LastTickTime { get; set; }
        private TimeSpan DifferenceToLastHeal => LastTickTime == default ? default : DateTimeOffset.UtcNow - LastTickTime;
    }
}
