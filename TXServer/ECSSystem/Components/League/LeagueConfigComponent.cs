using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1502713060357L)]
	public class LeagueConfigComponent : Component
	{
		public LeagueConfigComponent(int leagueIndex, double reputationToEnter)
		{
			LeagueIndex = leagueIndex;
            ReputationToEnter = reputationToEnter;
        }

		public int LeagueIndex { get; set; }

		public double ReputationToEnter { get; set; } = 0;
	}
}
