namespace TXServer.Core.ECSSystem.Components
{
    [SerialVersionUID(636389758870600269)]
    public class GameplayChestScoreComponent : Component
    {
        public long Current { get; set; }

        public long Limit { get; set; } = 1000;
    }
}
