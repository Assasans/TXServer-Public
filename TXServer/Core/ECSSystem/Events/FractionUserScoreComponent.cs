using TXServer.Core.ECSSystem.Components;

namespace TXServer.Core.ECSSystem.Events
{
    [SerialVersionUID(1544590059379)]
	public class FractionUserScoreComponent : Component
	{
		public FractionUserScoreComponent(long TotalEarnedPoints)
		{
			this.TotalEarnedPoints = TotalEarnedPoints;
		}

		public long TotalEarnedPoints { get; set; }
	}
}
