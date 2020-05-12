using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(636324457894395944)]
    public class ModuleTankPartComponent : Component
    {
        public ModuleTankPartComponent(TankPartModuleType TankPart)
        {
            this.TankPart = TankPart;
        }

        public TankPartModuleType TankPart { get; set; }
    }
}
