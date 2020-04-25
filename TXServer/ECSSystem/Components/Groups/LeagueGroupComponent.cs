using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1503298026299)]
    public class LeagueGroupComponent : GroupComponent
    {
        public LeagueGroupComponent(Entity Key) : base(Key)
        {
        }

        public LeagueGroupComponent(long Key) : base(Key)
        {
        }
    }
}
