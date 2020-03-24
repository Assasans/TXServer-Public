namespace TXServer.Core.ECSSystem.Components
{
    [SerialVersionUID(1544510801819)]
	public class FractionGroupComponent : Component
	{
        public FractionGroupComponent(long Key)
        {
            this.Key = Key;
        }

        [Protocol] public static ulong ComponentSerialUID { get; } = 1544510801819;
        [Protocol] public long Key { get; set; }
    }
}
