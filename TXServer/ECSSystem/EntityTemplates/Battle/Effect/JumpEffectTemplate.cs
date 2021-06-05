using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Module;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Effect
{
	[SerialVersionUID(1538451741218L)]
	public class JumpEffectTemplate : EffectBaseTemplate
    {
        private static readonly string _configPath = "/battle/effect/jumpimpact";

		public static Entity CreateEntity(MatchPlayer matchPlayer, float forceMultiplier)
        {
            Entity effect = CreateEntity(new JumpEffectTemplate(), _configPath, matchPlayer);
            effect.AddComponent(new JumpEffectConfigComponent(forceMultiplier));

            return effect;
		}
	}
}
