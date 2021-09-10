using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Module.LifeSteal;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;
using TXServer.ECSSystem.Events.Battle.Effect;

namespace TXServer.Core.Battles.Effect
{
    public class LifeStealModule : BattleModule
    {
        public LifeStealModule(MatchPlayer matchPlayer, Entity garageModule) : base(
            matchPlayer,
            ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
        ) { }

        public override void Activate()
        {
            if (IsOnCooldown || MatchPlayer.TankState == TankState.Dead || IsEmpLocked) return;

            if (!(MatchPlayer.Tank.CurrentHealth < MatchPlayer.Tank.MaxHealth)) return;

            CurrentAmmunition--;

            EffectEntity = LifestealEffectTemplate.CreateEntity(MatchPlayer);
            MatchPlayer.Battle.PlayersInMap.ShareEntities(EffectEntity);

            Damage.DealHeal(MatchPlayer.Tank.MaxHealth * AdditiveHpFactor + FixedHp, MatchPlayer);

            MatchPlayer.Battle.PlayersInMap.SendEvent(new TriggerEffectExecuteEvent(), EffectEntity);

            Schedule(() => { MatchPlayer.Battle.PlayersInMap.UnshareEntities(EffectEntity); });
        }

        public override void Init()
        {
            base.Init();

            AdditiveHpFactor = Config.GetComponent<ModuleLifestealEffectAdditiveHPFactorPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
            FixedHp = Config.GetComponent<ModuleLifestealEffectFixedHPPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
        }

        public override void On_EnemyKill()
        {
            base.On_Death();
            Activate();
        }

        private float AdditiveHpFactor { get; set; }
        private float FixedHp { get; set; }
    }
}
