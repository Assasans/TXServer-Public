using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Components
{
	[SerialVersionUID(636462622709176439L)]
	public class UserDailyBonusNextReceivingDateComponent : Component
	{
		[OptionalMapped]
		public TXDate Date { get; set; } = new TXDate(DateTimeOffset.Now);

		public long TotalMillisLength { get; set; } = 24000000;
	}
}
