using System.Collections.Generic;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using static TXServer.ECSSystem.Base.Entity;

namespace TXServer.Core.ECSSystem.Events
{
	[SerialVersionUID(1545371971895)]
	public class UpdateClientFractionScoresEvent : ECSEvent
	{
		public Dictionary<Entity, long> Scores { get; set; } = new Dictionary<Entity, long>()
		{
			{ GlobalEntities.FRACTIONSCOMPETITION_FRACTIONS_FRONTIER, 1000000},
			{ GlobalEntities.FRACTIONSCOMPETITION_FRACTIONS_ANTAEUS, 1000000}
		};

		public long TotalCryFund { get; set; } = 1000000;
	}
}
