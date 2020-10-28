using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Tank
{
    [SerialVersionUID(4088029591333632383)]
    public class TankGroupComponent : GroupComponent
    {
        public TankGroupComponent(Entity entity) : base(entity)
        {
        }

        public TankGroupComponent(long Key) : base(Key)
        {
        }
    }
}