using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.Core.ECSSystem.Events
{
    [SerialVersionUID(1478774431678)]
    public class ClientLaunchEvent : ECSEvent
    {
        public void Execute(Player player, Entity clientSession)
        {
            clientSession.AddComponent(new WebIdComponent());
        }

        public string WebId { get; set; }
    }
}
