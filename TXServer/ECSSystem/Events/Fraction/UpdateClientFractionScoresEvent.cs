using System.Collections.Generic;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.Events.Fraction
{
	[SerialVersionUID(1545371971895)]
	public class UpdateClientFractionScoresEvent : ECSEvent
	{
		public Dictionary<Entity, long> Scores { get; set; } = new()
		{
			{ Fractions.GlobalItems.Frontier, Server.Instance.ServerData.FrontierScore },
			{ Fractions.GlobalItems.Antaeus, Server.Instance.ServerData.AntaeusScore }
		};

		public long TotalCryFund { get; set; } = Server.Instance.ServerData.FractionsCompetitionCryFund;
	}
}
