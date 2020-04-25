using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1502713060357L)]
	public class LeagueConfigComponent : Component
	{
		public int LeagueIndex { get; set; } = 2;

		public double ReputationToEnter { get; set; } = 0;
	}
}
