using System;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.DailyBonus
{
    [SerialVersionUID(636462622709176439L)]
	public class UserDailyBonusNextReceivingDateComponent : Component
	{
        public UserDailyBonusNextReceivingDateComponent(Player player)
        {
            Date = player.Data.DailyBonusNextReceiveDate;
            TotalMillisLength = (long) (Date - DateTime.UtcNow).TotalMilliseconds;
            SelfOnlyPlayer = player;
        }

		[OptionalMapped] public DateTime Date { get; set; } = DateTime.UtcNow;
        public long TotalMillisLength { get; set; } = 24000000;
	}
}
