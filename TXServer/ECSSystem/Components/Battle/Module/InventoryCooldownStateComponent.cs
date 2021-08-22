using System;

using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Module
{
	[SerialVersionUID(1486635434064L)]
	public class InventoryCooldownStateComponent : Component
    {
		public InventoryCooldownStateComponent(int cooldownTime, DateTime cooldownStartTime)
        {
			CooldownTime = cooldownTime;
			CooldownStartTime = cooldownStartTime;
		}

		public int CooldownTime { get; set; }

		public DateTime CooldownStartTime { get; set; }
	}
}
