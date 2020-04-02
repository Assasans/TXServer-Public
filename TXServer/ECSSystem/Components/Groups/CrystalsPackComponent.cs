using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1507791399668)]
	public class CrystalsPackComponent : Component
	{
		public long Amount { get; set; } = 0;

		public long Bonus { get; set; } = 0;
	}
}
