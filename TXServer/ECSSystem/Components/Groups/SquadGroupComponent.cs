using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Groups
{
    [SerialVersionUID(1507120787784L)]
    public sealed class SquadGroupComponent : GroupComponent
    {
        public SquadGroupComponent(Entity Key) : base(Key)
        {
        }

        public SquadGroupComponent(long Key) : base(Key)
        {
        }
    }
}