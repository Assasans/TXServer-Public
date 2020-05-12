using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(636185325862880582)]
    public class PackIdComponent : Component
    {
        public PackIdComponent(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
