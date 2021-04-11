using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.User.Squad
{
    [SerialVersionUID(1510746420839L)]
    public class InviteToSquadRejectedEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            
        }
    }
}