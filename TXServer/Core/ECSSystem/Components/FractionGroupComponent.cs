namespace TXServer.Core.ECSSystem.Components
{
    [SerialVersionUID(1544510801819)]
	public class FractionGroupComponent : Component
	{
        public FractionGroupComponent(long Key)
        {
            this.Key = Key;
        }

        public static ulong ComponentSerialUID { get; } = 1544510801819;
        public long Key { get; set; }
    }
}
