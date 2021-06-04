using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1485846320654L)]
    public class SlotUserItemInfoComponent : Component
    {
        public SlotUserItemInfoComponent(Slot slot, ModuleBehaviourType moduleBehaviourType)
        {
            Slot = slot;
            ModuleBehaviourType = moduleBehaviourType;
        }

        public Slot Slot { get; set; }

        public ModuleBehaviourType ModuleBehaviourType { get; set; }

        public int UpgradeLevel { get; set; } = 1;
    }
}
