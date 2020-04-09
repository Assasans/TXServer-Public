using System.Collections.Generic;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.Core.ECSSystem.Events
{
	[SerialVersionUID(1545371971895)]
	public class UpdateClientFractionScoresEvent : ECSEvent
	{
		public Dictionary<Entity, long> Scores { get; set; } = new Dictionary<Entity, long>()
		{
			{ Fractions.Frontier, 1000000},
			{ Fractions.Antaeus, 1000000}
		};

		public long TotalCryFund { get; set; } = 1000000;
	}
}
