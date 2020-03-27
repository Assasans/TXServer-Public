using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(636389758870600269)]
    public class GameplayChestScoreComponent : Component
    {
        public long Current { get; set; }

        public long Limit { get; set; } = 1000;
    }
}
