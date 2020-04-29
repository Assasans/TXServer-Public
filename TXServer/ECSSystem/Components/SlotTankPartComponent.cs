using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(636326081851010949L)]
    public class SlotTankPartComponent : Component
    {
        public SlotTankPartComponent(TankPartModuleType tankPart)
        {
            TankPart = tankPart;
        }

        public TankPartModuleType TankPart { get; set; }
    }
}
