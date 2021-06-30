using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.User
{
    [SerialVersionUID(636389758870600269)]
    public class GameplayChestScoreComponent : Component
    {

        public GameplayChestScoreComponent(long current)
        {
            Current = current;
        }

        public long Current { get; set; }
        public long Limit { get; set; } = 1000;
    }
}
