using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(636330378478033958)]
    public class ModuleTierComponent : Component
    {
        public ModuleTierComponent(int TierNumber)
        {
            this.TierNumber = TierNumber;
        }

        public int TierNumber { get; set; }
    }
}
