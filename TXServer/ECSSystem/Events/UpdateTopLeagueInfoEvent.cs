using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1522324991586L)]
	public class UpdateTopLeagueInfoEvent : ECSEvent
	{
		public UpdateTopLeagueInfoEvent(long UserId)
		{
			this.UserId = UserId;
		}

		public long UserId { get; set; }

		public double LastPlaceReputation { get; set; } = 1;

		public int Place { get; set; } = 1;
	}
}
