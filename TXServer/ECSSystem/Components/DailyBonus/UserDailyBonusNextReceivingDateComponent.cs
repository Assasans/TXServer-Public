using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.DailyBonus
{
    [SerialVersionUID(636462622709176439L)]
	public class UserDailyBonusNextReceivingDateComponent : Component
	{
        public UserDailyBonusNextReceivingDateComponent(DateTime date)
        {
            Date = date;
            TotalMillisLength = (long) (date - DateTime.UtcNow).TotalMilliseconds;
        }

		[OptionalMapped]
		public DateTime Date { get; set; } = DateTime.UtcNow;

		public long TotalMillisLength { get; set; } = 24000000;
	}
}
