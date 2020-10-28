using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Team
{
    [SerialVersionUID(6955808089218759626)]
    public class TeamGroupComponent : GroupComponent
    {
        public TeamGroupComponent(Entity entity) : base(entity)
        {
        }

        public TeamGroupComponent(long Key) : base(Key)
        {
        }
    }
}