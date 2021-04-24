using System.Threading;
using System.Threading.Tasks;

using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Module;
using TXServer.ECSSystem.EntityTemplates.Item.Module;

namespace TXServer.Core.Battles.Module {
	public class JumpImpactModule : BattleModule {
		public JumpImpactModule(MatchPlayer player, Entity garageModule) : base(
			player,
			ModuleUserItemTemplate.CreateEntity(garageModule, player.Player.BattlePlayer)
		) { }

		public override async void Activate() {
			if(!Player.Tank.HasComponent<JumpEffectComponent>()) {
				Player.Tank.AddComponent(
					new JumpEffectComponent() {
						BaseImpact = 150000,
						ScaleByMass = true,
						FlyComponentDelayInMs = 10,
						AlwaysUp = false,
						GravityPenalty = 3.0f
					}
				);
			}

			if(!Player.Tank.HasComponent<JumpEffectConfigComponent>()) {
				Player.Tank.AddComponent(new JumpEffectConfigComponent(12.00f));
			}

			await Task.Delay(10);

			if(Player.Tank.HasComponent<JumpEffectConfigComponent>()) {
				Player.Tank.RemoveComponent<JumpEffectComponent>();
			}

			if(Player.Tank.HasComponent<JumpEffectConfigComponent>()) {
				Player.Tank.RemoveComponent<JumpEffectConfigComponent>();
			}
		}

		public override void Deactivate() { }
	}
}
