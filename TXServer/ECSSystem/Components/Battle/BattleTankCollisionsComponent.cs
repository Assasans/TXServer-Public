using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle
{
    [SerialVersionUID(6549840349742289518L)]
	public class BattleTankCollisionsComponent : Component
	{
		public long SemiActiveCollisionsPhase { get; set; }
	}
}