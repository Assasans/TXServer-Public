using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Tank;

namespace TXServer.ECSSystem.Components.Battle.Module {
	[SerialVersionUID(1505906670608L)]
	public class ForceFieldTransformComponent : Component {
		public ForceFieldTransformComponent(Movement movement) {
			Movement = movement;
		}

		public Movement Movement { get; set; }
	}
}
