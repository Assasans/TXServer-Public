using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1496052424091)]
    public class BattleLobbyGroupComponent : GroupComponent
    {
        public BattleLobbyGroupComponent(Entity entity) : base(entity)
        {
            
        }

        public BattleLobbyGroupComponent(long Key) : base(Key)
        {
        }
    }
}