using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1436338996992L)]
    public class ExperienceItemComponent : Component
    {
        public long Experience { get; set; } = 100000;
    }
}
