﻿using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Module;
using TXServer.ECSSystem.Components.Battle.Tank;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Effect {
	[SerialVersionUID(1538451741218L)]
	public class JumpEffectTemplate : IEntityTemplate {
		public static Entity CreateEntity(MatchPlayer matchPlayer) {
			Entity entity = new(
				new TemplateAccessor(new JumpEffectTemplate(), "/battle/effect/jumpimpact"),
				new EffectComponent(),
				new JumpEffectConfigComponent(15.0f),
				matchPlayer.Tank.GetComponent<TankGroupComponent>()
			);
			return entity;
		}
	}
}