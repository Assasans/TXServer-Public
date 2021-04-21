using System;

using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Module {
	[SerialVersionUID(636367520290400984L)]
	public class InventorySlotTemporaryBlockedByServerComponent : Component {
		public InventorySlotTemporaryBlockedByServerComponent(long blockTimeMs, DateTime startBlockTime) {
			BlockTimeMS = blockTimeMs;
			StartBlockTime = startBlockTime;
		}

		public long BlockTimeMS { get; set; }

		public DateTime StartBlockTime { get; set; }
	}
}
