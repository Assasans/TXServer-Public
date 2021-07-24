using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1522324991586L)]
	public class UpdateTopLeagueInfoEvent : ECSEvent
	{
		public UpdateTopLeagueInfoEvent(long userId)
        {
            Player searchedPlayer = Server.Instance.FindPlayerByUid(userId);

			UserId = userId;
            Place = Leveling.GetSeasonPlace(searchedPlayer);
        }

		public long UserId { get; set; }

		public double LastPlaceReputation { get; set; } = 1;

		public int Place { get; set; }
	}
}
