using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1494535525136L)]
    public class SlotIndexComponent : Component
    {
        public SlotIndexComponent(int Index)
        {
            this.Index = Index;
        }

        public int Index { get; set; }
    }
}
