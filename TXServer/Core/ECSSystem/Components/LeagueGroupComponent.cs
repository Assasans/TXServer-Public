namespace TXServer.Core.ECSSystem.Components
{
    [SerialVersionUID(1503298026299)]
    public class LeagueGroupComponent : Component
    {
        public LeagueGroupComponent(long Key)
        {
            this.Key = Key;
        }

        public static ulong ComponentSerialUID { get; } = 1503298026299;
        public long Key { get; set; }
    }
}
