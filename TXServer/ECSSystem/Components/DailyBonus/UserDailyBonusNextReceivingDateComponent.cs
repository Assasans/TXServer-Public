using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(636462622709176439L)]
	public class UserDailyBonusNextReceivingDateComponent : Component
	{
		[OptionalMapped]
		public DateTime Date { get; set; } = DateTime.Now;

		public long TotalMillisLength { get; set; } = 24000000;
	}
}
