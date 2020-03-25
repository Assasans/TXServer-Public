namespace TXServer.Core.ECSSystem.Components
{
    [SerialVersionUID(7453043498913563889)]
    public sealed class UserGroupComponent : Component
    {
        public UserGroupComponent(long Key)
        {
            this.Key = Key;
        }

        public static ulong ComponentSerialUID { get; } = 7453043498913563889;
        public long Key { get; set; }
    }
}
