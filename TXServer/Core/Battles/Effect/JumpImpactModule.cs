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


            /*MatchPlayer.Tank.ChangeComponent<TemperatureComponent>(component =>
                component.Temperature += WorkingTemperature);*/

            Schedule(() => {
				MatchPlayer.Battle.JoinedTankPlayers.UnshareEntities(EffectEntity);
			});
		}

        public override void Init()
        {
            ForceMultiplier = Config.GetComponent<JumpImpactForceMultPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
            WorkingTemperature = Config.GetComponent<JumpImpactWorkingTemperaturePropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
        }

        private float ForceMultiplier { get; set; }
        private float WorkingTemperature { get; set; }
	}
}
