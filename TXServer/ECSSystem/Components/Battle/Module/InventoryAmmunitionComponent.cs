﻿using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Module {
	[SerialVersionUID(636383014039871905L)]
	public class InventoryAmmunitionComponent : Component {
		public int MaxCount { get; set; }

		public int CurrentCount { get; set; }
	}
}
