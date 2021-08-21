using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1437990639822L)]
    public class CheckUserUidEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            if (Server.Instance.Database.IsUsernameAvailable(Uid))
            {
                player.SendEvent(new UserUidVacantEvent(Uid), entity);
            }
            else
            {
                player.SendEvent(new UserUidOccupiedEvent(Uid), entity);
            }
        }

        public string Uid { get; set; }
    }
}
