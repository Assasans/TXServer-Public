using System.Collections.Generic;
using static TXServer.Core.ECSSystem.Entity;

namespace TXServer.Core.ECSSystem.Events
{
	[SerialVersionUID(1545371971895)]
	public class UpdateClientFractionScoresEvent : ECSEvent
	{
		public Dictionary<long, long> Scores { get; set; } = new Dictionary<long, long>()
		{
			{ GlobalEntities.FRACTIONSCOMPETITION_FRACTIONS_FRONTIER.EntityId, 1000000},
			{ GlobalEntities.FRACTIONSCOMPETITION_FRACTIONS_ANTAEUS.EntityId, 1000000}
		};

		public long TotalCryFund { get; set; } = 1000000;
	}
}
