using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Module.JumpImpact
{
	[SerialVersionUID(1538548472363L)]
	public class JumpEffectConfigComponent : Component
    {
		public JumpEffectConfigComponent(float forceUpgradeMult) => ForceUpgradeMult = forceUpgradeMult;

        public float ForceUpgradeMult { get; set; }
	}
}
