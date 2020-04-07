using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(636341573884178402)]
    public class ModuleBehaviourTypeComponent : Component
    {
        public ModuleBehaviourTypeComponent(ModuleBehaviourType Type)
        {
            this.Type = Type;
        }

        public ModuleBehaviourType Type { get; set; }
    }
}
