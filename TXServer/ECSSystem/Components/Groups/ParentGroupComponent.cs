using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(635908808598551080)]
    public class ParentGroupComponent : GroupComponent
    {
        public ParentGroupComponent(Entity Key) : base(Key)
        {
        }

        public ParentGroupComponent(long Id) : base(Id)
        {
        }
    }
}
