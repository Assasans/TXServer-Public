using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1476091404409L)]
	public class QuestProgressComponent : Component
	{
		public float PrevValue { get; set; } = 0;

		public float CurrentValue { get; set; } = 0;

		public float TargetValue { get; set; } = 100;

		public bool PrevComplete { get; set; } = true;

		public bool CurrentComplete { get; set; } = true;
	}
}
