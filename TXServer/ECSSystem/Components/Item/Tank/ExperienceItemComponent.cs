using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Item.Tank
{
    [SerialVersionUID(1436338996992L)]
    public class ExperienceItemComponent : Component
    {
        public ExperienceItemComponent(long experience)
        {
            Experience = experience;
        }

        public long Experience { get; set; }
    }
}
