using System.Collections.Generic;

namespace TXServer.Core.ECSSystem.Events
{
	[SerialVersionUID(1545371971895)]
	public class UpdateClientFractionScoresEvent : ECSEvent
	{
		public UpdateClientFractionScoresEvent(long TotalCryFund, Dictionary<long, long> Scores)
		{
			this.TotalCryFund = TotalCryFund;
			this.Scores = Scores;
		}

		[Protocol] public Dictionary<long, long> Scores { get; set; }

		[Protocol] public long TotalCryFund { get; set; }
	}
}
