using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1485852459997L)]
    public class ModuleGroupComponent : GroupComponent
    {
        public ModuleGroupComponent(Entity entity) : base(entity)
        {
        }

        public ModuleGroupComponent(long key) : base(key)
        {
        }
    }
}
