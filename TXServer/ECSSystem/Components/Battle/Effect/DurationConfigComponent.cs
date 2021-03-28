using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle
{
    [SerialVersionUID(482294559116673084L)]
    public class DurationConfigComponent : Component
    {
        public DurationConfigComponent(long duration)
        {
            Duration = duration;
        }

        public long Duration { get; set; }
    }
}