using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Module.JumpImpact;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;

namespace TXServer.Core.Battles.Effect
{
	public class JumpImpactModule : BattleModule
    {
		public JumpImpactModule(MatchPlayer matchPlayer, Entity garageModule) : base(
			matchPlayer,
            ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
		) { }

		public override void Activate()
        {
			EffectEntity = JumpEffectTemplate.CreateEntity(MatchPlayer, ForceMultiplier);
            MatchPlayer.Battle.JoinedTankPlayers.ShareEntities(EffectEntity);

            Schedule(Deactivate);
		}

        public override void Deactivate()
        {
            if (EffectEntity == null) return;

            MatchPlayer.Battle.PlayersInMap.UnshareEntities(EffectEntity);
            EffectEntity = null;
        }

        public override void Init()
        {
            base.Init();
            IsAffectedByEmp = false;

            ForceMultiplier = Config.GetComponent<JumpImpactForceMultPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
            WorkingTemperature = Config.GetComponent<JumpImpactWorkingTemperaturePropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
        }

        private float ForceMultiplier { get; set; }
        private float WorkingTemperature { get; set; }
	}
}
